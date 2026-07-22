using UnityEngine;

public class TitleSoundTrigger : MonoBehaviour
{
    void Update()
    {
        // GameManager의 현재 상태가 Title일 때만 스페이스바를 감지합니다.
        if (GameManager.Instance.currentState == GameState.Title)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SoundManager.Instance.PlaySFX("bus");
            }
        }
    }
}