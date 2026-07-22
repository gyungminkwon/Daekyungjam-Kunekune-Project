using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour, IInteractable
{   
    [SerializeField] private Image holdGuage;
    [SerializeField] private float threshold = 3f;  // 획득을 위해 F키를 홀드해야 하는 시간
    [SerializeField] private string itemName;
    [SerializeField] private TextData textData; // 아이템 획득 시 띄울 텍스트
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

        holdTimer = 0f;
        holdGuage.fillAmount = 0f;

        Debug.Log($"{gameObject.name} 아이템 획득");
        TextManager.Instance.PlayText(textData);

        isAcquired = true;
        anim?.SetTrigger("Acquire");
        GameManager.Instance.ChangeStage(Stage.School);
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
