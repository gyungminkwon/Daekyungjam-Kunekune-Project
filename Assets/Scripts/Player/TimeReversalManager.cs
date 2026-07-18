using System.Collections;
using UnityEngine;
using UnityEngine.UI; // UI 사용을 위해 필수 (TMPro 사용 시 TMPro로 변경)

public class TimeReversalManager : MonoBehaviour
{
    [Header("Time Reversal Settings")]
    [Tooltip("시간이 흐르는 속도 (1 = 현실 1초당 게임 1분 소요, 60 = 1초당 게임 1시간)")]
    [SerializeField] private float timeScale = 30f;
    [SerializeField] private int startHour = 18; // 시작 시간 (오후 6시)
    [SerializeField] private int targetHour = 6;  // 목표 역행 시간 (오전 6시)

    [Header("D-Day Settings")]
    [SerializeField] private int currentDDay = 3;

    [Header("UI References")]
    [SerializeField] private Text timeText;       // "PM 06:00" 표시용 텍스트
    [SerializeField] private Text dDayText;       // "D - 3" 표시용 텍스트
    [SerializeField] private GameObject warningPanel; // "시간이 역행합니다" 알림 패널
    [SerializeField] private Text warningText;    // 알림 텍스트

    [Header("System References")]
    [SerializeField] private PlayerGrowthManager playerGrowth; // 캐릭터 성장/퇴화 스크립트

    private float currentHourFloat;
    private bool isTimerRunning = true;

    void Start()
    {
        currentHourFloat = startHour;
        UpdateUI();
        if (warningPanel != null) warningPanel.SetActive(false);
    }

    void Update()
    {
        if (!isTimerRunning) return;

        // 시간이 거꾸로 흐름 (-Time.deltaTime)
        currentHourFloat -= (Time.deltaTime * timeScale) / 60f;

        // UI 업데이트
        UpdateUI();

        // 목표 시간(오전 6시 등)에 도달했는지 확인
        if (currentHourFloat <= targetHour)
        {
            TriggerReversalEvent();
        }
    }

    private void UpdateUI()
    {
        int hour = Mathf.FloorToInt(currentHourFloat);
        int minute = Mathf.FloorToInt((currentHourFloat - hour) * 60f);

        // 12시간제 변환 및 AM/PM 표시
        string amPm = hour >= 12 ? "PM" : "AM";
        int displayHour = hour > 12 ? hour - 12 : hour;
        if (displayHour == 0) displayHour = 12;

        if (timeText != null)
            timeText.text = $"{amPm} {displayHour:D2}:{minute:D2}";

        if (dDayText != null)
            dDayText.text = $"D - {currentDDay}";
    }

    // ★ 시간이 도달했을 때 실행되는 핵심 이벤트
    private void TriggerReversalEvent()
    {
        isTimerRunning = false;
        currentDDay--; // D-day 차감
        UpdateUI();

        Debug.Log("⏱️ [시간 역행] 목표 시간에 도달하여 퇴화 시스템을 작동합니다!");

        // 코루틴으로 문구 출력 후 퇴화 시스템 작동
        StartCoroutine(ReversalRoutine());
    }

    private IEnumerator ReversalRoutine()
    {
        // 1. "시간이 역행합니다" 문구 출력
        if (warningPanel != null && warningText != null)
        {
            warningText.text = "시간이 역행합니다\n[ 퇴화 시스템 작동 ]";
            warningPanel.SetActive(true);
        }

        // 2초간 대기하며 유저에게 상황 인지시킴
        yield return new WaitForSeconds(2.0f);

        // 2. 캐릭터 퇴화 작동 (성인 -> 학생 -> 어린이)
        if (playerGrowth != null)
        {
            playerGrowth.RegressToPreviousStage();
        }

        // 알림창 끄기
        if (warningPanel != null) warningPanel.SetActive(false);

        // =====================================================================
        // ★ [잠시 비활성화됨] 장면 넘김 및 공간 변경 + 캐릭터 이미지 교체 로직 ★
        // 나중에 기능이 필요해지면 아래 /* 와 */ 주석을 지우시면 즉시 작동합니다!
        // =====================================================================
        /*
        yield return StartCoroutine(SceneAndSpaceTransitionRoutine());
        */

        // 다음 날 역행을 위해 시간 리셋 후 타이머 재개
        currentHourFloat = startHour;
        isTimerRunning = true;
    }

    /* =====================================================================
     * [주석 처리된 대기 기능] 공간(배경)이 바뀌면서 캐릭터가 변하는 장면 넘김
     * =====================================================================
    private IEnumerator SceneAndSpaceTransitionRoutine()
    {
        Debug.Log("🌌 [장면 넘김 시작] 화면이 어두워지고 공간이 이동합니다...");

        // 1. 화면을 검게 페이드 아웃 (여기에 UI Fade 아웃 로직이나 애니메이션 트리거 연결)
        // fadeImage.color = new Color(0, 0, 0, 1);
        yield return new WaitForSeconds(1.0f);

        // 2. 배경맵(공간) 오브젝트 교체 또는 씬 이동
        // mapStage1.SetActive(false);
        // mapStage2.SetActive(true);
        // transform.position = new Vector3(0, 0, 0); // 캐릭터 시작 위치 이동

        // 3. 캐릭터의 외형 및 애니메이션 오버라이드 컨트롤러 교체
        if (playerGrowth != null)
        {
            // 예: 강제로 특정 단계(학생 등)로 변경하고 싶을 때
            // playerGrowth.SetGrowthStage(PlayerGrowthManager.GrowthStage.Student);
        }

        // 4. 화면을 다시 밝게 페이드 인
        yield return new WaitForSeconds(1.0f);
        Debug.Log("✨ [장면 넘김 완료] 새로운 공간에서 깨어났습니다.");
    }
    */
}