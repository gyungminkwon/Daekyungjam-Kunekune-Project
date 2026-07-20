using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Playables;

public class IntroCutscene : MonoBehaviour
{
    private PlayableDirector director;
    private bool isCutscenePlayed = false;

    [Header("Cinemachine & Offset Setting")]
    [SerializeField] private CinemachineCamera cineCam;
    [SerializeField] private Vector3 targetCameraOffset;
    void Awake()
    {
        director = GetComponent<PlayableDirector>();
    }

    void Start()
    {
        if (director != null)
            director.Pause(); 

        cineCam.Follow = null;
        SpriteRenderer playerSr = GameObject.FindGameObjectWithTag("Player").GetComponent<SpriteRenderer>();
        Color c = playerSr.color;
        c.a = 0f;
        playerSr.color = c;
    }

    void Update()
    {
        if (!isCutscenePlayed && Input.GetKeyDown(KeyCode.Space))
        {
            isCutscenePlayed = true;
            director.Play();
        }
    }

    public void InitiateCameraSetting()
    {
        Transform player = GameObject.FindGameObjectWithTag("Player").transform;
        cineCam.Follow = player;
        CinemachineFollow follow = cineCam.GetComponent<CinemachineFollow>();
        if (follow != null) follow.FollowOffset = targetCameraOffset;
    }

    public void ShowPlayer(float duration)
    {
        StartCoroutine(PadePlayer(duration));
    }

    private IEnumerator PadePlayer(float duration)
    {
        SpriteRenderer playerSr = GameObject.FindGameObjectWithTag("Player").GetComponent<SpriteRenderer>();
        Color c = playerSr.color;

        float timer = 0f;

        while (timer < duration)
        {
            c = playerSr.color;
            timer += Time.deltaTime;
            c.a = Mathf.Lerp(0, 1, timer / duration);
            playerSr.color = c;

            yield return null;
        }
        c.a = 1;
        playerSr.color = c;
    }
}
