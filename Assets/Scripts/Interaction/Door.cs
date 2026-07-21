using System.Collections;
using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    [Header("Unlock & Link Settings")]
    [SerializeField] private Stage unlockStage;
    [SerializeField] private Transform targetPos;
    
    [Header("Sprite Settings")]
    [SerializeField] Sprite closedSprite;
    [SerializeField] Sprite openedSprite;

    [Header("Text Settings")]
    [SerializeField] private TextData lockedMonologue;

    private SpriteRenderer sr;
    private PlayerInput playerInput;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        if (closedSprite != null) sr.sprite = closedSprite;
    }

    public void Interact()
    {
        if (targetPos == null)
        {
            Debug.Log($"{gameObject.name}: 목적지가 지정되지 않았습니다.");
            return;
        }

        if (GameManager.Instance.currentStage != unlockStage && lockedMonologue != null)
        {
            TextManager.Instance.PlayText(lockedMonologue);
            return;
        }

        StartCoroutine(InteractRoutine());
    }

    private IEnumerator InteractRoutine()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (openedSprite != null) sr.sprite = openedSprite;

        yield return new WaitForSeconds(0.3f);

        // 페이드 아웃/인

        yield return new WaitForSeconds(0.5f);

        if (player != null)
        {
            Rigidbody2D rb = player.GetComponent<Rigidbody2D>();

            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
                rb.position = targetPos.position;
            }
            else
            {
                player.transform.position = targetPos.position;
            }

            Physics2D.SyncTransforms();

            Debug.Log($"[Teleport] {targetPos.name} (으)로 이동했습니다.");
        }

        if (closedSprite != null) sr.sprite = closedSprite;
    }

    public string GetInteractPrompt()
    {
        return "문 열기(F)";
    }
}
