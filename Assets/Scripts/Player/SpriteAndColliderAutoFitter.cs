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

    //입체감을 위해 발 부분을 제외하고 콜라이더를 맞추는 함수
    public void ApplyNewImageWithBottomPivot(Texture2D newTexture)
    {
        if (newTexture == null || spriteRenderer == null) return;

        // 1. 피벗을 '발바닥(0.5f, 0.0f)'으로 잡아서 새 스프라이트 생성
        Vector2 bottomPivot = new Vector2(0.5f, 0.0f);
        Sprite newSprite = Sprite.Create(
            newTexture,
            new Rect(0, 0, newTexture.width, newTexture.height),
            bottomPivot,
            16f // Pixels Per Unit (프로젝트 설정에 따라 16으로 변경)
        );

        // 2. 캐릭터에 새 이미지 장착
        spriteRenderer.sprite = newSprite;

        // 3. 발 부분을 제외한 캡슐 콜라이더 공식
        if (capsuleCollider != null)
        {
            float spriteWidth = newSprite.bounds.size.x;
            float spriteHeight = newSprite.bounds.size.y;

            //Offset Y를 무조건 1.5로 고정
            float fixedOffsetY = 1.5f;

            // 중심이 1.5일 때 머리 끝(이미지 최상단)에만 캡슐 상단이 닿도록 높이를 계산.
            float calculatedHeight = Mathf.Max((spriteHeight - fixedOffsetY) * 2f, 0.5f);

            capsuleCollider.offset = new Vector2(0f, fixedOffsetY);
            capsuleCollider.size = new Vector2(spriteWidth, calculatedHeight);

            Debug.Log($"[AutoFitter 적용 완료] Offset Y: {fixedOffsetY} / Size Y: {calculatedHeight} (발 부분 제외 됨)");
        }
    }
}