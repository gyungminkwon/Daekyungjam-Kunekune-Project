using UnityEngine;
using System.Collections.Generic;

// 인스펙터 창에서 이름과 소리 파일을 짝지어 넣을 수 있게 해주는 구조체
[System.Serializable]
public struct Sound
{
    public string soundName; // 예: "JumpScare", "DoorOpen" 등
    public AudioClip clip;
}

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("스피커 연결")]
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("효과음(SFX) 보관함")]
    [Tooltip("여기에 게임에 쓰이는 모든 효과음을 등록해두세요.")]
    public Sound[] sfxList;

    // ★ [추가] 배경음악(BGM) 보관함
    [Header("배경음악(BGM) 보관함")]
    [Tooltip("여기에 게임에 쓰이는 모든 배경음악을 등록해두세요.")]
    public Sound[] bgmList;

    // 소리 이름을 빠르게 찾기 위한 딕셔너리(사전)
    private Dictionary<string, AudioClip> sfxDictionary = new Dictionary<string, AudioClip>();
    private Dictionary<string, AudioClip> bgmDictionary = new Dictionary<string, AudioClip>(); // ★ [추가]

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // 게임이 시작될 때 효과음 리스트를 사전에 싹 정리해둡니다
            foreach (Sound sfx in sfxList)
            {
                if (!sfxDictionary.ContainsKey(sfx.soundName))
                {
                    sfxDictionary.Add(sfx.soundName, sfx.clip);
                }
            }

            // ★ [추가] BGM 리스트도 사전에 싹 정리해둡니다
            foreach (Sound bgm in bgmList)
            {
                if (!bgmDictionary.ContainsKey(bgm.soundName))
                {
                    bgmDictionary.Add(bgm.soundName, bgm.clip);
                }
            }

        }
        else
        {
            Destroy(gameObject);
        }
    }

    // SoundManager.cs 내부의 Awake() 함수 아래에 추가해 주세요!
    private void Start()
    {
        // 여기에 게임 시작 시 기본으로 계속 틀어둘 BGM 이름을 적습니다.
        PlayBGM("outside");
    }

    // 효과음 틀기 함수 (이름으로 호출)
    public void PlaySFX(string name)
    {
        if (sfxDictionary.ContainsKey(name))
        {
            sfxSource.PlayOneShot(sfxDictionary[name]);
        }
        else
        {
            Debug.LogWarning($"[SoundManager] '{name}'(이)라는 이름의 효과음을 찾을 수 없습니다!");
        }
    }

    // 배경음악(BGM) 틀기 함수
    public void PlayBGM(string name)
    {
        if (bgmDictionary.ContainsKey(name))
        {
            AudioClip targetClip = bgmDictionary[name];

            // 중요: 이미 똑같은 음악이 재생 중이라면 처음부터 다시 틀지 않고 그대로 유지합니다!
            if (bgmSource.clip == targetClip && bgmSource.isPlaying) return;

            bgmSource.clip = targetClip;
            bgmSource.Play();
        }
        else
        {
            Debug.LogWarning($"[SoundManager] '{name}'(이)라는 이름의 BGM을 찾을 수 없습니다!");
        }
    }

    // 배경음악 즉시 끄기 함수 (필요할 때 호출)
    public void StopBGM()
    {
        if (bgmSource != null && bgmSource.isPlaying)
        {
            bgmSource.Stop();
        }
    }
}