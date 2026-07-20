using System.Collections;
using UnityEngine;
using UnityEngine.Playables;

public class GameManager : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(Intro());
    }

    void Update()
    {
        
    }

    private IEnumerator Intro()
    {
        PlayableDirector director = FindFirstObjectByType<PlayableDirector>();

        while (true)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                director.Play();
                break;
            }

            yield return null;
        }
    }

    private void Run()
    {
        
    }
}
