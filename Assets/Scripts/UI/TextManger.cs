using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro; // TextMeshPro 사용 필수!

public class TextManager : MonoBehaviour
{
    public static TextManager Instance;

    [Header("1. UI 연결")]
    [SerializeField, Tooltip("대사창 전체 패널 (네모 박스)")]
    private GameObject dialoguePanel;
    [SerializeField, Tooltip("대사가 출력될 TextMeshPro UI")]
    private TMP_Text dialogueText;
    [SerializeField, Tooltip("다음 페이지로 넘어가는 ▶ 화살표 아이콘")]
    private GameObject nextIcon;

    [Header("2. 타이핑 설정")]
    [SerializeField, Tooltip("글자 하나당 출력 속도 (0.05초 추천)")]
    private float typingSpeed = 0.05f;

    // 내부 상태 변수들
    private bool isTyping = false;
    private bool isWaitingForClick = false;
    private string currentFullText = "";

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        if (dialoguePanel != null) dialoguePanel.SetActive(false);
    }

    private void Update()
    {
        if (dialoguePanel != null && dialoguePanel.activeSelf)
        {
            bool leftClicked = (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame);

            if (leftClicked)
            {
                if (isTyping)
                {
                    StopAllCoroutines();
                    dialogueText.text = currentFullText;
                    isTyping = false;
                    isWaitingForClick = true;
                    if (nextIcon != null) nextIcon.SetActive(true);
                }
                else if (isWaitingForClick)
                {
                    CloseDialogue();
                }
            }
        }
    }

    /// <summary>
    /// ★ 대사 호출 메인 함수 (속도뿐만 아니라 글자 색상까지 지정 가능!)
    /// </summary>
    public void ShowText(string text, float customSpeed = -1f, Color? customColor = null)
    {
        StopAllCoroutines();
        // 색상을 지정하지 않았으면(null) 기본 흰색(Color.white)으로 자동 설정!
        StartCoroutine(TypeTextRoutine(text, customSpeed, customColor ?? Color.white));
    }

    private IEnumerator TypeTextRoutine(string text, float customSpeed, Color textColor)
    {
        if (dialoguePanel != null) dialoguePanel.SetActive(true);
        if (nextIcon != null) nextIcon.SetActive(false);

        // ★ UI 텍스트의 색상을 전달받은 색상으로 즉시 변경!
        if (dialogueText != null) dialogueText.color = textColor;

        currentFullText = text;
        dialogueText.text = "";
        isTyping = true;
        isWaitingForClick = false;

        float speed = customSpeed > 0f ? customSpeed : typingSpeed;

        foreach (char c in text.ToCharArray())
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(speed);
        }

        isTyping = false;
        isWaitingForClick = true;
        if (nextIcon != null) nextIcon.SetActive(true);
    }

    public void CloseDialogue()
    {
        isWaitingForClick = false;
        if (dialoguePanel != null) dialoguePanel.SetActive(false);
    }

    public bool IsDialogueActive()
    {
        return dialoguePanel != null && dialoguePanel.activeSelf;
    }

    public bool IsTyping()
    {
        return isTyping;
    }
}