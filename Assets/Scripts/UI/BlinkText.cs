using UnityEngine;
using System.Collections;
using TMPro;
using NUnit.Framework;

public class BlinkText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    private bool isBlinking = false;
    void Update()
    {
        if (isBlinking) return;

        Blink();
    }

    private void Blink()
    {
        StartCoroutine(Fade(0, 1, 0.4f));
    }

    private IEnumerator Fade(float start, float end, float duration)
    {
        isBlinking = true;
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            text.alpha = Mathf.Lerp(start, end, timer / duration);
            yield return null;
        }

        text.alpha = end;

        yield return new WaitForSeconds(0.4f);

        timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            text.alpha = Mathf.Lerp(end, start, timer / duration);
            yield return null;
        }
        text.alpha = start;

        isBlinking = false;
    }
}
