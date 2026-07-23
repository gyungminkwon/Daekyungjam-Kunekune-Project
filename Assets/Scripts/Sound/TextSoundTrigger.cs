using UnityEngine;

public class TextSoundTrigger : MonoBehaviour
{
    [Header("대화창 연결")]
    [Tooltip("실제 대화 텍스트가 나오는 UI 패널을 연결하세요. 이 창이 켜져 있을 때만 소리가 납니다.")]
    [SerializeField] private GameObject dialoguePanel;

    [Header("사운드 쿨타임 설정")]
    [SerializeField] private float soundCooldown = 0.1f;

    private float lastSoundTime = 0f;

    void Update()
    {
        // 1. 대화창이 연결되어 있지 않거나, 화면에 꺼져(비활성화) 있다면 소리 재생 코드를 실행하지 않고 바로 종료!
        if (dialoguePanel == null || !dialoguePanel.activeInHierarchy)
        {
            return;
        }

        // 2. 대화창이 켜져 있을 때만 스페이스바 또는 좌클릭 감지
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            if (Time.time >= lastSoundTime + soundCooldown)
            {
                SoundManager.Instance.PlaySFX("ui");
                lastSoundTime = Time.time;
            }
        }
    }
}