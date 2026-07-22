using UnityEngine;

public class HeatArea : MonoBehaviour
{
    [Header("Heat delay")]
    [SerializeField] private float delayBeforeHeat = 0.15f;

    private Collider2D areaCollider;
    private PlayerHeatPoint playerHeatPoint; // ★ 새로 만든 좌표 관리자 연결
    private float timer = 0f;
    private bool isPlayerIn = false;
    private bool isRegistered = false;

    void Awake()
    {
        areaCollider = GetComponent<Collider2D>();
    }

    void Update()
    {
        if (isPlayerIn && playerHeatPoint != null && areaCollider != null)
        {
            // ★ 플레이어의 현재 성장 상태/앉기 상태에 맞춘 가슴~몸통 감지 좌표를 가져옵니다!
            Vector2 checkPoint = playerHeatPoint.GetDetectionPoint();

            // 몸통 좌표점이 햇빛 영역 안에 들어왔는지 검사
            if (areaCollider.OverlapPoint(checkPoint))
            {
                timer += Time.deltaTime;

                if (timer >= delayBeforeHeat && !isRegistered)
                {
                    if (HeatManager.Instance != null)
                    {
                        HeatManager.Instance.RegisterHeatArea(this);
                    }

                    // ==================================================
                    // ChaseSceneManager 연계 사항
                    // ChaseScene에서 플레이어가 HeatArea에 있으면 신호를 보냄
                    // ==================================================
                    if (ChaseSceneManager.Instance != null)
                    {
                        ChaseSceneManager.Instance.AddPlayerHeatArea();
                    }

                    isRegistered = true;
                }
            }
            else
            {
                // 영역 안에는 있지만 몸통 좌표가 창문 밖으로 벗어났다면 등록 해제
                ResetState();
            }
        }
    }

    private void ResetState()
    {
        timer = 0f;
        if (isRegistered)
        {
            if (HeatManager.Instance != null)
            {
                HeatManager.Instance.UnregisterHeatArea(this);
            }

            // ==================================================
            // ChaseSceneManager 연계 사항
            // ChaseScene에서 플레이어가 HeatArea를 빠져나가면 신호를 보냄
            // ==================================================
            if (ChaseSceneManager.Instance != null)
            {
                ChaseSceneManager.Instance.RemovePlayerHeatArea();
            }

            isRegistered = false;
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            timer = 0f;
            // 플레이어에게 붙은 좌표 관리 컴포넌트를 가져옵니다.
            playerHeatPoint = collision.GetComponent<PlayerHeatPoint>();
            isPlayerIn = true;
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerIn = false;
            playerHeatPoint = null;
            ResetState(); // 영역을 나가면 안전하게 상태 초기화
        }
    }
}