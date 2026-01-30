using UnityEngine;
using TMPro;
using UnityEditor.ShaderGraph.Internal;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Rigidbody _rb;
    [Header("References")]
    public Animator animator;
    public Transform cameraTransform;

    [Header("Player Movement")]
    public float moveSpeed = 5f;
    public float crouchSpeed = 2f;
    public float runningSpeed = 8f;
    public float rotationSpeed = 10f;
    public float jumpForce = 5f;
    public float aimMoveSpeed = 3f;

    [Header("Stamina Settings")]
    public float maxStamina = 100f;
    public float staminaDrainRate = 15f;
    public float staminaRegenRate = 10f;
    public TextMeshProUGUI staminaText;

    private float currentStamina;

    [Header("Camera Settings")]
    public Vector3 cameraOffset = new Vector3(0, -2, -5);
    public float mouseSensitivity = 2f;
    public float zoomSpeed = 5f;
    public float minZoom = 2f;
    public float maxZoom = 10f;
    public float normalFov = 60f;
    public float aimFov = 45f;
    public float fovChangeSpeed = 10f;

    private float currentZoom = 5f;
    private float yaw = 0f;
    private float pitch = 15f;

    private bool isCrouching = false;
    private bool isRunning = false;

    public Transform firstPersonPivot;
    public bool isFirstPerson = false;
    public Weapon weaponScript;

    private Renderer[] allRenderers;

    private Camera mainCamera;

    private void Start()
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        if (_rb == null)
            _rb = GetComponent<Rigidbody>();

        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;

        if (cameraTransform != null)
            mainCamera = cameraTransform.GetComponent<Camera>();//

        currentZoom = Mathf.Clamp(currentZoom, minZoom, maxZoom);
        GetRenderers();

        currentStamina = maxStamina;
        UpdateStaminaUI();
    }

    private void Update()
    {
        if (DialogueManager.instance != null && DialogueManager.instance.isDialogueActive)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            if (animator != null) animator.SetBool("isWalking", false);

            return;
        }

        HandleJumping();
        HandleCrouching();
        HandleRunning();
        HandleCamera();
        HandleModelVisibility();
        HandleWeaponState();
    }
    private void FixedUpdate()
    {
        if (DialogueManager.instance != null && DialogueManager.instance.isDialogueActive)
        {
            _rb.linearVelocity = new Vector3(0, _rb.linearVelocity.y, 0);
            return;
        }

        HandleMovement();
    }

    private void HandleWeaponState()
    {
        if (weaponScript == null) return;

        weaponScript.gameObject.SetActive(isFirstPerson);
    }

    private void HandleCrouching()
    {
        if(Input.GetKey(KeyCode.C))
        {
            isCrouching = true;
        } else
        {
            isCrouching = false;
        }
        animator.SetBool("isCrouching", isCrouching);
    }

    private void HandleRunning()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        bool isTryingToMove = (Mathf.Abs(h) > 0 || Mathf.Abs(v) > 0);

        if(Input.GetKey(KeyCode.LeftShift) && isTryingToMove && !isCrouching && currentStamina > 0)
        {
            isRunning = true;
            currentStamina -= staminaDrainRate * Time.deltaTime;
        }
        else
        {
            isRunning = false;
            if(currentStamina < maxStamina && !isTryingToMove)
            {
                currentStamina += staminaRegenRate * Time.deltaTime;
            }
        }

        currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
        UpdateStaminaUI();
    }

    private void UpdateStaminaUI()
    {
        if(staminaText != null)
        {
            staminaText.text = "Stamina: " + (int)currentStamina;
        }
    }

    private void HandleMovement()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 move = new Vector3(h, 0, v).normalized;

        bool isWalking = move.magnitude > 0;
        animator.SetBool("isWalking", isWalking);

        if (isWalking)
        {
            float currentMoveSpeed = moveSpeed;

            if (isFirstPerson && weaponScript != null && weaponScript.IsAiming)
            {
                currentMoveSpeed = aimMoveSpeed;
            }
            else if (isCrouching)
            {
                currentMoveSpeed = crouchSpeed;
            }
            else if (isRunning)
            {
                currentMoveSpeed = runningSpeed;
            }

                Vector3 moveDir = Quaternion.Euler(0, cameraTransform.eulerAngles.y, 0) * move;

            Vector3 targetPos = transform.position + moveDir * currentMoveSpeed * Time.fixedDeltaTime;

            _rb.MovePosition(targetPos);

            if(!isFirstPerson)
            {
                Quaternion targetRotation = Quaternion.LookRotation(moveDir);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
            }
        }
    }

    private void HandleJumping()
    {
        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded())
        {
            animator.SetTrigger("isJumping");

            Vector3 vel = _rb.linearVelocity;
            vel.y = 0;
            _rb.linearVelocity = vel;

            _rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    //private IEnumerator Jump()
    //{
    //    animator.SetTrigger("isJumping");

    //    yield return new WaitForSeconds(0.3f);

    //    Vector3 vel = _rb.linearVelocity;
    //    vel.y = 0;
    //    _rb.linearVelocity = vel;

    //    _rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    //}

    //private void HandleJumping()
    //{
    //    if (Input.GetKeyDown(KeyCode.Space) && IsGrounded())
    //    {
    //        StartCoroutine(Jump());
    //    }
    //}

    private bool IsGrounded()
    {
        return Physics.Raycast(transform.position + Vector3.up * 0.1f, Vector3.down, 1.1f);
    }

    private void HandleCamera()
    {
        if (cameraTransform == null) return;

        if (Input.GetKeyDown(KeyCode.V))
            isFirstPerson = !isFirstPerson;

        // zmiana FOV
        if (isFirstPerson && weaponScript != null && mainCamera != null)
        {
            float targetFov = weaponScript.IsAiming ? aimFov : normalFov;
            mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, targetFov, Time.deltaTime * fovChangeSpeed);
        }
        else if (mainCamera != null)
        {
            mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, normalFov, Time.deltaTime * fovChangeSpeed);
        }

        // obrót kamery
        if (isFirstPerson && ( InventoryManager.Instance == null ||  !InventoryManager.Instance.IsInventoryOpen))
        {
            yaw += Input.GetAxis("Mouse X") * mouseSensitivity;
            pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

        }
        else
        {
            if (Input.GetMouseButton(1))
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                yaw += Input.GetAxis("Mouse X") * mouseSensitivity;
                pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity;
            }
        }

        pitch = Mathf.Clamp(pitch, -30f, 60f);
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);

        if (isFirstPerson)
        {
            cameraTransform.position = firstPersonPivot.position;
            cameraTransform.rotation = rotation;
            transform.rotation = Quaternion.Euler(0, yaw, 0);
            return;
        }

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        currentZoom -= scroll * zoomSpeed;
        currentZoom = Mathf.Clamp(currentZoom, minZoom, maxZoom);

        Vector3 desiredPosition =
            transform.position + rotation * (cameraOffset.normalized * -currentZoom);

        cameraTransform.position = desiredPosition;
        cameraTransform.LookAt(transform.position + Vector3.up * 1.5f);
    }

    private void HandleModelVisibility()
    {
        foreach (Renderer r in allRenderers)
        {
            if (isFirstPerson)
                r.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
            else
                r.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
        }
    }

    private void GetRenderers()
    {
        var renderers = GetComponentsInChildren<Renderer>();
        var rendererList = new System.Collections.Generic.List<Renderer>();
        int playerBodyLayer = LayerMask.NameToLayer("PlayerBody");

        foreach (var renderer in renderers)
        {
            if (renderer.gameObject.layer == playerBodyLayer)
            {
                rendererList.Add(renderer);
            }
        }
        allRenderers = rendererList.ToArray();
    }
}
