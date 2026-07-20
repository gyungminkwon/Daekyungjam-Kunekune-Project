using UnityEngine;
using UnityEngine.Events; // UnityEvent 사용을 위해 필수!
using UnityEngine.InputSystem;

public class InteractTrigger : MonoBehaviour
{
    [Header("References")]
    [SerializeField, Tooltip("연결할 힌트 UI 스크립트 (FloatingHint)")]
    private FloatingHint floatingHint;
    [SerializeField, Tooltip("감지할 Collider2D (비워두면 자동 할당)")]
    private Collider2D triggerCollider;

    [Header("Settings")]
    [SerializeField, Tooltip("상호작용 키")]
    private Key interactKey = Key.F;
    [SerializeField, Tooltip("일회성 상호작용 여부 (아이템 획득 등은 true, 문 열기/대화는 false)")]
    private bool isOneTimeOnly = true;

    [Header("Events (F키를 눌렀을 때 실행될 기능들)")]
    [SerializeField]
    private UnityEvent onInteract;

    private bool isPlayerInRange = false;
    private bool hasInteracted = false;

    private void Awake()
    {
        if (triggerCollider == null) triggerCollider = GetComponent<Collider2D>();
    }

    private void Update()
    {
        if (hasInteracted || !isPlayerInRange) return;

        // 신버전 인풋: F키를 이번 프레임에 눌렀는가?
        if (Keyboard.current != null && Keyboard.current[interactKey].wasPressedThisFrame)
        {
            Interact();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!hasInteracted && other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            if (floatingHint != null) floatingHint.Show();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!hasInteracted && other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            if (floatingHint != null) floatingHint.Hide();
        }
    }

    private void Interact()
    {
        if (isOneTimeOnly)
        {
            hasInteracted = true;
            isPlayerInRange = false;
            if (triggerCollider != null) triggerCollider.enabled = false;
        }

        if (floatingHint != null) floatingHint.Hide();

        // ★ 핵심: Inspector 창에서 연결해 둔 기능(아이템 등장, 문 열기 등)을 실행!
        onInteract?.Invoke();

        if (isOneTimeOnly)
        {
            this.enabled = false;
        }
    }
}