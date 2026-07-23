using UnityEngine;

public class InteractionSoundTrigger : MonoBehaviour
{
    [Header("재생할 효과음 이름")]
    [Tooltip("SoundManager에 등록한 소리 이름을 적어주세요.")]
    [SerializeField] private string soundName = "intration";

    private bool isPlayerInRange = false;

    void Update()
    {
        // 플레이어가 닿아있는 상태에서 F키를 누르는 순간 소리 재생
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.F))
        {
            SoundManager.Instance.PlaySFX(soundName);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInRange = false;
        }
    }
}