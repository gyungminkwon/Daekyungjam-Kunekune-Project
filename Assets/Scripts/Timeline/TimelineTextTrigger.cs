using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Playables;

public class TimelineTextTrigger : MonoBehaviour
{
    [Header("Timeline Director")]
    public PlayableDirector director;

    void Awake()
    {
        if (director == null)
        {
            director = GetComponent<PlayableDirector>();
        }
    }

    public void TriggerText(TextData textData)
    {
        TextManager.Instance.PlayText(textData, director);
    }
}
