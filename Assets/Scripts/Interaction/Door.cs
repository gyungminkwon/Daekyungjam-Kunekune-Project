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

    public void OnInteractPressed()
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

    public void OnInteractHeld() {}
    public void OnInteractReleased() {}

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

            /* =========================================================
             * KunekuneAI.cs 연계 내용
             * =========================================================
             * 플레이어가 문을 통해 다른 맵으로 이동 시 쿠네쿠네도 함께 쫓아옴.
             */
            KunekuneAI kunekune = Object.FindFirstObjectByType<KunekuneAI>();
            if (kunekune != null && kunekune.gameObject.activeInHierarchy)
            {
                kunekune.ChaseDoorAndTeleport(transform.position, targetPos.position);
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
