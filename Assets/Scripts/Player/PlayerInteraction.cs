using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    private PlayerInput input;

    private List<IInteractable> nearbyInteractables = new List<IInteractable>();
    private IInteractable currentTarget = null;

    void Awake()
    {
        input = GetComponent<PlayerInput>();
    }

    void Update()
    {
        UpdateCurrentTarget();

        // 웅크리기(Crouch) 중에는 상호작용 불가
        if (input == null || currentTarget == null || input.IsCrouch) return;

        if (input.InteractPressed) currentTarget.OnInteractPressed();
        if (input.InteractHeld) currentTarget.OnInteractHeld();
        if (input.InteractReleased) currentTarget.OnInteractReleased();
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