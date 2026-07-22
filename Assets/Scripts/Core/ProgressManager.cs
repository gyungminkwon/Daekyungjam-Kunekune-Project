using UnityEngine;
using System;
using System.Collections.Generic;

public enum ProgressFlag
{
    None,

    Has_Bedroom_Key,
    Has_Doll,
    HasClassroomKey,
    HasTrowel,
    HasBusTicket
}
public class ProgressManager : MonoBehaviour
{
    public static ProgressManager Instance { get; private set; }

    public event Action<ProgressFlag, bool> OnFlagChanged;

    private Dictionary<ProgressFlag, bool> flags = new Dictionary<ProgressFlag, bool>();

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void SetFlag(ProgressFlag flagID, bool value)
    {
        flags[flagID] = value;

        OnFlagChanged?.Invoke(flagID, value);
    }

    public bool GetFlag(ProgressFlag flagID)
    {
        return flags.TryGetValue(flagID, out bool value) && value;
    }
}
