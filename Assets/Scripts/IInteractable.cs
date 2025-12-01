using UnityEngine;

public interface IInteractable
{
    void StartInteract(Transform holdPoint);
    void StopInteract();
}