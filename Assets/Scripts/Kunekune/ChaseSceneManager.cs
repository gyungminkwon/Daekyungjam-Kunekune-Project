using UnityEngine;

public class ChaseSceneManager : MonoBehaviour
{
    public static ChaseSceneManager Instance { get; private set; }
    [Header("추격전 연출 설정")]
    [Tooltip("추격 전용 쿠네쿠네 할당")]
    public KunekuneChaseAI chaseKunekune;
    
    [Tooltip("허수아비 할당")]
    public KunekuneProp startingProp;

    private int activeHeatAreaCount = 0;
    private int activeFreezeeAreaCount = 0;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    void Start()
    {
        if (chaseKunekune != null && startingProp != null)
        {
            Debug.Log("추격전 시작");
            chaseKunekune.StartChaseFromProp(startingProp);
        }
        else
        {
            Debug.LogWarning("쿠네쿠네 또는 시작 프롭이 할당되지 않았습니다");
        }
    }

    public void AddPlayerHeatArea()
    {
        activeHeatAreaCount++;
        UpdateKunekuneSpeedState();
    }

    public void RemovePlayerHeatArea()
    {
        activeHeatAreaCount--;
        if (activeHeatAreaCount < 0) activeHeatAreaCount = 0; 
        
        UpdateKunekuneSpeedState();
    }

    public void AddPlayerFreezeeArea()
    {
        activeFreezeeAreaCount++;
        UpdateKunekuneSpeedState();
    }

    public void RemovePlayerFreezeeArea()
    {
        activeFreezeeAreaCount--;
        if (activeFreezeeAreaCount < 0) activeFreezeeAreaCount = 0;
        UpdateKunekuneSpeedState();
    }

    private void UpdateKunekuneSpeedState()
    {
        if (chaseKunekune != null)
        {
            bool isHot = (activeHeatAreaCount > 0) && (activeFreezeeAreaCount == 0);
            
            chaseKunekune.isPlayerInHeat = isHot;
        }
    }
}