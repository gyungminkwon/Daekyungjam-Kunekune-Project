using UnityEngine;
using UnityEngine.UI; // TMPro를 쓰신다면 using TMPro; 로 변경해주세요.

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Guage Sliders")]
    [SerializeField] private Slider staminaGuage;
    [SerializeField] private PlayerStamina stamina;
    [SerializeField] private Slider heatGuage;

    void Awake()
    {
        // 어디서든 UIManager.Instance로 UI를 조작할 수 있게 싱글톤 설정
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Update()
    {
        if (stamina != null && staminaGuage != null)
        {
            staminaGuage.value = stamina.Ratio;
        }

        if (heatGuage != null && HeatManager.Instance != null)
        {
            heatGuage.value = HeatManager.Instance.GetHeatRatio();
        }
    }
}