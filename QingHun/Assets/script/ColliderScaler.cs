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
        // ��� Scale ����Ƶ���仯�������� Update �г�������
        UpdateColliderSize();
    }

    void UpdateColliderSize()
    {
        if (spriteRenderer == null || boxCollider == null) return;

        // ��ȡ Sprite ��ԭʼ�ߴ磨���� Scale Ӱ�죩
        Vector2 spriteSize = spriteRenderer.sprite.bounds.size;

        // ���� Collider �� size������ Scale �� Y ֵ
        boxCollider.size = new Vector2(
            spriteSize.x ,//* transform.localScale.x,
            spriteSize.y //* transform.localScale.y
        );
    }
}