using UnityEngine;
using UnityEngine.Playables;

public class DirtyDesk : MonoBehaviour, IInteractable
{
    [SerializeField] private PlayableDirector cutsceneDirector;
    public void OnInteractPressed()
    {
        if (cutsceneDirector != null && !ProgressManager.Instance.GetFlag(ProgressFlag.DirtyDesk))
        {
            cutsceneDirector.Play();
            ProgressManager.Instance.SetFlag(ProgressFlag.DirtyDesk, true);
        }
    }

    public void OnInteractHeld() {}
    public void OnInteractReleased() {}

    public string GetInteractPrompt()
    {
        return "더러운 책상 (F)";
    }
}
