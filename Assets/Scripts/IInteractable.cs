using UnityEngine;

public interface IInteractable
{
    bool StartInteract(Transform holdPoint, GameObject player);
    void StopInteract();
}