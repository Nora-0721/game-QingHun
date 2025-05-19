using UnityEngine;

public class LineCheck : MonoBehaviour
{
    public Sprite originalSprite;
    public Sprite replacementSprite;

    private SpriteRenderer spriteRenderer;
    private KeyCode activationKey;
    private int touchingPointCount = 0;
    private float conditionTimer = 0;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = originalSprite;
        SetActivationKeyByPosition();
    }

    private void SetActivationKeyByPosition()
    {
        float xPos = transform.position.x;
        activationKey = xPos switch
        {
            _ when Mathf.Approximately(xPos, -5f) => KeyCode.S,
            _ when Mathf.Approximately(xPos, -3f) => KeyCode.D,
            _ when Mathf.Approximately(xPos, -1f) => KeyCode.F,
            _ when Mathf.Approximately(xPos, 1f) => KeyCode.J,
            _ when Mathf.Approximately(xPos, 3f) => KeyCode.K,
            _ when Mathf.Approximately(xPos, 5f) => KeyCode.L,
            _ => KeyCode.None
        };
    }

    private void Update()
    {
        bool isPressingKey = Input.GetKey(activationKey);

        // 更新精灵显示
        spriteRenderer.sprite = (touchingPointCount > 0 && isPressingKey) ?
            replacementSprite : originalSprite;

        // 分数计算逻辑
        if (isPressingKey)
        {
            conditionTimer += Time.deltaTime;
            if (touchingPointCount > 0)
            {
                ScoreManager.Instance.AddScore(Time.deltaTime); // 每秒+1
            }
            //else
            //{
            //    ScoreManager.Instance.AddScore(-Time.deltaTime); // 每秒-1
            //}
        }
        else
        {
            conditionTimer = 0;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("point")) touchingPointCount++;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("point")) touchingPointCount--;
    }
}