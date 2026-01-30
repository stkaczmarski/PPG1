using UnityEngine;

public class TextInfo : MonoBehaviour
{
    private Transform camTransform;

    void Start()
    {
        if (Camera.main != null)
            camTransform = Camera.main.transform;
    }

    void LateUpdate()
    {
        if (camTransform == null) return;
        transform.forward = camTransform.forward;
    }
}