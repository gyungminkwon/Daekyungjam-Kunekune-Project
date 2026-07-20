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

    /*
     * =========================================================
     * KunekuneAI.cs 연계 내용
     * =========================================================
     */
    [Header("Kunekune Reference")]
    [SerializeField] private KunekuneAI kunekuneAI;

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

            /*
             * =========================================================
             * KunekuneAI.cs 연계 내용
             * =========================================================
             * 더위 게이지가 다 차면 쿠네쿠네를 소환.
             */
            if (CurrentHeat >= maxHeat && kunekuneAI != null && !kunekuneAI.gameObject.activeInHierarchy)
            {
                // 쿠네쿠네가 변신할 수 있는 사물에 tag: KunekuneProp을 붙임.
                // 태그가 붙은 사물을 배열로 묶음.
                GameObject[] props = GameObject.FindGameObjectsWithTag("KunekuneProp");
                GameObject closestProp = null;
                float minDistance = Mathf.Infinity;

                Transform playerTransform = GameObject.FindGameObjectWithTag("Player").transform;

                foreach (GameObject prop in props)
                {
                    float distance = Vector2.Distance(playerTransform.position, prop.transform.position);
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        closestProp = prop;
                    }
                    else if (Mathf.Approximately(distance, minDistance))
                    {
                        if (closestProp != null && prop.transform.position.x > closestProp.transform.position.x)
                        {
                            closestProp = prop;
                        }
                    }
                }

                // 가장 가까운 사물의 위치에 쿠네쿠네를 소환.
                if (closestProp != null)
                {
                    closestProp.SetActive(false);
                    KunekuneProp propData = closestProp.GetComponent<KunekuneProp>();

                    if (propData != null)
                    {
                        kunekuneAI.StartChaseFrom(closestProp.transform.position, closestProp, propData.transformAnimName, propData.transformDuration);
                    }
                    else
                    {
                        kunekuneAI.StartChaseFrom(closestProp.transform.position, closestProp);
                    }
                }
                // 없으면 플레이어의 위치에서 3칸 떨어진 곳에 소환.
                else
                {
                    kunekuneAI.StartChaseFrom(new Vector2(playerTransform.position.x - 3f, playerTransform.position.y), null);
                }
                
                Debug.Log("더위가 최대치에 달해 쿠네쿠네가 소환됩니다.");
            }
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
