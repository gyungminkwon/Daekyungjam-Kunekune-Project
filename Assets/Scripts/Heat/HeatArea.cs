using UnityEngine;

public class HeatArea : MonoBehaviour
{
    public int heatIntensity = 10;

    [SerializeField] private float gracePeriod = 0.5f;

    private Collider2D sunCollider;
    private Transform playerTransform;

    private bool isPlayerInside = false;
    private float insideTimer = 0f;

    void Awake()
    {
        sunCollider = GetComponent<Collider2D>();
    }

    void Update()
    {
        if (isPlayerInside && playerTransform != null)
        {   if (sunCollider.OverlapPoint(playerTransform.position))
            {
                insideTimer += Time.deltaTime;
                if (insideTimer >= gracePeriod)
                {
                    HeatManager.Instance.HeatUp(heatIntensity);   
                }
            }
        }
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInside = true;
            playerTransform = collision.transform;
            insideTimer = 0f;
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInside = false;
            playerTransform = null;
            insideTimer = 0f;
        }
    }
}
