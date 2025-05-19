// ObstacleController.cs
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class ObstacleController : MonoBehaviour
{
    public int TrackIndex { get; private set; }
    public float Width { get; private set; }
    private float _moveSpeed;
    private SpriteRenderer _spriteRenderer;

    public float RightEdge => transform.position.x + Width * 0.5f;

    void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        UpdateSize();
    }

    public void Initialize(int trackIndex, float speed)
    {
        TrackIndex = trackIndex;
        _moveSpeed = speed;
    }

    void Update()
    {
        transform.Translate(Vector3.left * _moveSpeed * Time.deltaTime);
    }

    public void UpdateSize()
    {
        Width = _spriteRenderer.size.x;
    }
}