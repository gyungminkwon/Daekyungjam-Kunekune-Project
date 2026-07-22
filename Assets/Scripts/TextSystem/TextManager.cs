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
            yield return ShowLine(line);
        }

        if (director != null) director.Play();
        dialogPanel.SetActive(false);
        // 플레이어 이동 가능 처리 (약간의 딜레이 주기)
        yield return new WaitForSeconds(enableDelay);
        if (GameManager.Instance?.currentState != GameState.IntroCutscene)
            playerInput.enabled = true;
    }

    private IEnumerator HandleScreenFade(TextData data)
    {
        // 페이드 인 -> 텍스트 출력 -> 대기 -> 페이드 아웃 (세부 코루틴 구현)
        // CanvasGroup.alpha를 Mathf.MoveTowards 등을 이용해 0에서 1로 조절
        yield return FadeCanvasGroup(fadePanel, 0, 1, 0.3f);

        fadeText.text = data.lines[0];

        yield return new WaitForSeconds(data.displayDuration);

        yield return FadeCanvasGroup(fadePanel, 1, 0, 0.3f); 
    }

    private IEnumerator ShowLine(string line)
    {

        yield return TypeLine(line);

        yield return new WaitForSeconds(0.2f);

        yield return new WaitUntil(() => Input.GetMouseButtonDown(0));

        yield return new WaitUntil(() => Input.GetMouseButtonDown(0));
    }

    private IEnumerator TypeLine(string line)
    {
        dialogText.text = line;
        dialogText.maxVisibleCharacters = 0;

        int visible = 0;
        float timer = 0f;

        while (visible < line.Length)
        {
            if (Input.GetMouseButtonDown(0))
            {
                dialogText.maxVisibleCharacters = line.Length;
                yield break;
            }

            timer += Time.deltaTime;

            if (timer >= 0.05f)
            {
                timer = 0f;
                visible++;
                dialogText.maxVisibleCharacters = visible;
            }

            yield return null;
        }
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
