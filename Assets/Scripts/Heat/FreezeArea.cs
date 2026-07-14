using UnityEngine;

public class FreezeArea : MonoBehaviour
{
    public int freezeIntensity = 5;

    [SerializeField] private float gracePeriod = 0.3f;

    private Collider2D shadowCollider;
    private Transform playerTransform;

    private bool isPlayerInside = false;
    private float insideTimer = 0f;

    void Awake()
    {
        shadowCollider = GetComponent<Collider2D>();
    }

    void Update()
    {
        if (isPlayerInside && playerTransform != null)
        {   if (shadowCollider.OverlapPoint(playerTransform.position))
            {
                insideTimer += Time.deltaTime;
                if (insideTimer >= gracePeriod)
                {
                    HeatManager.Instance.HeatDown(freezeIntensity);   
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
