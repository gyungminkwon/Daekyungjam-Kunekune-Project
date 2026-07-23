using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Playables;

public class IntroCutscene : MonoBehaviour
{
    [Header("Cinemachine & Offset Setting")]
    [SerializeField] private CinemachineCamera curCam;
    [SerializeField] private CinemachineCamera nextCam;
    [SerializeField] private CanvasGroup ui;

    void Start()
    {
        curCam.Priority = 10;
        nextCam.Priority = 0;
    }

    public void SetNextCamera()
    {
        nextCam.Priority = 10;
        curCam.Priority = 0;
    }

    public void PlayerPadeIn(float duration)
    {
        StartCoroutine(PadePlayer(duration));
    }

    private IEnumerator PadePlayer(float duration)
    {
        SpriteRenderer playerSr = GameObject.FindGameObjectWithTag("Player").GetComponent<SpriteRenderer>();
        if (playerSr == null) yield break;;

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

    public void FadeUI(float duration)
    {
        if (ui != null) StartCoroutine(FadeUIRoutine(duration));
    }

    private IEnumerator FadeUIRoutine(float duration)
    {
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            ui.alpha = Mathf.Lerp(0, 1, timer / duration);

            yield return null;
        }

        ui.alpha = 1;
    }
}
