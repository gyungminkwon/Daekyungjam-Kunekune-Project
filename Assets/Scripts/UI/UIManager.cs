using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Slider staminaGuage;
    [SerializeField] private PlayerStamina stamina;

    [SerializeField] private Slider heatGuage;
    
    void Update()
    {
        if (stamina != null && staminaGuage != null)
        {
            staminaGuage.value = stamina.Ratio;
        }

        if (heatGuage != null)
        {
            heatGuage.value = HeatManager.Instance.GetHeatRatio();
        }
    }
}
