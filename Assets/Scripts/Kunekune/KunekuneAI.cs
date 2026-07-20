using UnityEngine;
using System.Collections;

public class KunekuneAI : MonoBehaviour
{
    [Header("기본 설정")]
    public Transform player;
    public float chaseTime = 10f; // 최대 추격 시간
    public float spawnDelay = 0f; // 등장 후 움직이기 전 대기 시간

    [Header("이동 설정")]
    public float slowSpeed = 3f; // 느린 속도
    public float dashSpeed = 12f; // 대시 속도
    public float slowDuration = 2f; // 느린 시간
    public float dashDuration = 0.5f; // 대시 시간

    [Header("애니메이션 설정")]
    public float walkSpeedMultiplier = 0.75f;
    public float dashSpeedMultiplier = 1.2f;
    private string currentAnimName = "";
    private float currentSpawnDelay = 0f;

    private SpriteRenderer spriteRenderer;
    private Animator anim;
    private bool isChasing = false;
    private float startY;

    private float dashTimer = 0f;
    private GameObject hiddenProp;
    [HideInInspector] public bool isDashing = false;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();

        GameObject foundPlayer = GameObject.FindGameObjectWithTag("Player");
        if (foundPlayer != null)
        {
            player = foundPlayer.transform;
        }
        else
        {
            Debug.LogWarning("Player 태그를 지닌 오브젝트가 없습니다");
        }
    }

    // 쿠네쿠네 이미지가 Bottom 기준으로 설정되어 있어, 실제 좌표 위치보다 이미지가 더 높게 떠 있습니다.
    // 그래서 쿠네쿠네가 소환될 때 땅보다 높은 위치에서 소환되는 경우가 잦습니다.
    // 이를 해결하기 위해 레이 캐스팅으로 바닥과의 거리를 재 수치를 보정합니다.
    // 이때 레이 캐스팅을 사용하기 위해서는 반드시 Ground Layer가 바닥에 있어야 합니다.
    private Vector2 GetGroundPosition(Vector2 targetPos)
    {
        Vector2 rayStartPos = targetPos + Vector2.up * 0.5f;
        RaycastHit2D hit = Physics2D.Raycast(rayStartPos, Vector2.down, 10f, LayerMask.GetMask("Ground"));

        if (hit.collider != null)
        {
            return new Vector2(targetPos.x, hit.point.y);
        }
        return targetPos; 
    }

    // HeatManager.cs와 연계
    public void StartChaseFrom(Vector2 spawnPosition, GameObject prop, string animName = "", float delay = 0f)
    {
        gameObject.SetActive(true);
        hiddenProp = prop;

        currentAnimName = animName;
        currentSpawnDelay = delay > 0f ? delay : spawnDelay;
        
        // Vector2 groundPos = GetGroundPosition(spawnPosition);
        // transform.position = groundPos;
        // startY = transform.position.y;

        transform.position = spawnPosition;
        startY = transform.position.y;
        
        StartCoroutine(ChaseSequenceRoutine());
    }

    IEnumerator ChaseSequenceRoutine()
    {
        // 1: 추격 전
        isChasing = false;
        anim.speed = 1f;

        if (!string.IsNullOrEmpty(currentAnimName))
        {
            anim.Play(currentAnimName);
        }

        yield return new WaitForSeconds(currentSpawnDelay);

        // 2: 추격 시작
        isChasing = true;
        isDashing = false;
        dashTimer = 0f;
        anim.speed = walkSpeedMultiplier;

        anim.Play("kunekune_move");
        yield return new WaitForSeconds(chaseTime);

        // 3: 추격 종료
        isChasing = false;

        if (hiddenProp != null)
        {
            hiddenProp.SetActive(true);
        }

        gameObject.SetActive(false); 
    }

    // Door.cs와 연계
    public void TeleportWithDelay(Vector2 newPosition)
    {
        StartCoroutine(TeleportDelayRoutine(newPosition));
    }

    IEnumerator TeleportDelayRoutine(Vector2 newPosition)
    {
        // 플레이어가 맵을 이동하면 2초간 대기
        yield return new WaitForSeconds(2f);
        Vector2 groundPos = GetGroundPosition(newPosition);

        // 이후 이동
        transform.position = groundPos;
        startY = transform.position.y;
        isDashing = false;
        dashTimer = 0f;
        anim.speed = walkSpeedMultiplier;
    }

    void Update()
    {
        if (player == null) return;

        // 방향 전환
        if (player.position.x < transform.position.x)
        {
            spriteRenderer.flipX = true;
        }
        else
        {
            spriteRenderer.flipX = false;
        }
        
        if (!isChasing) return;

        float direction = Mathf.Sign(player.position.x - transform.position.x);

        dashTimer += Time.deltaTime;
        float currentSpeed = 0f;

        if (!isDashing)
        {
            currentSpeed = slowSpeed;
            if (dashTimer >= slowDuration)
            {
                isDashing = true;
                dashTimer = 0f;
                anim.speed = dashSpeedMultiplier;
            }
        }
        else
        {
            currentSpeed = dashSpeed;
            if (dashTimer >= dashDuration)
            {
                isDashing = false;
                dashTimer = 0f;
                anim.speed = walkSpeedMultiplier;
            }
        }

        float newX = transform.position.x + (direction * currentSpeed * Time.deltaTime);
        transform.position = new Vector2(newX, transform.position.y);
    }
}