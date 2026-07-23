using System;
using Unity.VisualScripting;
using UnityEngine;

public class FirstVisitTrigger : MonoBehaviour
{
    [SerializeField] private ProgressFlag flag;
    [SerializeField] private TextData triggerText;
    [SerializeField] SpriteRenderer skyRenderer;
    [SerializeField] Sprite nextSky;
    private bool isPlayerIn = false;

    void Start()
    {
        if (UIManager.Instance != null)
        {
            Debug.Log(UIManager.Instance);
            UIManager.Instance.OnFadeFinished += ShowText;
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerIn = true;
            skyRenderer.sprite = nextSky;
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerIn = false;
        }
    }

    private void ShowText()
    {
        Debug.Log("함수 진입");
        if (ProgressManager.Instance != null && !ProgressManager.Instance.GetFlag(flag) && isPlayerIn)
        {
            Debug.Log("조건문 진입");
            TextManager.Instance?.PlayText(triggerText);
            ProgressManager.Instance?.SetFlag(flag, true);
            if (UIManager.Instance) UIManager.Instance.OnFadeFinished -= ShowText;
            gameObject.SetActive(false);
        }
    }
}
