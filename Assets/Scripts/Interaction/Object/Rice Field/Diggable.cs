using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Diggable : MonoBehaviour, IInteractable
{
    [SerializeField] private ProgressFlag flag;
    [SerializeField] private Image holdGuage;
    [SerializeField] private float threshold = 3f;  // 획득을 위해 F키를 홀드해야 하는 시간
    [SerializeField] private string itemName;
    [SerializeField] private TextData textData; // 아이템 획득 시 띄울 텍스트
    [SerializeField] private Date targetDate;
    [SerializeField] private bool isDateChanger = false;

    [SerializeField] private bool hasTicket = false;
    private bool isAcquired = false;    // 재획득 방지를 위한 플래그 변수
    private float holdTimer = 0f;
    private Animator anim;

    void Awake()
    {
        anim = GetComponent<Animator>();
        holdGuage.fillAmount = 0f;
    }

    public void OnInteractPressed()
    {
        if (!ProgressManager.Instance.GetFlag(ProgressFlag.HasTrowel)) return;

        holdTimer = 0f;
        holdGuage.fillAmount = 0f;
        anim.SetBool("isDigging", true);
    }

    public void OnInteractHeld()
    {
        if (!ProgressManager.Instance.GetFlag(ProgressFlag.HasTrowel)) return;

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

        holdTimer = 0f;
        holdGuage.fillAmount = 0f;

        TextManager.Instance.PlayText(textData);

        ProgressManager.Instance.SetFlag(flag, true);

        if (isDateChanger)
        {
            GameManager.Instance.ChangeDate(targetDate);
        }

        anim?.SetTrigger("Acquire");
        gameObject.SetActive(false);
    }

    public void OnInteractReleased()
    {
        if (!ProgressManager.Instance.GetFlag(ProgressFlag.HasTrowel)) return;

        holdTimer = 0f;
        holdGuage.fillAmount = 0f;
        anim.SetBool("isDigging", false);
    }

    public string GetInteractPrompt()
    {
        if (!ProgressManager.Instance.GetFlag(ProgressFlag.HasTrowel)) return null;

        return "땅 파기 (F)";
    }
}
