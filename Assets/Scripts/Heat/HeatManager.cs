using System.Collections;
using UnityEngine;

public class HeatManager : MonoBehaviour
{
    public static HeatManager Instance { get; private set; }
    
    [SerializeField] private int maxHeat = 100;
    [SerializeField] private float interval = 2f;
    private float heatupTimer = 0f;
    private float heatdownTimer = 0f;
    public int CurrentHeat { get; private set; }

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
        if (heatupTimer > 0)
        {
            heatupTimer -= Time.deltaTime;
        }
        if (heatdownTimer > 0)
        {
            heatdownTimer -= Time.deltaTime;
        }
    }

    public void HeatUp(int amount)
    {
        if (heatupTimer > 0f) return;

        heatupTimer = interval;
        CurrentHeat += amount;
        CurrentHeat = Mathf.Clamp(CurrentHeat, 0, maxHeat);
        Debug.Log($"햇빛에 노출되었다. ({CurrentHeat} / {maxHeat})");
    }

    public void HeatDown(int amount)
    {
        if (heatdownTimer > 0) return;

        heatdownTimer = interval;
        CurrentHeat -= amount;
        CurrentHeat = Mathf.Clamp(CurrentHeat, 0, maxHeat);
        Debug.Log($"더위가 조금씩 가신다... ({CurrentHeat} / {maxHeat})");
    }

    public float GetHeatRatio()
    {
        return (float)CurrentHeat / maxHeat;
    }
}
