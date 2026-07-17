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

        // 2. 실제 물리 이동 속도 계산 (X축 절댓값)
        float currentPhysicalSpeed = Mathf.Abs(rb.linearVelocity.x);
        bool isMoving = currentPhysicalSpeed > 0.1f;

        // 3. ★ 만들어두신 애니메이터 파라미터로 값 완벽 전달! ★
        animator.SetFloat("isrunning", currentPhysicalSpeed);
        animator.SetBool("isCrouching", playerInput.IsCrouch);

        if (playerMovement != null)
        {
            animator.SetBool("isGrounded", playerMovement.IsGrounded());
        }

        // 4. 이동 속도 비례 애니메이션 재생 속도(speed) 동적 조절
        if (isMoving)
        {
            // 기본 걷기 속도(5)일 때 딱 0.75배속이 되도록 비례식 계산
            float targetAnimSpeed = (currentPhysicalSpeed / baseMoveSpeed) * 0.75f;
            // 비정상적인 속도 방지 (최소 0.3배속 ~ 최대 2.0배속)
            animator.speed = Mathf.Clamp(targetAnimSpeed, 0.3f, 2.0f);
        }
        else
        {
            // 서서 대기(Idle) 또는 앉아서 대기(Crouch_Idle) 중일 때는 숨쉬기 모션 속도 고정
            animator.speed = 0.6f;
        }
    }
}