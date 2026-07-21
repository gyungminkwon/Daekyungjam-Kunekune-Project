using UnityEngine;

// 단순 아이템 획득: ItemPickup
// 조건부 상태 변경: ConditionDoor
// 단발성 이벤트: OneTimeEventObject
public class GameProgressManager : MonoBehaviour
{
    public static GameProgressManager Instance { get; private set; }

    [Header("Flags")]
    public bool hasBedroomKey = false;
    public bool hasDoll = false;
    public bool hasClassroomKey = false;
    public bool hasTrowel = false;
    public bool hasBusTicket = false;
    
    [Header("Event")]
    public bool isPot = false;
    public bool isFuneralStand = false;
    public int toiletOpenedCount = 0;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
}