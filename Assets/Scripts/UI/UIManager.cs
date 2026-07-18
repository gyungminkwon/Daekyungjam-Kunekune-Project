using UnityEngine;
using UnityEngine.UI; // TMPro를 쓰신다면 using TMPro; 로 변경해주세요.

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Guage Sliders")]
    [SerializeField] private Slider staminaGuage;
    [SerializeField] private PlayerStamina stamina;
    [SerializeField] private Slider heatGuage;

    [Header("Time & D-Day UI (선택 사항)")]
    [SerializeField] private Text timeText;          // 시간 표시용 텍스트 (예: PM 06:00)
    [SerializeField] private Text dDayText;          // D-Day 표시용 텍스트 (예: D - 3)
    [SerializeField] private GameObject warningPanel;// 시간이 역행합니다 알림 패널
    [SerializeField] private Text warningText;       // 알림 텍스트

    void Awake()
    {
        // 어디서든 UIManager.Instance로 UI를 조작할 수 있게 싱글톤 설정
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Update()
    {
        // 1. 스태미나 게이지 실시간 업데이트
        if (stamina != null && staminaGuage != null)
        {
            staminaGuage.value = stamina.Ratio;
        }

        // 2. 더위 게이지 실시간 업데이트 (Instance != null 안전장치 추가!)
        if (heatGuage != null && HeatManager.Instance != null)
        {
            heatGuage.value = HeatManager.Instance.GetHeatRatio();
        }
    }

    // =========================================================================
    // 외부(TimeReversalManager 등)에서 시간과 경고창을 쉽게 바꿀 수 있는 함수들
    // =========================================================================

    public void UpdateTimeUI(string timeString)
    {
        if (timeText != null) timeText.text = timeString;
    }

    public void UpdateDDayUI(int dDay)
    {
        if (dDayText != null) dDayText.text = $"D - {dDay}";
    }

    public void ShowWarningPanel(string message)
    {
        if (warningPanel != null && warningText != null)
        {
            warningText.text = message;
            warningPanel.SetActive(true);
        }
    }

    public void HideWarningPanel()
    {
        if (warningPanel != null) warningPanel.SetActive(false);
    }
}