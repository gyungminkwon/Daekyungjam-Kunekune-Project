using UnityEngine;

public class SingleInteractable : MonoBehaviour, IInteractable
{
    public enum InteractType 
    { 
        BedroomKey, Doll, ClassroomKey, Trowel, BusTicket, FuneralStand, BrokenPot, Mirror
    }
    
    public enum RequiredCondition { None, PotBroken }
    
    [Header("Interact Settings")]
    public InteractType interactType;
    public string interactName;
    
    [Header("실패")]
    public RequiredCondition requiredCondition;
    [Tooltip("상호 작용 실패 시 출력할 내용")]
    public TextData failMonologue;
    
    [Header("성공")]
    [Tooltip("상호 작용 성공 시 출력할 내용")]
    public TextData successMonologue; 
    [Tooltip("사물 이미지 변경 시")]
    public Sprite afterEventSprite; 

    private bool isTriggered = false;
    private SpriteRenderer sr;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    public void Interact()
    {
        if (isTriggered) return;

        bool canInteract = true;
        if (requiredCondition == RequiredCondition.PotBroken)
        {
            canInteract = GameProgressManager.Instance.isPot;
        }

        if (!canInteract)
        {
            if (failMonologue != null) TextManager.Instance.PlayText(failMonologue);
            else Debug.Log($"상호 작용 실패");
            return;
        }

        switch (interactType)
        {
            case InteractType.BedroomKey: GameProgressManager.Instance.hasBedroomKey = true; break;
            case InteractType.Doll: GameProgressManager.Instance.hasDoll = true; break;
            case InteractType.ClassroomKey: GameProgressManager.Instance.hasClassroomKey = true; break;
            case InteractType.Trowel: GameProgressManager.Instance.hasTrowel = true; break;
            case InteractType.BusTicket: GameProgressManager.Instance.hasBusTicket = true; break;
            
            case InteractType.FuneralStand: GameProgressManager.Instance.isFuneralStand = true; break;
            case InteractType.BrokenPot: GameProgressManager.Instance.isPot = true; break;
        }

        if (afterEventSprite != null && sr != null)
        {
            sr.sprite = afterEventSprite;
        }

        if (successMonologue != null)
        {
            TextManager.Instance.PlayText(successMonologue);
        }

        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        Debug.Log($"{interactName} 상호 작용 성공");
        isTriggered = true;
    }

    public string GetInteractPrompt()
    {
        if (isTriggered) return "";
        return $"{interactName} (F)";
    }
}