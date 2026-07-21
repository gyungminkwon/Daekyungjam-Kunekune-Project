using UnityEngine;

public class ToiletBowlInteractable : MonoBehaviour, IInteractable
{
    public bool isEventBowl = false; 
    private bool isKeyObtained = false;
    public TextData failMonologue;
    public TextData successMonologue;

    public void Interact()
    {
        if (isKeyObtained) return;

        if (isEventBowl)
        {
            GameProgressManager.Instance.hasClassroomKey = true;
            isKeyObtained = true;
            Debug.Log("교실 열쇠 획득");
            if (successMonologue != null) TextManager.Instance.PlayText(successMonologue);
        }
        else
        {
            if (failMonologue != null) TextManager.Instance.PlayText(failMonologue);
            Debug.Log("1~3번째 좌변기");
        }
    }

    public string GetInteractPrompt()
    {
        return isKeyObtained ? "" : "좌변기 (F)";
    }
}