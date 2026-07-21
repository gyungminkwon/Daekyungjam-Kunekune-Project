using UnityEngine;

public class Bulkhead : MonoBehaviour, IInteractable
{
    [SerializeField] private Sprite closedSprite;
    [SerializeField] private Sprite openedSprite;

    private SpriteRenderer sr;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        sr.sprite = closedSprite;
    }

    public void Interact()
    {
        if (sr.sprite == openedSprite) sr.sprite = closedSprite;
        else sr.sprite = openedSprite;
    }

    public string GetInteractPrompt()
    {
        return "문 열기 (F)";
    }
}
