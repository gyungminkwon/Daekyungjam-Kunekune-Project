using UnityEngine;
using UnityEngine.UI;

public class KeyItem : MonoBehaviour, IInteractable
{
    [SerializeField] private ProgressFlag flag;
    [SerializeField] private Image holdGuage;
    [SerializeField] private float threshold = 3f;  // 획득을 위해 F키를 홀드해야 하는 시간
    [SerializeField] private string itemName;
    [SerializeField] private TextData textData; // 아이템 획득 시 띄울 텍스트

    [Header("사운드 설정")]
    [Tooltip("아이템 획득 시 재생할 효과음 이름 (예: ui)")]
    [SerializeField] private string acquireSoundName = "ui"; // ★ 유니티에서 바꿀 수 있게 추가!

    private bool isAcquired = false;    // 재획득 방지를 위한 플래그 변수
    private float holdTimer = 0f;
    private Animator anim;

    void Awake()
    {
        anim = GetComponent<Animator>();
        holdGuage.gameObject.SetActive(true);
        holdGuage.fillAmount = 0f;
    }

    public void OnInteractPressed()
    {
        holdTimer = 0f;
        holdGuage.fillAmount = 0f;
    }

    public void OnInteractHeld()
    {
        holdTimer += Time.deltaTime;
        holdGuage.fillAmount = holdTimer / threshold;

        if (holdTimer >= threshold)
        {
            Collect();
        }
    }

    public void Collect()
    {
        if (isAcquired) return;
        isAcquired = true; // ★ [안전장치 추가] 획득 처리 완료 표시

        holdTimer = 0f;
        holdGuage.fillAmount = 0f;

        Debug.Log($"{gameObject.name} 아이템 획득");
        TextManager.Instance.PlayText(textData);

        ProgressManager.Instance.SetFlag(flag, true);

        // ★ [사운드 재생 추가] 아이템 획득(UI 띄움)과 동시에 소리 재생!
        if (SoundManager.Instance != null && !string.IsNullOrEmpty(acquireSoundName))
        {
            SoundManager.Instance.PlaySFX(acquireSoundName);
        }

        anim?.SetTrigger("Acquire");
        gameObject.SetActive(false);
    }

    public void OnInteractReleased()
    {
        holdTimer = 0f;
        holdGuage.fillAmount = 0f;
    }

    public string GetInteractPrompt()
    {
        return "아이템 획득 (F 홀드)";
    }
}