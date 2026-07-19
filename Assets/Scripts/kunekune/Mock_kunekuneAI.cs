using UnityEngine;

public class Mock_KunekuneAI : MonoBehaviour
{
    [Header("Temp AI Settings")]
    [SerializeField] private float moveSpeed = 3.5f;

    private Transform playerTarget;
    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        // 태그를 이용해 씬에 있는 플레이어(또는 가슴~몸통 감지점)를 자동으로 타겟팅!
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            // PlayerHeatPoint가 있다면 그 가슴~몸통 좌표 오브젝트를 타겟으로 삼습니다!
            PlayerHeatPoint heatPoint = player.GetComponent<PlayerHeatPoint>();
            playerTarget = (heatPoint != null) ? heatPoint.transform : player.transform;
        }
    }

    void FixedUpdate()
    {
        if (playerTarget == null || rb == null) return;

        // 플레이어 쪽으로 방향을 잡고 다가감 (임시 추적 로직)
        Vector2 direction = ((Vector2)playerTarget.position - rb.position).normalized;
        rb.MovePosition(rb.position + direction * moveSpeed * Time.fixedDeltaTime);

        // 플레이어 보는 방향으로 좌우 이미지 반전
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.flipX = (direction.x < 0);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("[임시 AI] 쿠네쿠네가 플레이어에게 닿았습니다! (공포 이벤트나 게임 오버 띄울 위치)");
        }
    }
}