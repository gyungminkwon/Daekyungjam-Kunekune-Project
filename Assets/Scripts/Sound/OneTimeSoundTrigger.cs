using UnityEngine;

public class OneTimeSoundTrigger : MonoBehaviour
{
    [Header("1회용 효과음 설정")]
    [Tooltip("SoundManager에 등록한 소리 이름을 적어주세요.")]
    [SerializeField] private string targetSfxName = "event_sound";

    private bool isPlayerInsideZone = false; // 플레이어 진입 여부
    private bool isSoundConsumed = false;    // 소리 사용 완료 여부

    void Update()
    {
        // 이미 소리를 소모했다면 더 이상 입력을 감지하지 않음
        if (isSoundConsumed) return;

        // 영역 안에 있고 F키를 누르면 단 한 번만 소리 재생
        if (isPlayerInsideZone && Input.GetKeyDown(KeyCode.F))
        {
            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.PlaySFX(targetSfxName);
            }

            isSoundConsumed = true; // 소리 소모 완료 상태로 변경

            // 팁: 더 이상 스크립트가 돌 필요가 없으니 아래 주석을 풀어 컴포넌트를 꺼도 됩니다.
            // this.enabled = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInsideZone = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInsideZone = false;
        }
    }
}