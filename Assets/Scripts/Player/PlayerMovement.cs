using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("Sprint Settings")]
    [SerializeField] private float sprintSpeed = 8f;
    [SerializeField] private float costPerSecond = 30f;

    [Header("Exhaustion")]
    [SerializeField] private float exhaustionDuration = 3.5f;
    [SerializeField] private float exhaustionSpeedMultiplier = 0.3f;
    private bool isExhausted = false; // stamina를 완전히 소모하면 탈진: Shift 키를 뗐다 눌러야 다시 달리기 작동
    private float exhaustionTimer = 0f;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float checkRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Jump Settings")]
    [SerializeField] private float jumpForce = 20f;

    private Rigidbody2D rb;
    private PlayerInput input;
    private PlayerStamina stamina;

    private Animator animator;
    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        input = GetComponent<PlayerInput>();
        stamina = GetComponent<PlayerStamina>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (input == null) return;

        if (input.IsJump && IsGrounded() && !isExhausted)
        {
            Jump();
        }

        if (input.IsCrouch)
        {
            transform.localScale = new Vector3(1f, 0.5f, 1f);
            if (animator != null) animator.SetBool("isCrouching", true);
        }
        else
        {
            transform.localScale = new Vector3(1f, 1f, 1f);
            if (animator != null) animator.SetBool("isCrouching", false);
        }

        if (isExhausted)
        {
            exhaustionTimer -= Time.deltaTime;

            if (exhaustionTimer < 0f && !input.IsSprint)
            {
                isExhausted = false;
            }
        }

        // Float 기반 애니메이션 및 반전 제어 함수 호출
        HandleSpriteFlipAndAnimation();
    }

    void FixedUpdate()
    {
        // 기본 속도 설정 (웅크리기 : 50% 감속)

        float speed = input.IsCrouch ? moveSpeed * 0.5f : moveSpeed;

        // 탈진 상태라면 속도를 추가로 감속
        if (isExhausted)
        {
            speed *= exhaustionSpeedMultiplier;
        }

        bool isMoving = Mathf.Abs(input.MoveInput) > 0.01f;

        if (stamina != null)
        {
            bool canSprint = !input.IsCrouch && input.IsSprint && !isExhausted && isMoving;
            if (canSprint)
            {
                speed = sprintSpeed;
                stamina.ConsumeStamina(costPerSecond * Time.fixedDeltaTime);

                // 달리는 중 스태미나가 바닥나면 탈진 상태 돌입
                if (stamina.CurrentStamina <= 0f)
                {
                    isExhausted = true;
                    exhaustionTimer = exhaustionDuration;   // 탈진 타이머 시작
                }
            }
        }

        rb.linearVelocity = new Vector2(speed * input.MoveInput, rb.linearVelocity.y);
    }

    private void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        if (animator != null) animator.SetTrigger("doJump");
    }

    // ★ 이 부분이 Float(isrunning) 기반으로 완전히 변경되었습니다! ★
    private void HandleSpriteFlipAndAnimation()
    {
        if (spriteRenderer == null || animator == null || rb == null) return;

        float moveValue = input.MoveInput;

        // 1. 캐릭터 이미지 좌우 반전
        if (moveValue > 0.01f)
        {
            spriteRenderer.flipX = false; // 오른쪽
        }
        else if (moveValue < -0.01f)
        {
            spriteRenderer.flipX = true;  // 왼쪽
        }

        // 2. ★ Rigidbody2D에서 '실제 X축 물리 속도'의 절댓값을 가져옵니다! ★
        // (주의: 최신 유니티는 linearVelocity, 구버전 유니티는 velocity를 씁니다)
        float currentPhysicalSpeed = Mathf.Abs(rb.linearVelocity.x);

        // 3. 이동 중인지 판정 (실제 속도가 0.1 이상일 때만 이동으로 인정)
        bool isMoving = currentPhysicalSpeed > 0.1f;

        // 애니메이터의 isrunning 파라미터에는 실제 속도값을 전달
        animator.SetFloat("isrunning", currentPhysicalSpeed);
        animator.SetBool("isGrounded", IsGrounded());

        // 4. ★ 현재 물리 속도를 기반으로 애니메이션 재생 속도를 동적 조절 ★
        if (isMoving)
        {
            // '현재 속도 / 기본 이동 속도' 비율로 애니메이션 배속을 결정합니다.
            // (예: moveSpeed가 5이고 현재 속도가 8이면 1.6배속으로 빨라짐)
            float targetAnimSpeed = (currentPhysicalSpeed / moveSpeed)*0.75f;

            // 너무 비정상적으로 빠르거나 느려지지 않도록 최솟값(0.3배속)~최댓값(2.0배속) 범위를 제한해 줍니다.
            animator.speed = Mathf.Clamp(targetAnimSpeed, 0.3f, 2.0f);
        }
        else
        {
            // 멈춰서 대기(Idle) 중일 때는 정상 속도(1배속) 복구
            animator.speed = 0.6f;
        }
    }

    private bool IsGrounded()
    {
        if (groundCheck == null) return false;
        return Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundLayer);
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, checkRadius);
    }
}