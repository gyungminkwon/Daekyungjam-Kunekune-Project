using UnityEngine;

public class TreeObstacle : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("사망");
        }
    }
}