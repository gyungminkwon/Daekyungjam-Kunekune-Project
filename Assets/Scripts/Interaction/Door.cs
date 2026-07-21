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

            /* =========================================================
             * KunekuneAI.cs 연계 내용
             * =========================================================
             * 플레이어가 문을 통해 다른 맵으로 이동 시 쿠네쿠네도 함께 쫓아옴.
             */
            KunekuneAI kunekune = Object.FindFirstObjectByType<KunekuneAI>();
            if (kunekune != null && kunekune.gameObject.activeInHierarchy)
            {
                kunekune.ChaseDoorAndTeleport(transform.position, targetPos.position);
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
