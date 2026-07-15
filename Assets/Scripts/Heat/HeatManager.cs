using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeatManager : MonoBehaviour
{
    public static HeatManager Instance { get; private set; }

    public enum TimeOfDay { Morning, Day, Night }
    
    [Header("Heat Limits")]
    [SerializeField] private int maxHeat = 100;

    [Header("Interval Settings")]
    [SerializeField] private float interval = 1f;

    [Header("Time Settings (Test)")]
    [SerializeField] private TimeOfDay currentTime = TimeOfDay.Night;

    public int CurrentHeat { get; private set; }

    private HashSet<HeatArea> activeHeatAreas = new HashSet<HeatArea>();
    private float heatupTimer = 0f;
    private float cooldownTimer = 0f;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        } 
        else
        {
            Destroy(gameObject);    
        }

        CurrentHeat = 0;
    }

    void Update()
    {
        if (activeHeatAreas.Count > 0)
        {
            HandleHeatUp();
        }
        else
        {
            HandleCoolDown();
        }
    }

    public void RegisterHeatArea(HeatArea area)
    {
        activeHeatAreas.Add(area);
    }

    public void UnregisterHeatArea(HeatArea area)
    {
        activeHeatAreas.Remove(area);
    }

    private void HandleHeatUp()
    {
        heatupTimer -= Time.deltaTime;

        if (heatupTimer <= 0f)
        {
            heatupTimer = interval;

            int intensity = currentTime == TimeOfDay.Morning ? 10 :
                                    currentTime == TimeOfDay.Day ? 20 : 15;
            CurrentHeat = Mathf.Min(CurrentHeat + intensity, maxHeat);

            Debug.Log($"[더위 증가] {CurrentHeat} / {maxHeat} (강도: {intensity})");
        }
    }

    private void HandleCoolDown()
    {
        cooldownTimer -= Time.deltaTime;
        if (cooldownTimer <= 0f)
        {
            cooldownTimer = interval;

            int intensity = currentTime == TimeOfDay.Morning ? 7 :
                            currentTime == TimeOfDay.Day ? 0 : 4;
            
            CurrentHeat = Mathf.Max(CurrentHeat - intensity, 0);

            Debug.Log($"[더위 감소] {CurrentHeat} / {maxHeat} ({intensity} 감소)");
        }
    }

    public void SetTimeOfDay(TimeOfDay time)
    {
        currentTime = time;
    }

    public void HeatDown(int amount)
    {
        if (cooldownTimer > 0) return;

        cooldownTimer = interval;
        CurrentHeat -= amount;
        CurrentHeat = Mathf.Clamp(CurrentHeat, 0, maxHeat);
        Debug.Log($"더위가 조금씩 가신다... ({CurrentHeat} / {maxHeat})");
    }

    public float GetHeatRatio()
    {
        return (float)CurrentHeat / maxHeat;
    }
}
