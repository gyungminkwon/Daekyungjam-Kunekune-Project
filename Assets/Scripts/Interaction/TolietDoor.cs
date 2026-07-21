using UnityEngine;

public class ToiletDoor : MonoBehaviour, IInteractable
{
    [Header("Door Settings")]
    public Sprite openedSprite;
    
    [Tooltip("Toilet Bowl")]
    public ToiletBowlInteractable innerToiletBowl; 
    
    private SpriteRenderer sr;
    private Collider2D innerBowlCollider;
    private bool isOpened = false;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        
        if (innerToiletBowl != null)
        {
            innerBowlCollider = innerToiletBowl.GetComponent<Collider2D>();
            if (innerBowlCollider != null) innerBowlCollider.enabled = false;
        }
    }

    public void Interact()
    {
        if (isOpened) return;

        isOpened = true;
        sr.sprite = openedSprite;
        
        if (innerBowlCollider != null) innerBowlCollider.enabled = true;
        
        GameProgressManager.Instance.toiletOpenedCount++;

        if (GameProgressManager.Instance.toiletOpenedCount == 4)
        {
            // TODO: 쿠네쿠네 빼꼼

            if (innerToiletBowl != null)
            {
                innerToiletBowl.isEventBowl = true;
            }
        }
    }

    public string GetInteractPrompt()
    {
        return isOpened ? "" : "문 열기 (F)";
    }
}