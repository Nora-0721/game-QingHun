using UnityEngine;

public class ColliderScaler : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private BoxCollider2D boxCollider;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        boxCollider = GetComponent<BoxCollider2D>();
        UpdateColliderSize();
    }

    void Update()
    {
        // 如果 Scale 可能频繁变化，可以在 Update 中持续更新
        UpdateColliderSize();
    }

    void UpdateColliderSize()
    {
        if (spriteRenderer == null || boxCollider == null) return;

        // 获取 Sprite 的原始尺寸（不受 Scale 影响）
        Vector2 spriteSize = spriteRenderer.sprite.bounds.size;

        // 计算 Collider 的 size，乘以 Scale 的 Y 值
        boxCollider.size = new Vector2(
            spriteSize.x ,//* transform.localScale.x,
            spriteSize.y //* transform.localScale.y
        );
    }
}