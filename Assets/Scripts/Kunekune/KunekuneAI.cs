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

    private SpriteRenderer spriteRenderer;
    private bool isChasing = false;
    private float startY;

    private float dashTimer = 0f;
    private GameObject hiddenProp;
    [HideInInspector] public bool isDashing = false;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // HeatManager.cs와 연계
    public void StartChaseFrom(Vector2 spawnPosition, GameObject prop)
    {
        gameObject.SetActive(true);
        hiddenProp = prop;

        transform.position = spawnPosition;
        startY = transform.position.y;
        
        StartCoroutine(ChaseSequenceRoutine());
    }

    IEnumerator ChaseSequenceRoutine()
    {
        // 1: 추격 전
        isChasing = false;
        yield return new WaitForSeconds(spawnDelay);

        // 2: 추격 시작
        isChasing = true;
        isDashing = false;
        dashTimer = 0f;
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

        // 이후 이동
        transform.position = newPosition;
        startY = newPosition.y;
        isDashing = false;
        dashTimer = 0f;
    }

    void Update()
    {
        if (!isChasing || player == null) return;

        // 방향 전환
        if (player.position.x < transform.position.x)
        {
            spriteRenderer.flipX = true;
        }
        else
        {
            spriteRenderer.flipX = false;
        }

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
            }
        }
        else
        {
            currentSpeed = dashSpeed;
            if (dashTimer >= dashDuration)
            {
                isDashing = false;
                dashTimer = 0f; 
            }
        }

        float newX = transform.position.x + (direction * currentSpeed * Time.deltaTime);
        transform.position = new Vector2(newX, transform.position.y);
    }
}