using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PostProcessController : MonoBehaviour
{
    [Header("관련 오브젝트")]
    public Volume globalVolume;
    public Transform player;
    public Transform kunekune;

    [Header("더위 게이지 관련 효과")]
    public float maxLensDistortion = 0.2f; // 렌즈 왜곡

    public Color normalVignetteColor = Color.black; // 기본 비네트 색상
    public Color heatVignetteColor = Color.red;     // 더울 때 비네트 색상
    public float normalVignetteIntensity = 0.4f;    // 기본 비네트 강도
    public float maxVignetteIntensity = 0.6f;       // 더울 때 비네트 강도

    public float maxBloomIntensity = 1.5f; // 빛 번짐 강도

    [Header("쿠네쿠네 거리 관련 효과")]
    public float maxDangerDistance = 7f; // 쿠네쿠네 거리
    public float maxChromaticAberration = 0.8f; // 색수차 강도
    public float maxFilmGrainIntensity = 5f; // 노이즈 강도
    public float maxMotionBlurIntensity = 1f; // 모션 블러 강도

    private LensDistortion lensDistortion;
    private Vignette vignette;
    private ChromaticAberration chromaticAberration;
    private Bloom bloom;
    private FilmGrain filmGrain;
    private MotionBlur motionBlur;
    private KunekuneAI kunekuneScript;

    void Start()
    {
        globalVolume.profile.TryGet(out lensDistortion);
        globalVolume.profile.TryGet(out vignette);
        globalVolume.profile.TryGet(out chromaticAberration);
        globalVolume.profile.TryGet(out bloom);
        globalVolume.profile.TryGet(out filmGrain);
        globalVolume.profile.TryGet(out motionBlur);

        if (kunekune != null)
        {
            kunekuneScript = kunekune.GetComponent<KunekuneAI>();
        }
    }

    void Update()
    {
        float heatRatio = HeatManager.Instance.GetHeatRatio();

        // 더위 게이지
        if (lensDistortion != null)
            lensDistortion.intensity.Override(heatRatio * maxLensDistortion);

        if (bloom != null) 
            bloom.intensity.Override(heatRatio * maxBloomIntensity);

        if (vignette != null)
        {
            Color currentColor = Color.Lerp(normalVignetteColor, heatVignetteColor, heatRatio);
            float currentIntensity = Mathf.Lerp(normalVignetteIntensity, maxVignetteIntensity, heatRatio);

            vignette.color.Override(currentColor);
            vignette.intensity.Override(currentIntensity);
        }

        // 쿠네쿠네
        if (kunekune != null && kunekune.gameObject.activeInHierarchy)
        {
            float distance = Vector2.Distance(player.position, kunekune.position);
            float distanceRatio = 1f - Mathf.Clamp01(distance / maxDangerDistance);

            if (chromaticAberration != null)
                chromaticAberration.intensity.Override(distanceRatio * maxChromaticAberration);
            
            if (filmGrain != null) 
                filmGrain.intensity.Override(distanceRatio * maxFilmGrainIntensity);
            
            if (motionBlur != null && kunekuneScript != null)
            {
                if (kunekuneScript.isDashing)
                    motionBlur.intensity.Override(maxMotionBlurIntensity);
                else
                    motionBlur.intensity.Override(0f);
            }
        }
        else
        {
            // 쿠네쿠네 없으면 효과 제거
            if (chromaticAberration != null)
                chromaticAberration.intensity.Override(0f);
            
            if (filmGrain != null)
                filmGrain.intensity.Override(0f);

            if (motionBlur != null)
                motionBlur.intensity.Override(0f);
        }
    }
}