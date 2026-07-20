using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Playables;

public class TextManager : MonoBehaviour
{
    public static TextManager Instance { get; private set; }

    [Header("UI References")]
    public GameObject dialogPanel;
    public TextMeshProUGUI dialogText;
    public Image iconImage;

    public CanvasGroup fadePanel;
    public TextMeshProUGUI fadeText;

    public CanvasGroup systemPanel;
    public TextMeshProUGUI systemText;

    private bool isWaitingForClick = false;

    private PlayerInput playerInput;

    private void Awake()
    {
        if (Instance == null) 
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);

        playerInput = FindFirstObjectByType<PlayerInput>();
    }

    public void PlayText(TextData data, PlayableDirector director = null, float enableDelay = 0f)
    {
        StartCoroutine(TextRoutine(data, director, enableDelay));
    }

    private IEnumerator TextRoutine(TextData data, PlayableDirector director, float enableDelay)
    {
        switch (data.type)
        {
            case TextType.Monologue :
            case TextType.Interaction :
                yield return StartCoroutine(HandleDialog(data, director, enableDelay));
                break;
            case TextType.ScreenFade :
                yield return StartCoroutine(HandleScreenFade(data));
                break;
            case TextType.SystemGuide :
                yield return StartCoroutine(HandleSystemGuide(data));
                break;
        }
    }

    private IEnumerator HandleDialog(TextData data, PlayableDirector director, float enableDelay)
    {
        // 이동 조작 불가 처리
        playerInput.enabled = false;
        dialogPanel.SetActive(true);
        if (director != null) director.Pause();

        if (data.type == TextType.Interaction && data.objectIcon != null)
        {
            iconImage.gameObject.SetActive(true);
            iconImage.sprite = data.objectIcon;
        } 
        else
        {
            iconImage.gameObject.SetActive(false);    
        }

        foreach (string line in data.lines)
        {
            dialogText.text = line;
            dialogText.maxVisibleCharacters = 0;

            for (int i = 0; i <= line.Length; i++)
            {
                dialogText.maxVisibleCharacters = i;
                yield return new WaitForSeconds(0.05f);
            }

            isWaitingForClick = true;
            while (!Input.GetMouseButtonDown(0))
            {
                yield return null;
            }
            isWaitingForClick = false;

            yield return null;
        }

        if (director != null) director.Play();
        dialogPanel.SetActive(false);
        // 플레이어 이동 가능 처리 (약간의 딜레이 주기)
        yield return new WaitForSeconds(enableDelay);
        playerInput.enabled = true;
    }

    private IEnumerator HandleScreenFade(TextData data)
    {
        // 페이드 인 -> 텍스트 출력 -> 대기 -> 페이드 아웃 (세부 코루틴 구현)
        // CanvasGroup.alpha를 Mathf.MoveTowards 등을 이용해 0에서 1로 조절
        yield return null;
    }

    private IEnumerator HandleSystemGuide(TextData data)
    {
        if (data.lines.Length > 0)
        {
            systemText.text = data.lines[0];
        }

        yield return FadeCanvasGroup(systemPanel, 0, 1, 0.5f);

        yield return new WaitForSeconds(data.displayDuration);

        yield return FadeCanvasGroup(systemPanel, 1, 0, 0.5f);
    }

    private IEnumerator FadeCanvasGroup(CanvasGroup cg, float start, float end, float duration)
    {
        float time = 0f;
        while (time < duration)
        {
            cg.alpha = Mathf.Lerp(start, end, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        cg.alpha = end;
    }
}
