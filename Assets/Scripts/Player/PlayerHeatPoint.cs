using UnityEngine;

public class PlayerHeatPoint : MonoBehaviour
{
    [Header("Height Offset Settings")]
    [Tooltip("성인일 때 감지점 높이")]
    [SerializeField] private float adultOffsetY = 1.8f;

    [Tooltip("학생일 때 감지점 높이")]
    [SerializeField] private float studentOffsetY = 1.8f;

    [Tooltip("어린이일 때 감지점 높이")]
    [SerializeField] private float childOffsetY = 1.8f;

    [Tooltip("웅크렸을 때 높이를 몇 배로 낮출지 (0.5 = 반토막)")]
    [SerializeField] private float crouchMultiplier = 0.5f;

    private PlayerInput playerInput;
    private PlayerGrowthManager growthManager;

    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        growthManager = GetComponent<PlayerGrowthManager>();
    }

    public Vector2 GetDetectionPoint()
    {
        float currentOffsetY = adultOffsetY;

        if (growthManager != null)
        {
            switch (growthManager.currentStage)
            {
                case PlayerGrowthManager.GrowthStage.Adult:
                    currentOffsetY = adultOffsetY;
                    break;
                case PlayerGrowthManager.GrowthStage.Student:
                    currentOffsetY = studentOffsetY;
                    break;
                case PlayerGrowthManager.GrowthStage.Child:
                    currentOffsetY = childOffsetY;
                    break;
            }
        }

        if (playerInput != null && playerInput.IsCrouch)
        {
            currentOffsetY *= crouchMultiplier;
        }

        return new Vector2(transform.position.x, transform.position.y + currentOffsetY);
    }

    // ★ 나이대별로 동그라미 색상이 다르게 변하는 실시간 기즈모!
    private void OnDrawGizmos()
    {
        if (growthManager != null)
        {
            switch (growthManager.currentStage)
            {
                case PlayerGrowthManager.GrowthStage.Adult:
                    Gizmos.color = Color.yellow; // 성인: 노란색
                    break;
                case PlayerGrowthManager.GrowthStage.Student:
                    Gizmos.color = Color.green;  // 학생: 초록색
                    break;
                case PlayerGrowthManager.GrowthStage.Child:
                    Gizmos.color = Color.cyan;   // 어린이: 파란색
                    break;
            }
        }
        else
        {
            Gizmos.color = Color.red; // 연결 에러 시 빨간색으로 경고!
        }

        Gizmos.DrawWireSphere(GetDetectionPoint(), 0.15f);
    }
}