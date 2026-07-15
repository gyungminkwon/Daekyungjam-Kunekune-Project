using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

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

        if (input != null && input.IsInteract && currentTarget != null)
        {
            currentTarget.Interact();
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
