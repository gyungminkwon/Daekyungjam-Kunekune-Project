using UnityEngine;

public class SpriteAndColliderAutoFitter : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private CapsuleCollider2D capsuleCollider;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
    }

    // ★ 텍스처(Texture2D)를 넘겨주면 피벗과 콜라이더를 완벽하게 맞춰서 입히는 함수 ★
    public void ApplyNewImageWithBottomPivot(Texture2D newTexture)
    {
        if (newTexture == null || spriteRenderer == null) return;

        // 1. 피벗을 '발바닥(0.5f, 0.0f)'으로 잡아서 새 스프라이트 생성!
        Vector2 bottomPivot = new Vector2(0.5f, 0.0f);
        Sprite newSprite = Sprite.Create(
            newTexture,
            new Rect(0, 0, newTexture.width, newTexture.height),
            bottomPivot,
            100f // Pixels Per Unit (필요에 따라 100이나 16 등으로 수정)
        );

        // 2. 캐릭터에 새 이미지 장착
        spriteRenderer.sprite = newSprite;

        // 3. 캡슐 콜라이더 자동 맞춤!
        if (capsuleCollider != null)
        {
            // 방법 A: 질문자님이 하신 것처럼 오프셋 Y를 1.5로 고정하고 싶을 때
            // capsuleCollider.offset = new Vector2(0f, 1.5f);

            // 방법 B (강력 추천): 바뀐 이미지의 크기에 맞춰 콜라이더 크기와 위치를 자동 계산!
            // 피벗이 발바닥(0)이므로, 콜라이더의 중심(Offset Y)은 무조건 '이미지 높이의 절반'이 되어야 완벽합니다.
            float spriteWidth = newSprite.bounds.size.x;
            float spriteHeight = newSprite.bounds.size.y;

            capsuleCollider.size = new Vector2(spriteWidth, spriteHeight);
            capsuleCollider.offset = new Vector2(0f, spriteHeight / 2f);

            Debug.Log($"[AutoFitter] 콜라이더 오프셋 자동 조정 완료! Offset Y: {spriteHeight / 2f}");
        }
    }
}