using UnityEngine;

public class InteractionHoldSound : MonoBehaviour
{
    private bool isPlayerInRange = false;

    // 이 오브젝트 전용 개인 스피커
    private AudioSource mySpeaker;
    private AudioClip findSound;

    private void Awake()
    {
        // 게임 시작 시 자동으로 개인 스피커 부착
        mySpeaker = gameObject.AddComponent<AudioSource>();
        mySpeaker.playOnAwake = false;

        // F키를 꾹 누르는 동안 소리가 반복되길 원한다면 true, 
        // 3초짜리 소리가 한 번만 끝까지 재생되길 원한다면 false로 설정하세요.
        mySpeaker.loop = false;
    }

    private void Start()
    {
        // 사운드 매니저가 있다면 'find' 음원 파일을 미리 빌려옵니다.
        if (SoundManager.Instance != null)
        {
            findSound = SoundManager.Instance.GetSFX("find");
        }
    }

    private void Update()
    {
        // 1. 플레이어가 범위 안에 있고 && F키를 꾹 누르고(GetKey) 있을 때
        if (isPlayerInRange && Input.GetKey(KeyCode.F))
        {
            // 아직 소리가 나지 않고 있다면 재생 시작!
            if (!mySpeaker.isPlaying && findSound != null)
            {
                mySpeaker.clip = findSound;
                mySpeaker.Play();
            }
        }
        // 2. 플레이어가 범위 밖으로 나가거나 || F키에서 손을 뗐을 때
        else
        {
            // 소리가 나고 있던 중이라면 즉시 뚝! 끊기
            if (mySpeaker.isPlaying)
            {
                mySpeaker.Stop();
            }
        }
    }

    // 플레이어가 콜라이더 영역에 들어왔을 때
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInRange = true;
        }
    }

    // 플레이어가 콜라이더 영역에서 나갔을 때
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInRange = false;
        }
    }
}