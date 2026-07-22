using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI; // TMPro를 쓰신다면 using TMPro; 로 변경해주세요.

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Guage Sliders")]
    [SerializeField] private Image staminaGuage;
    [SerializeField] private PlayerStamina stamina;
    [SerializeField] private Image heatGuage;
    [SerializeField] private CanvasGroup fadeLayer;

    public event Action OnFadeFinished;

    void Awake()
    {
        // 어디서든 UIManager.Instance로 UI를 조작할 수 있게 싱글톤 설정
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Update()
    {
        if (stamina != null && staminaGuage != null)
        {
            staminaGuage.fillAmount = stamina.Ratio;
        }

        if (heatGuage != null && HeatManager.Instance != null)
        {
            heatGuage.fillAmount = HeatManager.Instance.GetHeatRatio();
        }
    }

    public void FadeSprite(SpriteRenderer target, float start, float end, float duration)
    {
        if (target == null) return;
        StartCoroutine(FadeRoutine(target, start, end, duration));
    }

    public void FadeUI(float start, float end, float duration)
    {
        if (fadeLayer == null) return;
        StartCoroutine(FadeRoutine(fadeLayer, start, end, duration));
    }


    private IEnumerator FadeRoutine(SpriteRenderer sr, float start, float end, float duration)
    {
        float timer = 0f;
        Color c = sr.color;
        
        while (timer < duration)
        {
            timer += Time.deltaTime;
            c = sr.color;
            c.a = Mathf.Lerp(start, end, timer / duration);
            sr.color = c;

            yield return null;
        }

        c.a = end;
        sr.color = c;
    }

    private IEnumerator FadeRoutine(CanvasGroup cg, float start, float end, float duration)
    {
        bool isFadeOut = end < start;
        float timer = 0f;
        
        while (timer < duration)
        {
            cg.alpha = Mathf.Lerp(start, end, timer / duration);
            timer += Time.deltaTime;
            yield return null;
        }

        cg.alpha = end;
        if (isFadeOut)
        {
            OnFadeFinished?.Invoke();
        }
    }
}