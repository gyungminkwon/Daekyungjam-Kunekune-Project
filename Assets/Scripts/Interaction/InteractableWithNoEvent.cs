using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class InteractableWithNoEvent : MonoBehaviour, IInteractable
{
    [SerializeField] private TextData interactionText;
    [SerializeField] private string objectName;

    void Awake()
    {
        GetComponent<BoxCollider2D>().isTrigger = true;
    }
    public void OnInteractPressed()
    {
        TextManager.Instance.PlayText(interactionText);
    }

    public void OnInteractHeld() {}
    public void OnInteractReleased() {}
    public string GetInteractPrompt()
    {
        return objectName + " (F)";
    }
}
