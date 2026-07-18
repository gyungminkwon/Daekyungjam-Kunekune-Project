using UnityEngine;

public class Ac_PlayerController : MonoBehaviour
{
    [Header("Speed Reference")]
    [Tooltip("PlayerMovement의 moveSpeed 값과 똑같이 맞춰주세요!")]
    [SerializeField] private float baseMoveSpeed = 5f;

    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private PlayerInput playerInput;
    private Rigidbody2D rb;
    private PlayerMovement playerMovement;

    void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerInput = GetComponent<PlayerInput>();
        rb = GetComponent<Rigidbody2D>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    void Update()
    {
        if (playerInput == null || animator == null || rb == null) return;

        // 1. 좌우 입력값에 따른 캐릭터 이미지 반전 (Flip)
        float moveInput = playerInput.MoveInput;
        if (moveInput > 0.01f)
        {
            spriteRenderer.flipX = false; // 오른쪽
        }
        else if (moveInput < -0.01f)
        {
            spriteRenderer.flipX = true;  // 왼쪽
        }

        // 2. 실제 물리 이동 속도 계산
        float currentPhysicalSpeed = Mathf.Abs(rb.linearVelocity.x);

        // ★ [여기가 누락되었던 부분!] 키보드 입력과 물리 속도를 보정하여 animSpeedParameter 변수를 생성합니다.
        float animSpeedParameter = Mathf.Abs(moveInput) > 0.01f ? Mathf.Max(currentPhysicalSpeed, baseMoveSpeed * Mathf.Abs(moveInput)) : 0f;

        bool isMoving = animSpeedParameter > 0.1f;

        // 3. 애니메이터 파라미터 전달
        animator.SetFloat("isrunning", animSpeedParameter);
        animator.SetBool("isCrouching", playerInput.IsCrouch);

        if (playerMovement != null)
        {
            animator.SetBool("isGrounded", playerMovement.IsGrounded());
        }

        // 4. ★ 웅크리기 상태에 따른 이동 속도 비례 애니메이션 재생 ★
        if (isMoving)
        {
            // 웅크리고 있다면 최고 기준 속도를 반토막(0.5f) 내서 계산합니다!
            float maxReferenceSpeed = playerInput.IsCrouch ? baseMoveSpeed * 0.5f : baseMoveSpeed;

            // (현재 속도 / 현재 상태의 최고 속도) * 0.75배속 기준값
            float targetAnimSpeed = (animSpeedParameter / maxReferenceSpeed) * 0.75f;

            animator.speed = Mathf.Clamp(targetAnimSpeed, 0.3f, 2.0f);
        }
        else
        {
            // 가만히 멈춰서 숨 쉴 때 (Idle / Crouch_Idle)
            animator.speed = 0.6f;
        }
    }
}