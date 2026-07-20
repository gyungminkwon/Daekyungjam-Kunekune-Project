using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Playables;

public class IntroCutscene : MonoBehaviour
{
    private PlayableDirector director;
    public CinemachineCamera cineCam;
    private bool isCutscenePlayed = false;
    void Awake()
    {
        director = GetComponent<PlayableDirector>();
    }

    void Start()
    {
        if (director != null)
            director.Pause(); 

        cineCam.Follow = null;
    }

    void Update()
    {
        if (!isCutscenePlayed && Input.GetKeyDown(KeyCode.Space))
        {
            isCutscenePlayed = true;
            director.Play();
        }
    }
}
