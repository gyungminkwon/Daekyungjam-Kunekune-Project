using UnityEngine;

public class PlayerGrowthManager : MonoBehaviour
{
    public enum GrowthStage { Child, Student, Adult }

    [Header("Current Status")]
    public GrowthStage currentStage = GrowthStage.Adult;

    [Header("Growth Controllers")]
    [SerializeField] private RuntimeAnimatorController childController;
    [SerializeField] private RuntimeAnimatorController studentController;
    [SerializeField] private RuntimeAnimatorController adultController;

    [Header("2.5D Collider Settings")]
    [Tooltip("발 바닥에서부터 콜라이더를 얼마나 띄울지 (2.5D 입체감 효과, 기본값: 0.2)")]
    [SerializeField] private float bottomGap = 0.2f;

    // ★ [핵심 변경] 나이대별 앉았을 때의 캡슐 콜라이더 높이(Size Y)를 직접 지정합니다!
    [Header("Crouch Height Settings (앉기 콜라이더 높이)")]
    [SerializeField] private float adultCrouchHeight = 2.0f;     // 성인 앉기 높이
    [SerializeField] private float studentCrouchHeight = 1.5f;   // 학생 앉기 높이
    [SerializeField] private float childCrouchHeight = 1.3125f;  // ★ 어린이 앉기 높이 (1.3125 고정!)

    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private CapsuleCollider2D capsuleCollider;
    private PlayerInput playerInput;

    private bool isCurrentlyCrouching = false;
    private float currentStandingHeight = 3.0f;
    private float currentStandingWidth = 0.9f;

    void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        playerInput = GetComponent<PlayerInput>();
    }

    void Start()
    {
        SetGrowthStage(currentStage);
    }

    void Update()
    {
        if (playerInput == null || capsuleCollider == null) return;

        // S키(앉기) 입력 상태가 변할 때마다 콜라이더 크기 조절 함수를 호출합니다.
        if (playerInput.IsCrouch && !isCurrentlyCrouching)
        {
            ApplyColliderSize(true);
        }
        else if (!playerInput.IsCrouch && isCurrentlyCrouching)
        {
            ApplyColliderSize(false);
        }
    }

    public void RegressToPreviousStage()
    {
        switch (currentStage)
        {
            case GrowthStage.Adult:
                SetGrowthStage(GrowthStage.Student);
                break;
            case GrowthStage.Student:
                SetGrowthStage(GrowthStage.Child);
                break;
            case GrowthStage.Child:
                break;
        }
    }

    public void SetGrowthStage(GrowthStage stage)
    {
        if (animator == null) return;
        currentStage = stage;

        switch (stage)
        {
            case GrowthStage.Child:
                animator.runtimeAnimatorController = childController;
                break;
            case GrowthStage.Student:
                animator.runtimeAnimatorController = studentController;
                break;
            case GrowthStage.Adult:
                animator.runtimeAnimatorController = adultController;
                break;
        }

        // 애니메이션 교체 후 0.1초 뒤에 바뀐 이미지 크기를 다시 읽어옵니다.
        Invoke(nameof(RecalculateSpriteBounds), 0.1f);
    }

    private void RecalculateSpriteBounds()
    {
        if (spriteRenderer == null || spriteRenderer.sprite == null) return;

        // 어려졌을 때 변한 실제 스프라이트의 너비(X)와 높이(Y)를 저장
        currentStandingWidth = spriteRenderer.sprite.bounds.size.x;
        currentStandingHeight = spriteRenderer.sprite.bounds.size.y;

        Debug.Log($"[스프라이트 갱신] {currentStage} - Width(X): {currentStandingWidth}, Height(Y): {currentStandingHeight}");

        ApplyColliderSize(isCurrentlyCrouching);
    }

    // ★ [핵심 연산] 발바닥을 고정한 상태로 앉기 높이(1.3125 등)를 정확히 적용하는 함수
    private void ApplyColliderSize(bool isCrouch)
    {
        if (capsuleCollider == null) return;
        isCurrentlyCrouching = isCrouch;

        // 1. X축 크기: 어려질 때 얇아진 이미지 너비를 100% 그대로 반영!
        float targetWidth = currentStandingWidth;
        float targetHeight;

        // 2. Y축 크기: 앉았을 때는 나이대별 지정 숫자(1.3125 등)를 딱 맞게 적용!
        if (isCrouch)
        {
            switch (currentStage)
            {
                case GrowthStage.Adult:
                    targetHeight = adultCrouchHeight;
                    break;
                case GrowthStage.Student:
                    targetHeight = studentCrouchHeight;
                    break;
                case GrowthStage.Child:
                    targetHeight = childCrouchHeight; // ★ 여기에서 1.3125가 정확하게 들어갑니다!
                    break;
                default:
                    targetHeight = 1.3125f;
                    break;
            }
        }
        else
        {
            // 서 있을 때는 스프라이트 원래 높이에서 발 띄움 간격(bottomGap)만 뺍니다.
            targetHeight = currentStandingHeight - bottomGap;
        }

        // 3. ★ [바닥 고정 공식] 중심(Offset.Y) = 바닥 띄움(0.2) + (높이 / 2)
        // 높이가 1.3125가 되든 2가 되든, 발바닥 선은 0.1밀리미터도 움직이지 않고 완벽 고정됩니다!
        float targetOffsetY = bottomGap + (targetHeight / 2f);

        capsuleCollider.size = new Vector2(targetWidth, Mathf.Max(targetHeight, 0.5f));
        capsuleCollider.offset = new Vector2(0f, targetOffsetY);

        Debug.Log($"[{currentStage} 콜라이더 적용] 앉음: {isCrouch} / Size Y: {targetHeight:F4} / Offset Y: {targetOffsetY:F4}");
    }
}