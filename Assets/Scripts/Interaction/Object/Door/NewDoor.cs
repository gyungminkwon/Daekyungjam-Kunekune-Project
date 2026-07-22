using System.Collections;
using UnityEngine;

public class NewDoor : MonoBehaviour
{
    [Header("Teleport Settings")]
    [SerializeField] private Transform targetPos;

    public IEnumerator TeleportRoutine()
    {
        if (targetPos == null) yield break;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) yield break;

        PlayerInput playerInput = player.GetComponent<PlayerInput>();
        if (playerInput != null) playerInput.enabled = false;

        yield return new WaitForSeconds(0.3f);

        yield return new WaitForSeconds(0.5f);
        
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

        if (playerInput != null) playerInput.enabled = true;
    }
}