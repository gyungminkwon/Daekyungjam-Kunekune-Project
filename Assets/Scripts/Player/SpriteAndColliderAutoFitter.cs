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

    public void ApplyNewImageWithBottomPivot(Texture2D newTexture)
    {
        if (newTexture == null || spriteRenderer == null) return;

        //피벗을 '발바닥(0.5f, 0.0f)'으로 잡아서 새 스프라이트 생성
        Vector2 bottomPivot = new Vector2(0.5f, 0.0f);
        Sprite newSprite = Sprite.Create(
            newTexture,
            new Rect(0, 0, newTexture.width, newTexture.height),
            bottomPivot,
            16f // Pixels Per Unit
        );

        spriteRenderer.sprite = newSprite;

        //바닥 고정 공식을 적용한 콜라이더 맞춤
        if (capsuleCollider != null)
        {
            float spriteWidth = newSprite.bounds.size.x;
            float spriteHeight = newSprite.bounds.size.y;

            float bottomGap = 0.2f; //발바닥에서 띄울 간격
            float targetHeight = Mathf.Max(spriteHeight - bottomGap, 0.5f);
            
            //맨 밑바닥을 bottomGap에 고정하는 중심 공식!
            float targetOffsetY = bottomGap + (targetHeight / 2f);

            capsuleCollider.size = new Vector2(spriteWidth, targetHeight);
            capsuleCollider.offset = new Vector2(0f, targetOffsetY);

            Debug.Log($"[AutoFitter 완료] X 너비: {spriteWidth} 완벽 동기화 / 발 위치 고정 됨");
        }
    }
}