using UnityEngine;

public class Ac_PlayerController : MonoBehaviour
{
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private PlayerInput playerInput; // 아까 만든 신규 인풋 시스템 스크립트

    void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerInput = GetComponent<PlayerInput>(); // 컴포넌트 가져오기
    }

    void Update()
    {
        if (playerInput == null) return;

        // 1. 좌우 입력값(-1, 0, 1)을 가져옵니다.
        float moveValue = playerInput.MoveInput;

        // 2. 입력값에 따라 캐릭터 이미지 좌우 반전
        if (moveValue > 0.01f)
        {
            spriteRenderer.flipX = false; // 오른쪽
        }
        else if (moveValue < -0.01f)
        {
            spriteRenderer.flipX = true; // 왼쪽
        }

        // 3. 속도의 절댓값(0 또는 1)을 훌륭하게 만들어두신 'isrunning' Float에 쏙 넣어줍니다!
        // 누르고 있으면 계속 1이 들어가서 달리기가 유지되고, 손을 떼면 0이 되어 멈춥니다.
        animator.SetFloat("isrunning", Mathf.Abs(moveValue));
    }
}