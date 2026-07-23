using System;
using UnityEngine;

public class BrokenPot : MonoBehaviour, IInteractable
{
    [SerializeField] private TextData textData;
    [SerializeField] private ProgressFlag flag;

    public void OnInteractPressed()
    {
        TextManager.Instance?.PlayText(textData);

        ProgressManager.Instance?.SetFlag(flag, true);
    }

    public void OnInteractHeld() {}
    public void OnInteractReleased() {}

    public string GetInteractPrompt()
    {
        return "깨진 화분 (F)";
    }
}
