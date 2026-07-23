using System.Collections;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;

public class ClassroomCutscene : MonoBehaviour
{
    [SerializeField] private CinemachineCamera originalCam;
    [SerializeField] private CinemachineCamera cine;
    [SerializeField] private CinemachineImpulseSource impulse;
    [SerializeField] private CanvasGroup cg;
    [SerializeField] private CanvasGroup perform2;
    [SerializeField] private TMP_Text textUI;
    [SerializeField] private TMP_Text[] texts;

    void Awake()
    {
        if (texts.Length <= 0) return;

        foreach (TMP_Text text in texts)
        {
            text.text = "";
        }
        if (perform2) perform2.gameObject.SetActive(false);
    }
    public void PerformText(string text)
    {
        cg.alpha = 0f;
        textUI.text = text;

        StartCoroutine(FadeRoutine(0f, 10f, 0.5f));
    }

    private IEnumerator FadeRoutine(float start, float end, float duration)
    {
        float timer = 0f;

        while (timer < 0.5f)
        {
            timer += Time.deltaTime;

            cg.alpha = Mathf.Lerp(start, end, timer / 0.5f);

            yield return null;
        }
        cg.alpha = end;

        yield return new WaitForSeconds(duration);

        timer = 0f;

        while (timer < 0.5f)
        {
            timer += Time.deltaTime;
            cg.alpha = Mathf.Lerp(end, start, timer / 0.5f);

            yield return null;
        }
        cg.alpha = start;
    }

    public void Perform2()
    {
        StartCoroutine(Routine2(texts));
    }

    private IEnumerator Routine2(TMP_Text[] texts)
    {
        if (perform2) perform2.gameObject.SetActive(true);

        foreach (TMP_Text text in texts)
        {
            text.text = "역겨워";

            yield return new WaitForSeconds(0.1f);
        }

        yield return new WaitForSeconds(1f);

        if (perform2) perform2.gameObject.SetActive(false);
    }

    public void InitiateCameraSetting()
    {
        cine.Priority = 15;
    }

    public void Impulse(float intensity)
    {
        impulse.GenerateImpulse(intensity);
    }

    public void OnFinished()
    {
        originalCam.Priority = 10;
        cine.Priority = 0;
    }
}
