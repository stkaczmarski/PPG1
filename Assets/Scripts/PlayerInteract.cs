using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Ustawienia")]
    [SerializeField] private float interactionDistance = 3f;
    [SerializeField] private LayerMask interactionLayer;

    [Header("Referencje")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private Transform holdPoint;

    private IInteractable currentObject;

    private void Update()
    {
        if (currentObject != null)
        {
            HandleHolding();
        }
        else
        {
            HandleRaycast();
        }
    }

    private void HandleRaycast()
    {
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactionDistance, interactionLayer))
        {
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();

            if (interactable != null)
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    currentObject = interactable;
                    currentObject.StartInteract(holdPoint);
                }
            }
        }
    }

    private void HandleHolding()
    {
        if (Input.GetKeyUp(KeyCode.E))
        {
            currentObject.StopInteract();
            currentObject = null;
        }
    }
}