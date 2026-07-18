using UnityEngine;

public class KunekuneMovement : MonoBehaviour
{
    public Transform player;
    public float moveSpeed = 3f;      // X축 전진 속도
    public float wiggleSpeed = 10f;   // 위아래로 떨리는 속도
    public float wiggleAmount = 0.3f; // 위아래로 떨리는 폭

    private float startY;

    void Start()
    {
        // 처음 스폰된 높이를 기억해둡니다.
        startY = transform.position.y;
    }

    void Update()
    {
        // 1. 플레이어를 향한 X축 방향을 구합니다. (왼쪽이냐 오른쪽이냐)
        float direction = Mathf.Sign(player.position.x - transform.position.x);

        // 2. X축으로는 일정하게 다가가고, Y축으로는 Sin 함수로 미친듯이 떱니다.
        float newX = transform.position.x + (direction * moveSpeed * Time.deltaTime);
        float newY = startY + (Mathf.Sin(Time.time * wiggleSpeed) * wiggleAmount);

        // 3. 실제 위치 적용
        transform.position = new Vector2(newX, newY);
    }
}