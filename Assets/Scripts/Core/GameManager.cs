using Unity.VisualScripting;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Playables;

public enum GameState
{
    Title,
    IntroCutscene,
    Playing,
    GameOver,
    GameClear
}

public enum Stage
{
    House,
    School,
    RiceField,
    Locked
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Game States")] 
    public GameState currentState = GameState.Title;
    public Stage currentStage = Stage.House;

    [Header("Intro & UI References")]
    public GameObject titleBannerUI;
    public PlayableDirector introDirector;
    public Transform introSpawnPoint;

    [Header("Player References")]
    [SerializeField] private GameObject playerObject;
    [SerializeField] private PlayerInput playerInputScript;

    // [Header("System References")]

    void Awake()
    {
        if (Instance == null) Instance = this;
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
    }

    private void StartIntroCutscene()
    {
        currentState = GameState.IntroCutscene;
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
        currentStage = Stage.House;

        playerInputScript.enabled = true;

        Debug.Log("인트로 종료. Stage-House 시작");
    }

    public void ChangeStage(Stage nextStage)
    {
        currentStage = nextStage;
    }
}
