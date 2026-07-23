using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MainGameOverManager : MonoBehaviour
{
    public static MainGameOverManager Instance { get; private set; }

    [Header("점프 스케어 설정")]
    [Tooltip("UI Canvas 하위에 있는 갑툭튀 이미지 객체")]
    public GameObject jumpscareUI; 
    public float jumpscareDuration = 1.2f; // 이미지가 화면에 머무는 시간

    [Header("사운드 설정 (선택)")]
    public AudioSource audioSource;
    public AudioClip jumpscareSound;

    [Header("페이드 설정")]
    public float fadeDuration = 0.5f;

    private bool isGameOver = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    void Start()
    {
        // 시작 시 갑툭튀 이미지는 무조건 꺼둠
        if (jumpscareUI != null) jumpscareUI.SetActive(false);

        // 씬 시작 시 페이드 인 (검은 화면 -> 밝아짐)
        if (UIManager.Instance != null)
        {
            UIManager.Instance.FadeUI(1f, 0f, fadeDuration);
        }
    }

    public void TriggerJumpscare()
    {
        if (isGameOver) return; // 중복 실행 방지
        isGameOver = true;

        // 플레이어 조작 막기
        if (GameManager.Instance != null) GameManager.Instance.currentState = GameState.Cutscene;

        StartCoroutine(JumpscareSequence());
    }

    private IEnumerator JumpscareSequence()
    {
        // 1. 갑툭튀 이미지 켜기 및 사운드 재생
        if (jumpscareUI != null) jumpscareUI.SetActive(true);
        if (audioSource != null && jumpscareSound != null)
        {
            audioSource.PlayOneShot(jumpscareSound);
        }

        // 2. 이미지가 떠 있는 시간만큼 대기
        yield return new WaitForSeconds(jumpscareDuration);

        // 3. 화면 페이드 아웃 (검게 변함)
        if (UIManager.Instance != null)
        {
            UIManager.Instance.FadeUI(0f, 1f, fadeDuration);
        }

        // 페이드 아웃이 끝날 때까지 대기
        yield return new WaitForSeconds(fadeDuration);

        // 4. 씬 재시작 (현재 활성화된 메인 씬을 다시 로드)
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}