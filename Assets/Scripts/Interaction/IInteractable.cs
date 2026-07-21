using UnityEngine;

public interface IInteractable
{
    public void OnInteractPressed();
    public void OnInteractHeld();
    public void OnInteractReleased();
    public string GetInteractPrompt();
}
