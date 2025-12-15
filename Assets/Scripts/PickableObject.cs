using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PickableObject : MonoBehaviour, IInteractable
{
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public bool StartInteract(Transform holdPoint, GameObject player)
    {
        rb.isKinematic = true; 
        transform.SetParent(holdPoint);
        Debug.Log("Interakcja z obiektem: " + gameObject.name);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        return true;
    }

    public void StopInteract()
    {
        transform.SetParent(null);
        rb.isKinematic = false;
        rb.AddForce(Camera.main.transform.forward * 10f, ForceMode.VelocityChange);
    }
}