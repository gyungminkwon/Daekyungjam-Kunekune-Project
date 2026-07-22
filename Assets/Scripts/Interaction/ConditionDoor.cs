using System.Collections;
using UnityEngine;

[RequireComponent(typeof(NewDoor))]
public class ConditionDoor : MonoBehaviour, IInteractable
{
    public enum RequiredItem { None, BedroomKey, Doll, ClassroomKey, Trowel }
    
    [Header("Door Settings")]
    public string doorName = "문";
    public RequiredItem requiredItem;
    
    [Header("Unlock Restrictions")]
    [SerializeField] private Stage unlockStage;
    [SerializeField] private TextData lockedMonologue;

    [Header("Sprites")]
    public Sprite closedSprite;
    public Sprite openedSprite;

    private SpriteRenderer sr;
    private NewDoor doorTeleporter;
    private bool isOpen = false;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        doorTeleporter = GetComponent<NewDoor>();
    }

    private void Start()
    {
        if (closedSprite != null) sr.sprite = closedSprite;
    }

    public void OnInteractPressed()
    {
        if (GameManager.Instance.currentStage < unlockStage)
        {
            if (lockedMonologue != null) TextManager.Instance.PlayText(lockedMonologue);
            else Debug.Log("잠김");
            return;
        }

        if (isOpen)
        {
            StartCoroutine(EnterDoorRoutine());
            return;
        }

        bool canOpen = false;
        switch (requiredItem)
        {
            case RequiredItem.None: canOpen = true; break;
            case RequiredItem.BedroomKey: canOpen = GameProgressManager.Instance.hasBedroomKey; break;
            case RequiredItem.Doll: canOpen = GameProgressManager.Instance.hasDoll; break;
            case RequiredItem.ClassroomKey: canOpen = GameProgressManager.Instance.hasClassroomKey; break;
            case RequiredItem.Trowel: canOpen = GameProgressManager.Instance.hasTrowel; break;
        }

        if (canOpen)
        {
            isOpen = true;
            Debug.Log($"{doorName} 해제");
            StartCoroutine(EnterDoorRoutine());
        }
        else
        {
            if (lockedMonologue != null) TextManager.Instance.PlayText(lockedMonologue);
            Debug.Log($"{doorName} 잠김");
        }
    }

    public void OnInteractHeld() {}
    public void OnInteractReleased() {}

    private IEnumerator EnterDoorRoutine()
    {
        if (openedSprite != null) sr.sprite = openedSprite;

        yield return StartCoroutine(doorTeleporter.TeleportRoutine());

        if (closedSprite != null) sr.sprite = closedSprite;
    }

    public string GetInteractPrompt()
    {
        return $"{doorName} 열기 (F)";
    }
}