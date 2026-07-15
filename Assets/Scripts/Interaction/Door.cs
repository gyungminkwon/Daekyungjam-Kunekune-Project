using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    [SerializeField] private Transform targetPos;

    public void Interact()
    {
        if (targetPos == null)
        {
            Debug.Log($"{gameObject.name}: 목적지가 지정되지 않았습니다.");
            return;
        }

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Rigidbody2D rb = player.GetComponent<Rigidbody2D>();

            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
                rb.position = targetPos.position;
            }
            else
            {
                player.transform.position = targetPos.position;
            }

            Physics2D.SyncTransforms();

            Debug.Log($"[Teleport] {targetPos.name} (으)로 이동했습니다.");
        }
    }

    public string GetInteractPrompt()
    {
        return "문 열기(F)";
    }
}
