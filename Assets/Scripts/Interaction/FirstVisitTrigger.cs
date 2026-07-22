using UnityEngine;

public class FirstVisitTrigger : MonoBehaviour
{
    [SerializeField] private ProgressFlag flag;
    [SerializeField] private TextData triggerText;

    void Awake()
    {
        if (UIManager.Instance != null)
            UIManager.Instance.OnFadeFinished += ShowText;
    }

    private void ShowText()
    {
        if (ProgressManager.Instance != null && !ProgressManager.Instance.GetFlag(flag))
        {
            TextManager.Instance?.PlayText(triggerText);
            ProgressManager.Instance?.SetFlag(flag, true);
        }
    }
}
