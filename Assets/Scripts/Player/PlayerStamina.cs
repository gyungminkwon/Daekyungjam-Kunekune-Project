using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerStamina : MonoBehaviour
{
    [Header("Stamina Settings")]
    [SerializeField] private float maxStamina = 100f;
    [SerializeField] private float recoverSpeed = 30f;
    [SerializeField] private float regenCooldownDuration = 1.0f;

    public float Ratio => CurrentStamina / maxStamina;

    private float cooldownTimer;

    public float CurrentStamina { get; private set; }

    void Awake()
    {
        CurrentStamina = maxStamina;
    }

    void Update()
    {
        if (cooldownTimer > 0)
        {
            cooldownTimer -= Time.deltaTime;
        }
        else
        {
            RecoverStamina(Time.deltaTime);
        }
    }

    public bool HasStamina(float amount)
    {
        return CurrentStamina >= amount;
    }

    public void ConsumeStamina(float amount)
    {
        CurrentStamina = Mathf.Max(0f, CurrentStamina - amount);
        cooldownTimer = regenCooldownDuration;
    }
    public void RecoverStamina(float deltaTime)
    {
        CurrentStamina = Mathf.Min(CurrentStamina + recoverSpeed * deltaTime, maxStamina);
    }
}
