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
        if (targetPos == null) return;

        // 초반에 가지 못하는 곳 등 권한 제어
        if (GameManager.Instance.currentStage < unlockStage)
        {
            if (lockedMonologue != null) TextManager.Instance.PlayText(lockedMonologue);
            return;
        }

        StartCoroutine(InteractRoutine());
    }

    private IEnumerator InteractRoutine()
    {
        if (openedSprite != null) sr.sprite = openedSprite;
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        PlayerInput playerInput = player.GetComponent<PlayerInput>();
        playerInput.enabled = false;

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
        playerInput.enabled = true;

        if (closedSprite != null) sr.sprite = closedSprite;
    }

    public string GetInteractPrompt()
    {
        return "문 열기(F)";
    }
}
