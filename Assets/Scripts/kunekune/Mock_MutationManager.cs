using UnityEngine;
using UnityEngine.InputSystem;

public class Mock_MutationManager : MonoBehaviour
{
    [Header("Targeting & Setup")]
    [Tooltip("플레이어 위치 연결")]
    [SerializeField] private Transform playerTransform;

    [Tooltip("변신 가능한 사물들이 속한 레이어 지정")]
    [SerializeField] private LayerMask transformableLayer;

    [Tooltip("주변 몇 미터 이내의 사물을 탐색할 것인가?")]
    [SerializeField] private float searchRadius = 10f;

    [Header("Monster Setup")]
    [Tooltip("변신이 끝나고 소환될 쿠네쿠네 프리팹 (임시 몬스터 프리팹 넣기)")]
    [SerializeField] private GameObject kunekunePrefab;

    [Tooltip("변신 연출에 걸리는 시간 (초)")]
    [SerializeField] private float mutationDuration = 2.0f;

    [Header("Test Controls")]
    [Tooltip("체크하면 더위 게이지가 안 차도 K키를 눌러서 바로 변신을 테스트할 수 있습니다.")]
    [SerializeField] private bool enableDebugKey = true;

    //Zero-GC 탐색용 버퍼 배열
    private Collider2D[] hitColliders = new Collider2D[10];
    private bool isTriggered = false;

    void Update()
    {
        if (isTriggered || playerTransform == null) return;

        // 1. 실제 게임 작동 조건: 더위 게이지 100 이상 시 발동
        if (HeatManager.Instance != null && HeatManager.Instance.CurrentHeat >= 100)
        {
            ExecuteMutation();
        }

        // 2. 개발/테스트용: K키를 누르면 더위 상관없이 가장 가까운 사물이 즉시 변신!
        if (enableDebugKey && Keyboard.current != null && Keyboard.current.kKey.wasPressedThisFrame)
        {
            Debug.Log("[테스트] K키 입력 감지: 강제 변신 시퀀스 발동!");
            ExecuteMutation();
        }
    }

    public void ExecuteMutation()
    {
        if (isTriggered) return;

        // 가장 가까운 변신 가능 사물 찾기
        ITransformable targetObject = FindClosestTransformable();

        if (targetObject == null)
        {
            Debug.LogWarning("주변 반경 내에 변신할 수 있는 사물(Mock_FakeObject)이 없습니다!");
            return;
        }

        isTriggered = true;

        // ★ [고급 연출] 시네마틱 슬로 모션 발동!
        Time.timeScale = 0.2f;
        Debug.Log("시간이 5배 느려집니다. (시네마틱 슬로 모션 발동)");

        // 대상 사물에게 변신 명령 내리기
        targetObject.StartTransformation(kunekunePrefab, mutationDuration);

        // 연출 시간이 끝나면 슬로 모션을 끄는 타이머 코루틴 시작
        StartCoroutine(ResetTimeScaleRoutine(mutationDuration));
    }

    // ★ Zero-GC & sqrMagnitude 최적화 탐색 로직
    private ITransformable FindClosestTransformable()
    {
        ContactFilter2D filter = new ContactFilter2D { layerMask = transformableLayer, useLayerMask = true };
        int count = Physics2D.OverlapCircle(playerTransform.position, searchRadius, filter, hitColliders);
        if (count == 0) return null;

        ITransformable closestObj = null;
        float minSqrDist = float.MaxValue;

        for (int i = 0; i < count; i++)
        {
            // 인터페이스(ITransformable)를 가진 오브젝트인지 확인
            ITransformable transformable = hitColliders[i].GetComponent<ITransformable>();
            if (transformable != null)
            {
                float sqrDist = (hitColliders[i].transform.position - playerTransform.position).sqrMagnitude;
                if (sqrDist < minSqrDist)
                {
                    minSqrDist = sqrDist;
                    closestObj = transformable;
                }
            }
        }

        return closestObj;
    }

    private System.Collections.IEnumerator ResetTimeScaleRoutine(float delay)
    {
        // UnscaledTime을 기준으로 기다려야 슬로 모션 중에도 정확한 시간 후에 풀립니다!
        yield return new WaitForSecondsRealtime(delay);

        Time.timeScale = 1.0f;
        isTriggered = false; // 다음 변신을 위해 초기화 (필요시 제거)
        Debug.Log("시간 속도가 정상으로 돌아왔습니다.");
    }

    private void OnDrawGizmosSelected()
    {
        if (playerTransform == null) return;
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(playerTransform.position, searchRadius);
    }
}