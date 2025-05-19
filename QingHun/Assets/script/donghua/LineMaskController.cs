using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class LineMaskController : MonoBehaviour
{
    public LineController targetLine;
    public float maskMoveSpeed = 4f;

    void Update()
    {
        // ͬ������λ�ú���������
        float targetX = Mathf.Lerp(-5, 0, targetLine.currentProgress);
        transform.position = new Vector3(
            Mathf.MoveTowards(transform.position.x, targetX, maskMoveSpeed * Time.deltaTime),
            transform.position.y,
            transform.position.z
        );
    }
}