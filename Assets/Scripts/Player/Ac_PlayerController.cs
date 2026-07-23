using UnityEngine;

public class Ac_PlayerController : MonoBehaviour
{
    [Header("Speed Reference")]
    [Tooltip("PlayerMovement의 moveSpeed 값과 똑같이 맞춰주세요!")]
    [SerializeField] private float baseMoveSpeed = 5f;

    [Header("발소리 사운드 설정")]
    [SerializeField] private AudioSource footstepSource;
    [Tooltip("기본 발소리 배속 (이 값을 바꾸면 달리기/웅크리기/탈진 소리도 전부 비례해서 변합니다)")]
    [SerializeField] private float basePitch = 1.0f;
    [SerializeField] private float sprintPitchMultiplier = 1.5f;    // 달리기 시 basePitch의 몇 배?
    [SerializeField] private float crouchPitchMultiplier = 0.7f;    // 웅크리기 시 basePitch의 몇 배?
    [SerializeField] private float exhaustedPitchMultiplier = 0.6f; // 탈진 시 basePitch의 몇 배?

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
        float animSpeedParameter = Mathf.Abs(moveInput) > 0.01f ? Mathf.Max(currentPhysicalSpeed, baseMoveSpeed * Mathf.Abs(moveInput)) : 0f;

        bool isMoving = animSpeedParameter > 0.1f;
        bool isGrounded = (playerMovement != null) && playerMovement.IsGrounded();
        // bool isExhausted = (playerMovement != null) && playerMovement.IsExhausted;

        // 3. 애니메이터 파라미터 전달
        animator.SetFloat("isrunning", animSpeedParameter);
        animator.SetBool("isCrouching", playerInput.IsCrouch);
        animator.SetBool("isGrounded", isGrounded);

        // 4. ★ 애니메이션 재생 속도 비례 계산 ★
        if (isMoving)
        {
            // 웅크리고 있다면 최고 기준 속도를 반토막(0.5f) 내서 계산
            float maxReferenceSpeed = playerInput.IsCrouch ? baseMoveSpeed * 0.5f : baseMoveSpeed;

            // (현재 속도 / 최고 기준 속도) * 0.75배속 기준값
            float targetAnimSpeed = (animSpeedParameter / maxReferenceSpeed) * 0.75f;

            // ★ [탈진 비례 감속] 탈진 상태일 때는 실제 물리 속도가 느려진 비율에 맞춰 애니메이션도 비례해서 느리게 재생
            // if (isExhausted)
            // {
            //     targetAnimSpeed *= exhaustedPitchMultiplier;
            // }

            animator.speed = Mathf.Clamp(targetAnimSpeed, 0.3f, 2.0f);
        }
        else
        {
            // 가만히 멈춰서 숨 쉴 때 (다른 모션이 느려지지 않도록 1.0배속 또는 0.6배속 유지)
            animator.speed = 1.0f;
        }

        // 5. ★ 발소리 재생 및 비례 피치(Pitch) 제어 로직 ★
        if (footstepSource != null)
        {
            if (isMoving && isGrounded)
            {
                // basePitch(기본값)를 기준으로 곱하기 연산을 하여 비례 적용!
                float targetPitch = basePitch;

                if (playerInput.IsCrouch)
                {
                    targetPitch = basePitch * crouchPitchMultiplier;     // 예: 1.0 * 0.7 = 0.7배속
                }
                // else if (isExhausted)
                // {
                //     targetPitch = basePitch * exhaustedPitchMultiplier;  // 예: 1.0 * 0.6 = 0.6배속
                // }
                else if (playerInput.IsSprint)
                {
                    targetPitch = basePitch * sprintPitchMultiplier;     // 예: 1.0 * 1.5 = 1.5배속
                }

                // 매 프레임 덮어쓰기 방지 최적화
                if (!Mathf.Approximately(footstepSource.pitch, targetPitch))
                {
                    footstepSource.pitch = targetPitch;
                }

                if (!footstepSource.isPlaying)
                {
                    footstepSource.Play();
                }
            }
            else
            {
                if (footstepSource.isPlaying)
                {
                    footstepSource.Stop();
                }
            }
        }
    }
}