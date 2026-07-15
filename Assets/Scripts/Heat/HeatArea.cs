using UnityEngine;

public class HeatArea : MonoBehaviour
{   
    [Header("Heat delay")]
    [SerializeField] private float delayBeforeHeat = 0.15f;

    private Collider2D areaCollider;
    private float timer = 0f;
    private Transform player;
    private bool isPlayerIn = false;
    private bool isRegistered = false;
    
    void Awake()
    {
        areaCollider = GetComponent<Collider2D>();
    }

    void Update()
    {
        if (player != null && isPlayerIn)
        {
            if (areaCollider.OverlapPoint(player.position))
            {
                timer += Time.deltaTime;

                if (timer >= delayBeforeHeat && !isRegistered)
                {
                    HeatManager.Instance.RegisterHeatArea(this);
                    isRegistered = true;
                }
            }
            else
            {
                ResetState();
            }
        }
    }

    private void ResetState()
    {
        timer = 0f;
        if (isRegistered)
        {
            HeatManager.Instance.UnregisterHeatArea(this);
            isRegistered = false;
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            timer = 0f;
            player = collision.transform;
            isPlayerIn = true;
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerIn = false;
            player = null;
            HeatManager.Instance.UnregisterHeatArea(this);
        }
    }
}
