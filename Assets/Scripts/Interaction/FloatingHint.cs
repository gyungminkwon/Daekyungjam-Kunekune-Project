using UnityEngine;

public class FloatingHint : MonoBehaviour
{
    [Header("Animation Settings")]
    [SerializeField, Tooltip("위아래로 움직이는 거리")]
    private float floatDistance = 0.15f;
    [SerializeField, Tooltip("위아래로 움직이는 속도")]
    private float floatSpeed = 4.0f;

    private Vector3 startPosition;
    private float timeElapsed;

    // 내부 제어용 변수들
    private SpriteRenderer sr;
    private Collider2D hintCollider;
    private bool isPlayerInRange = false;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        hintCollider = GetComponent<Collider2D>();

        // 에디터에 배치한 초기 위치를 기억
        startPosition = transform.position;

        // ★ 오브젝트 전체를 끄지 않고, 이미지(SpriteRenderer)만 숨깁니다!
        // 그래야 콜리더가 살아있어서 플레이어가 다가오는 걸 감지할 수 있습니다.
        if (sr != null) sr.enabled = false;
        isPlayerInRange = false;
    }

    private void Update()
    {
        // 플레이어가 근처에 없을 때는 위아래로 움직이는 연산을 아예 안 함! (최적화)
        if (!isPlayerInRange) return;

        // 위아래 바운스 연산
        timeElapsed += Time.deltaTime * floatSpeed;
        Vector3 pos = startPosition;
        pos.y += Mathf.Sin(timeElapsed) * floatDistance;
        transform.position = pos;
    }

    // 💡 플레이어가 콜리더 범위 안에 들어왔을 때 알아서 켜짐
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Show();
        }
    }

    // 💡 플레이어가 콜리더 범위 밖으로 나갔을 때 알아서 꺼짐
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Hide();
        }
    }

    // 힌트 이미지를 켜는 함수
    public void Show()
    {
        isPlayerInRange = true;
        timeElapsed = 0f;
        transform.position = startPosition;
        if (sr != null) sr.enabled = true; // 이미지 켜기
    }

    // 힌트 이미지를 끄는 함수
    public void Hide()
    {
        isPlayerInRange = false;
        if (sr != null) sr.enabled = false; // 이미지 끄기
    }

    // ★ 책상에서 F키 눌러서 상호작용 했을 때, 힌트를 영구적으로 끌 때 호출할 함수
    public void DisablePermanently()
    {
        Hide();
        if (hintCollider != null) hintCollider.enabled = false; // 감지까지 완전 차단
        this.enabled = false; // 스크립트 끄기
    }
}