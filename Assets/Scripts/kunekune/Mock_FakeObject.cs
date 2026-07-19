using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public interface ITransformable
{
    void StartTransformation(GameObject monsterPrefab, float duration);
}

public class Mock_FakeObject : MonoBehaviour, ITransformable
{
    [Header("Collaboration Events")]
    public UnityEvent onTransformStart;

    // 교체되는 찰나(Glitch 순간)에 권경민님의 노이즈 MAX 함수를 호출할 이벤트
    public UnityEvent onGlitchMaskingMoment;
    public UnityEvent onTransformComplete;

    private SpriteRenderer spriteRenderer;
    private bool isTransforming = false;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void StartTransformation(GameObject monsterPrefab, float duration)
    {
        if (isTransforming) return;
        isTransforming = true;

        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        //허수아비 애니메이터에게 변신 애니메이션 재생 명령 내리기
        Animator anim = GetComponent<Animator>();
        if (anim != null)
        {
            anim.updateMode = AnimatorUpdateMode.UnscaledTime; //슬로 모션 무시하고 재생
            anim.SetTrigger("doTransform"); //애니메이터 파라미터 트리거 발동
        }

        onTransformStart?.Invoke();
        StartCoroutine(SeamlessMorphingRoutine(monsterPrefab, duration));
    }

    private IEnumerator SeamlessMorphingRoutine(GameObject monsterPrefab, float totalDuration)
    {
        float timer = 0f;
        Vector3 originalPos = transform.position;

        //1단계: 전체 시간의 75% 동안은 사물이 기괴하게 변신 애니메이션 재생 (또는 떨림)
        float phase1Duration = totalDuration * 0.75f;

        while (timer < phase1Duration)
        {
            timer += Time.unscaledDeltaTime;

            //(에디터 애니메이터를 쓴다면 애니메이션이 재생 중일 것이고, 코드로 한다면 기괴하게 떨림)
            float offsetX = Mathf.Sin(Time.unscaledTime * 50f) * 0.1f;
            transform.position = originalPos + new Vector3(offsetX, 0, 0);

            //색상이 점차 검붉게 짓눌림
            if (spriteRenderer != null)
            {
                spriteRenderer.color = Color.Lerp(Color.white, new Color(0.3f, 0f, 0f), timer / phase1Duration);
            }

            yield return null;
        }

        // =========================================================================
        // 2단계: 크로스 페이드 모핑 (Cross-Fade Morphing) 시작!
        // =========================================================================

        GameObject monster = null;
        SpriteRenderer monsterSR = null;
        MonoBehaviour[] monsterAIs = null;
        Collider2D[] monsterColliders = null; // ★ [버그 방지] 몬스터의 충돌체 배열 추가

        if (monsterPrefab != null)
        {
            monster = Instantiate(monsterPrefab, originalPos, Quaternion.identity);
            monsterSR = monster.GetComponent<SpriteRenderer>();

            // 1. 변이가 끝나기 전까지 혼자 움직이지 못하게 AI 스크립트 일시 중지
            monsterAIs = monster.GetComponents<MonoBehaviour>();
            foreach (var ai in monsterAIs)
            {
                if (ai != this && ai.GetType().Name.Contains("AI")) ai.enabled = false;
            }

            // 2. 투명한 상태에서 플레이어와 부딪혀 씬이 재시작되지 않도록
            // 몬스터에 붙은 모든 충돌체(CapsuleCollider2D, BoxCollider2D 등)를 완전히 오프
            monsterColliders = monster.GetComponentsInChildren<Collider2D>();
            foreach (var col in monsterColliders)
            {
                col.enabled = false;
            }

            // 3. 투명도 0(완전 투명)으로 설정
            if (monsterSR != null)
            {
                Color c = monsterSR.color;
                c.a = 0f;
                monsterSR.color = c;
            }
        }

        onGlitchMaskingMoment?.Invoke();
        Debug.Log("[Glitch Masking] 화면 왜곡을 일으키며 안전하게 실루엣 교체를 진행합니다!");

        float phase2Duration = totalDuration * 0.25f;
        float fadeTimer = 0f;

        while (fadeTimer < phase2Duration)
        {
            fadeTimer += Time.unscaledDeltaTime;
            float progress = fadeTimer / phase2Duration;

            if (spriteRenderer != null)
            {
                Color c = spriteRenderer.color;
                c.a = Mathf.Lerp(1f, 0f, progress);
                spriteRenderer.color = c;
            }

            if (monsterSR != null)
            {
                Color c = monsterSR.color;
                c.a = Mathf.Lerp(0f, 1f, progress);
                monsterSR.color = c;
            }

            yield return null;
        }

        // =========================================================================
        // 3단계: 변이 완료! AI와 충돌체를 동시에 깨웁니다
        // =========================================================================

        // 1. 멈춰두었던 AI 스크립트 재작동
        if (monsterAIs != null)
        {
            foreach (var ai in monsterAIs)
            {
                if (ai != null) ai.enabled = true;
            }
        }

        // 2.이제 눈에 완벽하게 보이고 변신이 끝났으므로 콜리드 다시 켜기
        // 지금 이 순간부터 플레이어와 닿으면 정상적으로 씬 재시작(Game Over) 판정이 가능해집니다.
        if (monsterColliders != null)
        {
            foreach (var col in monsterColliders)
            {
                if (col != null) col.enabled = true;
            }
        }

        onTransformComplete?.Invoke();
        Debug.Log("변이 완료! 충돌체 활성화 및 추적 시작.");

        Destroy(gameObject);
    }
}