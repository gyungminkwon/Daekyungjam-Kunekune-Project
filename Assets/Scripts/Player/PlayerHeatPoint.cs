using UnityEngine;

public class PlayerHeatPoint : MonoBehaviour
{
    [Header("Height Offset Settings")]
    [SerializeField] private float adultOffsetY = 2.5f;

    [SerializeField] private float studentOffsetY = 2.0f;

    [SerializeField] private float childOffsetY = 1.7f;

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

    //나이대별로 동그라미 색상이 다르게 변하는 실시간 기즈모
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(GetDetectionPoint(), 0.15f);
    }
}