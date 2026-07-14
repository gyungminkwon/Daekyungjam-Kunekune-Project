using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Slider staminaGuage;
    [SerializeField] private PlayerStamina stamina;

    
    void Update()
    {
        if (stamina != null)
        {
            staminaGuage.value = stamina.Ratio;
        }
    }
}
