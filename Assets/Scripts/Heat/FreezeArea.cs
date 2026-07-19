using UnityEngine;

public class FreezeArea : MonoBehaviour
{
    public int freezeIntensity = 5;

    [SerializeField] private float gracePeriod = 0.3f;

    private Collider2D shadowCollider;
    private PlayerHeatPoint playerHeatPoint; // ★ 새로 만든 좌표 관리자 연결

    private bool isPlayerInside = false;
    private float insideTimer = 0f;

    void Awake()
    {
        shadowCollider = GetComponent<Collider2D>();
    }

    void Update()
    {
        if (isPlayerInside && playerHeatPoint != null && shadowCollider != null)
        {
            // ★ 플레이어의 실시간 가슴~몸통 좌표를 가져옵니다!
            Vector2 checkPoint = playerHeatPoint.GetDetectionPoint();

            // 몸통 좌표가 그늘 영역 안에 제대로 들어왔는지 검사
            if (shadowCollider.OverlapPoint(checkPoint))
            {
                insideTimer += Time.deltaTime;
                if (insideTimer >= gracePeriod)
                {
                    if (HeatManager.Instance != null)
                    {
                        HeatManager.Instance.HeatDown(freezeIntensity);
                    }
                }
            }
            else
            {
                // 콜라이더 근처엔 있지만 몸통 기준점이 햇빛 쪽으로 나갔다면 타이머 초기화
                insideTimer = 0f;
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInside = true;
            playerHeatPoint = collision.GetComponent<PlayerHeatPoint>();
            insideTimer = 0f;
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInside = false;
            playerHeatPoint = null;
            insideTimer = 0f;
        }
    }
}