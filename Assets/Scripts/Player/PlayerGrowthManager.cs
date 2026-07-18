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

    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private CapsuleCollider2D capsuleCollider;
    private PlayerInput playerInput;

    // ★ [핵심 변수] 서 있을 때의 원래 콜라이더 크기와 중심을 기억해 둡니다.
    private Vector2 originalSize;
    private Vector2 originalOffset;
    private bool isCurrentlyCrouching = false;

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

        //앉기 키(S)를 누르면 콜라이더를 반토막 내고, 떼면 원래대로 복구합니다!
        if (playerInput.IsCrouch && !isCurrentlyCrouching)
        {
            ApplyCrouchCollider(true);
        }
        else if (!playerInput.IsCrouch && isCurrentlyCrouching)
        {
            ApplyCrouchCollider(false);
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
                Debug.Log("이미 가장 어린 시절(Child)입니다!");
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

        Invoke(nameof(AdjustColliderToSprite), 0.1f);
    }

    //Y축 1.5 오프셋 기준으로 머리가 안 튀어나오게 세팅
    private void AdjustColliderToSprite()
    {
        if (spriteRenderer == null || spriteRenderer.sprite == null || capsuleCollider == null) return;

        float spriteWidth = spriteRenderer.sprite.bounds.size.x;
        float spriteHeight = spriteRenderer.sprite.bounds.size.y;

        //Offset Y를 1.5로 고정하고, 발 아랫부분을 제외한 2.5D 높이 계산
        float fixedOffsetY = 1.5f;
        float targetHeight = Mathf.Max((spriteHeight - fixedOffsetY) * 2f, 0.5f);

        originalSize = new Vector2(spriteWidth, targetHeight);
        originalOffset = new Vector2(0f, fixedOffsetY);

        //웅크리고 있는 상태가 아닐 때만 즉시 적용
        if (!isCurrentlyCrouching)
        {
            capsuleCollider.size = originalSize;
            capsuleCollider.offset = originalOffset;
        }
    }

    //앉기/일어서기에 따른 콜라이더 반토막 조절 함수
    private void ApplyCrouchCollider(bool crouch)
    {
        isCurrentlyCrouching = crouch;

        if (crouch)
        {
            //높이를 딱 반토막(0.5배) 내고, 바닥에서 뜨지 않도록 중심(Offset)도 반으로 낮춥니다!
            capsuleCollider.size = new Vector2(originalSize.x, originalSize.y * 0.5f);
            capsuleCollider.offset = new Vector2(originalOffset.x, originalOffset.y * 0.5f);
            Debug.Log("[콜라이더 반토막] 웅크리기 상태 적용");
        }
        else
        {
            //일어서면 원래 기억해둔 크기와 1.5 오프셋으로 복귀
            capsuleCollider.size = originalSize;
            capsuleCollider.offset = originalOffset;
            Debug.Log("[콜라이더 복구] 서기 상태 적용");
        }
    }
}