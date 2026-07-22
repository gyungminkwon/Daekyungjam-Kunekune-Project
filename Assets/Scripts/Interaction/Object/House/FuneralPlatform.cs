using UnityEngine;

public class FuneralPlatform : MonoBehaviour, IInteractable
{
    [Header("Interact Settings")]
    [SerializeField] private Sprite afterInteractSprite;
    [SerializeField] private ProgressFlag flag;

    [Header("Text")]
    [SerializeField] private TextData textData;
    private Sprite originalSprite;
    private SpriteRenderer sr;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        originalSprite = sr.sprite;
    }

    void Start()
    {
        if (UIManager.Instance != null)
            UIManager.Instance.OnFadeFinished += RecoverSprite;
    }

    public void OnInteractPressed()
    {
         if (TextManager.Instance == null) return;

        TextManager.Instance.PlayText(textData);

        if (!ProgressManager.Instance.GetFlag(flag))
        {
            if (afterInteractSprite != null) sr.sprite = afterInteractSprite;
            ProgressManager.Instance.SetFlag(flag, true);
        }
    }

    public void RecoverSprite()
    {
        if (sr != null) sr.sprite = originalSprite;
    }
    public void OnInteractHeld() {}
    public void OnInteractReleased() {}

    public string GetInteractPrompt()
    {
        return "장례대 (F)";
    }
}
