using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class LineRevealController : MonoBehaviour
{
    [Range(0, 1)] public float revealProgress;
    private Vector2 initialSize;

    void Start()
    {
        initialSize = new Vector2(2, GetComponent<SpriteRenderer>().bounds.size.y);
        UpdateMask();
    }

    void Update()
    {
        // 控制进度（示例使用键盘输入）
        if (Input.GetKey(KeyCode.Space))
            revealProgress = Mathf.Clamp01(revealProgress + Time.deltaTime);

        UpdateMask();
    }

    void UpdateMask()
    {
        // 从中间向两边展开
        transform.localScale = new Vector3(
            Mathf.Lerp(initialSize.x, 0, revealProgress),
            initialSize.y,
            1
        );
    }
}