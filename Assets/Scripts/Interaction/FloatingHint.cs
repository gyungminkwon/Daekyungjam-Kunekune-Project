using UnityEngine;

public class FloatingHint : MonoBehaviour
{
    [SerializeField, Tooltip("위아래로 움직이는 거리")]
    private float floatDistance = 0.15f;
    [SerializeField, Tooltip("위아래로 움직이는 속도")]
    private float floatSpeed = 4.0f;

    private Vector3 startPosition;
    private float timeElapsed;

    private void Awake()
    {
        // 에디터에 배치된 초기 위치를 기준점으로 기억
        startPosition = transform.position;
        gameObject.SetActive(false); // 시작할 때는 숨김
    }

    private void OnEnable()
    {
        // 켜질 때마다 위치와 시간 초기화 (오류 방지)
        transform.position = startPosition;
        timeElapsed = 0f;
    }

    private void Update()
    {
        // 위아래 바운스 연산
        timeElapsed += Time.deltaTime * floatSpeed;
        Vector3 pos = startPosition;
        pos.y += Mathf.Sin(timeElapsed) * floatDistance;
        transform.position = pos;
    }

    // 외부에서 쉽게 켜고 끌 수 있는 함수
    public void Show() => gameObject.SetActive(true);
    public void Hide() => gameObject.SetActive(false);
}