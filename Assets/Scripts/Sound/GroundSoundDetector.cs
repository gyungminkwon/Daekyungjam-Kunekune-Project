using UnityEngine;

public class GroundSoundDetector : MonoBehaviour
{
    [Header("스피커 연결")]
    [SerializeField] private AudioSource footstepSource;
    [SerializeField] private AudioSource bgmSource;

    private LocationAudioZone currentZone;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        LocationAudioZone newZone = collision.GetComponentInParent<LocationAudioZone>();

        if (newZone != null && newZone != currentZone)
        {
            currentZone = newZone;
            ApplyLocationAudio(newZone);
        }
    }

    // ★ [추가] 방(구역)에서 발을 빼고 나가는 순간 감지!
    private void OnTriggerExit2D(Collider2D collision)
    {
        LocationAudioZone exitedZone = collision.GetComponentInParent<LocationAudioZone>();

        // 내가 방금 들어갔던 그 방에서 나오는 게 맞다면
        if (exitedZone != null && exitedZone == currentZone)
        {
            currentZone = null; // 현재 구역 비우기

            // BGM 볼륨을 다시 원래 크기(100% = 1.0f)로 즉시 복구!
            if (bgmSource != null)
            {
                bgmSource.volume = 1f;
            }
        }
    }

    private void ApplyLocationAudio(LocationAudioZone zone)
    {
        // 1. 발소리 교체
        if (footstepSource != null && zone.footstepClip != null)
        {
            bool wasPlaying = footstepSource.isPlaying;
            footstepSource.clip = zone.footstepClip;
            if (wasPlaying) footstepSource.Play();
        }

        // 2. BGM 볼륨 조절 (방에 들어왔으니 0.5f로 줄어듦)
        if (bgmSource != null && !Mathf.Approximately(bgmSource.volume, zone.bgmVolume))
        {
            bgmSource.volume = zone.bgmVolume;
        }

        // 3. 방마다 BGM을 안 바꿀 거라면 이 부분은 실행되지 않고 무시됩니다.
        if (!string.IsNullOrEmpty(zone.bgmName) && SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayBGM(zone.bgmName);
        }
    }
}