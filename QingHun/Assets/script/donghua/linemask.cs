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
        // ���ƽ��ȣ�ʾ��ʹ�ü������룩
        if (Input.GetKey(KeyCode.Space))
            revealProgress = Mathf.Clamp01(revealProgress + Time.deltaTime);

        UpdateMask();
    }

    void UpdateMask()
    {
        // ���м�������չ��
        transform.localScale = new Vector3(
            Mathf.Lerp(initialSize.x, 0, revealProgress),
            initialSize.y,
            1
        );
    }
}