using UnityEngine;

public class ChangeDateTrigger : MonoBehaviour
{
    [SerializeField] private PlayerGrowthManager growth;
    [SerializeField] private PlayerGrowthManager.GrowthStage stage;
    [SerializeField] private ProgressFlag requiredFlag;

    [SerializeField] private SpriteRenderer skyRenderer;
    [SerializeField] private Sprite nextSky;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (ProgressManager.Instance.GetFlag(requiredFlag))
            {
                growth.SetGrowthStage(stage);
                gameObject.SetActive(false);

                skyRenderer.sprite = nextSky;
            }
        }
        
    }
}
