using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    private PlayerInput input;

    private List<IInteractable> nearbyInteractables = new List<IInteractable>();
    private IInteractable currentTarget = null;
    private InputAction interactAction;

    void Awake()
    {
        input = GetComponent<PlayerInput>();
    }

    void Update()
    {
        UpdateCurrentTarget();

        if (input == null) return;

        // ★ [핵심 수정] input.IsCrouch가 'false'(서 있는 상태)일 때만 F키 상호작용을 허용합니다!
        if (!input.IsCrouch && input.IsInteract && currentTarget != null)
        {
            Debug.Log("🔍 [상호작용 실행] 서 있는 상태에서 F키를 눌렀습니다.");
            currentTarget.Interact();
        }
        else if (input.IsCrouch && input.IsInteract && currentTarget != null)
        {
            // (선택 사항) 앉아서 눌렀을 때 왜 안 되는지 플레이어에게 안내하고 싶다면 로그나 UI를 띄워주세요.
            Debug.Log("[상호작용 불가] 앉은 상태에서는 상호작용(F키)을 할 수 없습니다!");
        }
    }

    private void UpdateCurrentTarget()
    {
        if (nearbyInteractables.Count == 0)
        {
            if (currentTarget != null) ClearTarget();
            return;
        }

        IInteractable closest = null;
        float closestDistance = Mathf.Infinity;
        Vector2 playerPos = transform.position;

        foreach (var interactable in nearbyInteractables)
        {
            if (interactable is MonoBehaviour mono)
            {
                float distance = Vector2.Distance(playerPos, mono.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closest = interactable;
                }
            }
        }

        if (currentTarget != closest)
        {
            currentTarget = closest;
            if (currentTarget != null)
            {
                Debug.Log($"{currentTarget.GetInteractPrompt()}");
            }
        }
    }

    private void ClearTarget()
    {
        currentTarget = null;
        Debug.Log("상호작용 타겟 없음");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        IInteractable interactable = collision.GetComponent<IInteractable>();
        if (interactable != null)
        {
            nearbyInteractables.Add(interactable);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        IInteractable interactable = collision.GetComponent<IInteractable>();
        if (interactable != null)
        {
            nearbyInteractables.Remove(interactable);
        }
    }
}