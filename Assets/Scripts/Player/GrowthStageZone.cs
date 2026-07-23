using UnityEngine;

public class GrowthStageZone : MonoBehaviour
{
    [Header("1. 처음 방문했을 때 설정")]
    [Tooltip("체크하면 처음 방문 시 플레이어의 '현재 모습'을 강제로 바꾸지 않고 그대로 유지합니다.")]
    public bool keepCurrentOnFirstVisit = false;
    public PlayerGrowthManager.GrowthStage firstVisitStage = PlayerGrowthManager.GrowthStage.Adult;

    [Header("2. 다시 돌아왔을 때 설정")]
    [Tooltip("체크하면 두 번째 방문부터는 플레이어의 '현재 모습'을 바꾸지 않고 그대로 유지합니다. (메인 장소에 체크!)")]
    public bool keepCurrentOnRevisit = true;

    [Tooltip("위 체크가 꺼져있을 때만 적용될 재방문 모습입니다.")]
    public PlayerGrowthManager.GrowthStage revisitStage = PlayerGrowthManager.GrowthStage.Adult;

    private bool hasVisited = false;

    // ★ [핵심] 플레이어의 '현재 모습(currentPlayerStage)'을 전달받아서 어떻게 할지 판단합니다!
    public PlayerGrowthManager.GrowthStage GetTargetStage(PlayerGrowthManager.GrowthStage currentPlayerStage)
    {
        if (hasVisited)
        {
            // 두 번째 방문부터: '현재 모습 유지'가 켜져있으면 지금 모습 그대로, 아니면 설정된 모습으로!
            return keepCurrentOnRevisit ? currentPlayerStage : revisitStage;
        }
        else
        {
            // 처음 방문 시: '현재 모습 유지'가 켜져있으면 지금 모습 그대로, 아니면 설정된 모습으로!
            return keepCurrentOnFirstVisit ? currentPlayerStage : firstVisitStage;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // 구역에서 완전히 빠져나갈 때 방문 도장 쾅!
        if (collision.GetComponent<PlayerGrowthManager>() != null)
        {
            hasVisited = true;
        }
    }
}