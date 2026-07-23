using UnityEngine;

public class KunekuneEvent : MonoBehaviour
{
    // 쿠네쿠네 전용 개인 스피커
    private AudioSource mySpeaker;

    private void Awake()
    {
        // 게임이 시작될 때 쿠네쿠네 몸에 알아서 스피커를 하나 부착합니다! (인스펙터 설정 필요 없음)
        mySpeaker = gameObject.AddComponent<AudioSource>();
        mySpeaker.playOnAwake = false; // 시작하자마자 지멋대로 재생되는 것 방지
    }

    private void OnEnable()
    {
        if (SoundManager.Instance != null && mySpeaker != null)
        {
            // 1. 사운드 매니저한테서 음원 파일 두 개를 빌려옵니다.
            AudioClip sound1 = SoundManager.Instance.GetSFX("Sci-Fi Sound");
            AudioClip sound2 = SoundManager.Instance.GetSFX("scary_music");

            // 2. 사운드 매니저 스피커가 아니라, '쿠네쿠네 개인 스피커'로 재생합니다!
            if (sound1 != null) mySpeaker.PlayOneShot(sound1);
            if (sound2 != null) mySpeaker.PlayOneShot(sound2);
        }
    }

    private void OnDisable()
    {
        // 쿠네쿠네가 꺼지거나 파괴될 때 '내 스피커'만 전원을 끕니다.
        // (사운드 매니저 공용 스피커를 건드리지 않으므로 발소리 등이 안 끊김!)
        if (mySpeaker != null)
        {
            mySpeaker.Stop();
        }
    }
}