using UnityEngine;
using UnityEngine.Playables;

public enum GameState
{
    Title,
    Cutscene,
    Playing,
    Interact,
    GameOver,
    GameClear
}

public enum Date
{
    Day1,
    Day2,
    Day3,
    Locked
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Game States")] 
    public GameState currentState = GameState.Title;
    public Date currentDate = Date.Day1;

    [Header("Intro & UI References")]
    public GameObject titleBannerUI;
    public PlayableDirector introDirector;
    public Transform introSpawnPoint;

    [Header("Player References")]
    [SerializeField] private GameObject playerObject;
    [SerializeField] private PlayerInput playerInputScript;
    [SerializeField] private CanvasGroup cg;

    void Awake()
    {
        if (Instance == null) 
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    void Start()
    {
        SetupTitleScreen();
    }

    void Update()
    {
        if (currentState == GameState.Title && Input.GetKeyDown(KeyCode.Space))
        {
            StartIntroCutscene();
        }
    }

    private void SetupTitleScreen()
    {
        currentState = GameState.Title;

        titleBannerUI.SetActive(true);
        playerInputScript.enabled = false;

        playerObject.transform.position = introSpawnPoint.position;
        SpriteRenderer playerSr = playerObject.GetComponent<SpriteRenderer>();
        Color c = playerSr.color;
        c.a = 0f;
        playerSr.color = c;

        playerObject.GetComponent<PlayerInput>().enabled = false;
        
        if (cg != null) cg.alpha = 0f;
    }

    private void StartIntroCutscene()
    {
        currentState = GameState.Cutscene;
        introDirector.stopped += OnIntroTimelineEnded;

        introDirector.Play();
    }

    private void OnIntroTimelineEnded(PlayableDirector director)
    {
        introDirector.stopped -= OnIntroTimelineEnded;

        StartGameplay();
    }

    private void StartGameplay()
    {
        currentState = GameState.Playing;
        currentDate = Date.Day1;

        if (playerInputScript != null) playerInputScript.enabled = true;
        if (cg != null) cg.alpha = 1f;

        Debug.Log("인트로 종료");
    }

    public void ChangeDate(Date nextDate)
    {
        currentDate = nextDate;
        Debug.Log($"현재 : {currentDate}");
    }

    public void ChangeState(GameState newState)
    {
        currentState = newState;
        Debug.Log($"현재 상태 : {currentState}");
    }
}
