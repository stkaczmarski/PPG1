using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PickableObject : MonoBehaviour, IInteractable
{
    private Rigidbody rb;
    private Collider col;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
    }

    public void StartInteract(Transform holdPoint)
    {
        rb.isKinematic = true;

        // col.enabled = false; 

        transform.SetParent(holdPoint);
        Debug.Log("Interakcja z obiektem: " + gameObject.name);

        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }

    public void StopInteract()
    {
        transform.SetParent(null);

        rb.isKinematic = false;

        // col.enabled = true;

        //rb.AddForce(Camera.main.transform.forward * 100f, ForceMode.VelocityChange);
        //for(int i = 0; i < 100; i++)
        //{
        //    Instantiate(gameObject);
        //    rb.AddForce(Camera.main.transform.forward * 100f, ForceMode.VelocityChange);
        //}
    }
}