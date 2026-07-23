using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class ChaseSceneManager : MonoBehaviour
{
    public static ChaseSceneManager Instance { get; private set; }

    public enum DeathCause { Kunekune, Rope }
    
    [Header("추격전 연출 설정")]
    [Tooltip("추격 전용 쿠네쿠네 할당")]
    public KunekuneChaseAI chaseKunekune;
    
    [Tooltip("허수아비 할당")]
    public KunekuneProp startingProp;

    [Header("게임 오버 설정")]
    [Tooltip("텍스트 UI")]
    public GameObject restartTextUI;
    public float fadeDuration = 0.5f;
    public float soundLength = 2f;

    private bool isGameOver = false;
    private bool canRestart = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    void Start()
    {
        if (restartTextUI != null) restartTextUI.SetActive(false);

        if (UIManager.Instance != null)
        {
            UIManager.Instance.FadeUI(1f, 0f, fadeDuration);
        }

        if (chaseKunekune != null && startingProp != null)
        {
            Debug.Log("추격전 시작");
            chaseKunekune.StartChaseFromProp(startingProp);
        }
        else
        {
            Debug.LogWarning("쿠네쿠네 또는 시작 프롭이 할당되지 않았습니다");
        }
    }

    void Update()
    {
        if (canRestart && Input.GetKeyDown(KeyCode.Space))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        if (HeatManager.Instance != null && chaseKunekune != null)
        {
            bool isHot = HeatManager.Instance.IsPlayerInHeatArea && !HeatManager.Instance.IsPlayerInFreezeArea;
            chaseKunekune.isPlayerInHeat = isHot;
        }
    }

    public void GameOver(DeathCause cause)
    {
        if (isGameOver) return;
        isGameOver = true;

        Debug.Log("게임 오버 원인: " + cause.ToString());

        if (chaseKunekune != null) chaseKunekune.StopChase();
        if (GameManager.Instance != null) GameManager.Instance.currentState = GameState.Cutscene; 

        StartCoroutine(GameOverSequence(cause));
    }

    private IEnumerator GameOverSequence(DeathCause cause)
    {
        if (UIManager.Instance != null)
        {
            UIManager.Instance.FadeUI(0f, 1f, fadeDuration);
        }

        yield return new WaitForSeconds(fadeDuration);

        if (soundLength > fadeDuration)
        {
            yield return new WaitForSeconds(soundLength - fadeDuration);
        }
        else
        {
            yield return new WaitForSeconds(1f);
        }

        if (restartTextUI != null) restartTextUI.SetActive(true);

        canRestart = true;
    }
}