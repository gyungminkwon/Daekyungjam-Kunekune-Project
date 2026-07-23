using UnityEngine;

public class LocationAudioZone : MonoBehaviour
{
    [Header("이 장소의 사운드 설정")]
    [Tooltip("이 구역에서 걸을 때 날 발소리 파일")]
    public AudioClip footstepClip;

    [Tooltip("이 구역에 들어왔을 때 바꿀 BGM 이름 (안 바꿀 거면 비워두세요)")]
    public string bgmName;

    [Tooltip("이 구역의 BGM 볼륨 비율 (1 = 100%, 0.5 = 50%)")]
    [Range(0f, 1f)] public float bgmVolume = 1f;
}