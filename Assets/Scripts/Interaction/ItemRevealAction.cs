using UnityEngine;

public class ItemRevealAction : MonoBehaviour
{
    [Header("References")]
    [SerializeField, Tooltip("나타날 아이템의 SpriteRenderer (비워두면 자동 할당)")]
    private SpriteRenderer itemSpriteRenderer;
    [SerializeField, Tooltip("재생할 Animator (없으면 비워두세요)")]
    private Animator itemAnimator;

    private static readonly int InteractHash = Animator.StringToHash("Interact");

    private void Awake()
    {
        if (itemSpriteRenderer == null) itemSpriteRenderer = GetComponent<SpriteRenderer>();
        if (itemAnimator == null) itemAnimator = GetComponent<Animator>();

        // 시작할 때 아이템 숨기기
        if (itemSpriteRenderer != null) itemSpriteRenderer.enabled = false;
    }

    // InteractTrigger의 이벤트에서 호출할 함수
    public void RevealItem()
    {
        if (itemSpriteRenderer != null) itemSpriteRenderer.enabled = true;
        if (itemAnimator != null) itemAnimator.SetTrigger(InteractHash);
    }
}