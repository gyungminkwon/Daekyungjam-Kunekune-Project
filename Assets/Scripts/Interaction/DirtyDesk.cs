using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class DirtyDeskHorrorEvent : MonoBehaviour
{
    [Header("0. 힌트 UI 연동")]
    [SerializeField, Tooltip("책상 위에 배치한 FloatingHint 스크립트 연결")]
    private FloatingHint floatingHint;

    [Header("1. 플레이어 제어 (조작 차단 및 해제용)")]
    [SerializeField, Tooltip("플레이어의 Transform (달려올 위치 추적용)")]
    private Transform player;
    [SerializeField, Tooltip("플레이어 이동 스크립트 (연출 중 멈춤 -> 끝나면 다시 켬)")]
    private MonoBehaviour playerMoveScript;

    [Header("2. 환경 오브젝트 (문 & 여학생 SpriteRenderer 제어)")]
    [SerializeField, Tooltip("닫힌 교실 문의 SpriteRenderer (처음엔 켜짐 -> 열릴 때 꺼짐)")]
    private SpriteRenderer closedDoorRenderer;
    [SerializeField, Tooltip("열린 교실 문의 SpriteRenderer (처음엔 꺼짐 -> 열릴 때 켜짐)")]
    private SpriteRenderer openedDoorRenderer;
    [SerializeField, Tooltip("문에 등장할 여학생의 SpriteRenderer (처음엔 꺼짐 -> 문 열릴 때 켜짐)")]
    private SpriteRenderer studentSpriteRenderer;
    [SerializeField, Tooltip("여학생의 최상위 Transform (이동 제어용)")]
    private Transform studentTransform;

    [Header("3. 쿠네쿠네 애니메이션")]
    [SerializeField, Tooltip("여학생의 Animator 컴포넌트")]
    private Animator studentAnimator;
    [SerializeField, Tooltip("변신 애니메이션이 재생되는 총 시간(초)")]
    private float transformAnimDuration = 1.5f;

    [Header("4. 이동 및 돌진 속도")]
    [SerializeField, Tooltip("여학생이 플레이어 Y좌표로 천천히 내려오는 걷기 속도")]
    private float walkSpeed = 2.0f;
    [SerializeField, Tooltip("쿠네쿠네가 달려오는 엄청난 돌진 속도")]
    private float rushSpeed = 35f;

    [Header("5. 카메라 연출")]
    [SerializeField, Tooltip("아까 복제해서 만든 여학생 전용 시네머신 카메라 연결")]
    private GameObject studentCamera; // ★ 주석 해제 완료!

    private bool isPlayerInRange = false;
    private bool isEventTriggered = false;
    private Collider2D deskCollider;

    private static readonly int TransformHash = Animator.StringToHash("Transform");

    private void Awake()
    {
        deskCollider = GetComponent<Collider2D>();

        if (closedDoorRenderer != null) closedDoorRenderer.enabled = true;
        if (openedDoorRenderer != null) openedDoorRenderer.enabled = false;
        if (studentSpriteRenderer != null) studentSpriteRenderer.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isEventTriggered && other.CompareTag("Player"))
        {
            isPlayerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
        }
    }

    private void Update()
    {
        if (!isEventTriggered && isPlayerInRange)
        {
            if (Keyboard.current != null && Keyboard.current.fKey.wasPressedThisFrame)
            {
                StartCoroutine(PlayHorrorSequenceRoutine());
            }
        }
    }

    private IEnumerator PlayHorrorSequenceRoutine()
    {
        isEventTriggered = true;
        isPlayerInRange = false;

        if (floatingHint != null) floatingHint.DisablePermanently();
        if (deskCollider != null) deskCollider.enabled = false;

        // [플레이어 완벽 정지 및 Idle 자세 고정]
        PlayerInput input = player.GetComponent<PlayerInput>();
        PlayerMovement movement = player.GetComponent<PlayerMovement>();
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        Animator anim = player.GetComponent<Animator>();

        if (input != null) input.enabled = false;
        if (movement != null) movement.enabled = false;
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
        }
        if (anim != null)
        {
            anim.SetFloat("isrunning", 0f);
            anim.SetBool("isCrouching", false);
            anim.speed = 0.6f;
        }

        yield return new WaitForSeconds(0.5f);

        // [STEP 1] 여학생 등장
        if (closedDoorRenderer != null) closedDoorRenderer.enabled = false;
        if (openedDoorRenderer != null) openedDoorRenderer.enabled = true;
        if (studentSpriteRenderer != null) studentSpriteRenderer.enabled = true;

        // =============================================================
        // ★ [카메라 이동 1] 여학생 쪽으로 카메라 부드럽게 이동!
        // =============================================================
        Debug.Log("▶ [카메라] 여학생 카메라 ON!");
        if (studentCamera != null) studentCamera.SetActive(true);

        // 시네머신이 여학생에게 스르륵 이동할 시간 2초 동안 대기
        yield return new WaitForSeconds(2.0f);

        // [STEP 1.5] Y좌표 일치 이동
        Debug.Log("여학생: 플레이어 Y좌표로 이동 시작!");
        while (player != null && studentTransform != null)
        {
            float targetY = player.position.y;
            float currentY = studentTransform.position.y;

            if (Mathf.Abs(currentY - targetY) <= 0.05f)
            {
                studentTransform.position = new Vector3(studentTransform.position.x, targetY, studentTransform.position.z);
                break;
            }

            float newY = Mathf.MoveTowards(currentY, targetY, walkSpeed * Time.deltaTime);
            studentTransform.position = new Vector3(studentTransform.position.x, newY, studentTransform.position.z);

            yield return null;
        }
        Debug.Log("Y좌표 일치 완료!");

        yield return new WaitForSeconds(0.3f);

        // =============================================================
        // [STEP 2] 자동 대사 진행
        // =============================================================
        TextManager.Instance.ShowText("내 체육복이잖아... 소름돋아.");
        yield return new WaitForSeconds(0.1f);
        yield return new WaitUntil(() => !TextManager.Instance.IsTyping());
        yield return new WaitForSeconds(1.0f);

        TextManager.Instance.ShowText("소름돋아... 소름돋아...!");
        yield return new WaitForSeconds(0.1f);
        yield return new WaitUntil(() => !TextManager.Instance.IsTyping());
        yield return new WaitForSeconds(1.0f);

        // =============================================================
        // [STEP 3] 쿠네쿠네 변신 애니메이션
        // =============================================================
        if (studentAnimator != null)
        {
            studentAnimator.SetTrigger(TransformHash);
        }

        TextManager.Instance.ShowText("소름돋아...소름돋아...소름돋아...소름돋아...소름돋아...소름돋아...소름돋아...소름돋아...소름돋아...!!!!!!!", 0.02f, new Color32(180, 0, 0, 255));

        yield return new WaitForSeconds(transformAnimDuration);
        TextManager.Instance.CloseDialogue();

        yield return new WaitForSeconds(0.2f);

        // [STEP 4] 초고속 돌진 (갑툭튀)
        Debug.Log("초고속 돌진 시작!");
        while (player != null && studentTransform != null)
        {
            if (Vector2.Distance(studentTransform.position, player.position) <= 0.3f)
            {
                Debug.Log("플레이어에게 도달하여 돌진 루프 종료!");
                break;
            }

            studentTransform.position = Vector2.MoveTowards(
                studentTransform.position,
                player.position,
                rushSpeed * Time.deltaTime
            );

            yield return null;
        }

        // [STEP 5] 덮침 및 종료
        Debug.Log("갑툭튀 덮침! 여학생 스프라이트 비활성화.");
        if (studentSpriteRenderer != null) studentSpriteRenderer.enabled = false;

        yield return new WaitForSeconds(0.5f);

        // =============================================================
        // ★ [카메라 이동 2] 연출 끝! 다시 주인공 카메라로 복귀!
        // =============================================================
        Debug.Log("▶ [카메라] 여학생 카메라 OFF (주인공 카메라로 복귀)!");
        if (studentCamera != null) studentCamera.SetActive(false);

        // 주인공에게 카메라가 돌아올 동안 1.5초 대기
        yield return new WaitForSeconds(1.5f);

        // =============================================================
        // [STEP 6] 플레이어 조작 및 애니메이션 복구
        // =============================================================
        input = player.GetComponent<PlayerInput>();
        movement = player.GetComponent<PlayerMovement>();
        rb = player.GetComponent<Rigidbody2D>();
        anim = player.GetComponent<Animator>();

        if (rb != null)
        {
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        }
        if (anim != null)
        {
            anim.speed = 1.0f; // 애니메이션 속도 100% 정상화
        }
        if (movement != null) movement.enabled = true;
        if (input != null) input.enabled = true;

        Debug.Log("플레이어 조작 해제 완공.");
    }
}