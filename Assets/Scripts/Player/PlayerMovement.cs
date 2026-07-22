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

    private Rigidbody2D rb;
    private PlayerInput input;
    private PlayerStamina stamina;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        input = GetComponent<PlayerInput>();
        stamina = GetComponent<PlayerStamina>();
    }

    void Update()
    {
        if (input == null) return;

        // 탈진 타이머 계산(사이즈 축소 제거)
        if (isExhausted)
        {
            exhaustionTimer -= Time.deltaTime;
            if (exhaustionTimer < 0f && !input.IsSprint)
            {
                isExhausted = false;
            }
        }
    }

    void FixedUpdate()
    {
        if (input == null || rb == null) return;

        // 1. 기본 이동 속도 설정 (웅크리기 : 50% 감속)
        float speed = input.IsCrouch ? moveSpeed * 0.5f : moveSpeed;

        // 2. 탈진 상태라면 속도를 추가 감속
        if (isExhausted)
        {
            speed *= exhaustionSpeedMultiplier;
        }

        bool isMoving = Mathf.Abs(input.MoveInput) > 0.01f;

        // 3. 스태미나 및 달리기(Sprint) 적용
        if (stamina != null)
        {
            // 앉아있을 때는 달리기 불가 (!input.IsCrouch)
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

        // 4. 최종 속도 물리 적용
        rb.linearVelocity = new Vector2(speed * input.MoveInput, rb.linearVelocity.y);
    }

    // 애니메이션 컨트롤러에서 호출할 수 있도록 public으로 개방
    public bool IsGrounded()
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

    // Ac_PlayerController에서 탈진 여부를 확인할 수 있도록 열어줌
    public bool IsExhausted => isExhausted;
}