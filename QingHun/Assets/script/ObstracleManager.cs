using UnityEngine;

public class ObstracleManager : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private bool isMoving = true;
    
    private Vector3 originalPosition;
    private bool isPaused;

    private void Start()
    {
        // 记录初始位置
        originalPosition = transform.position;
        
        // 订阅游戏状态事件
        GameManager1.OnGameOver += PauseMovement;
        GameManager1.OnGameRestart += ResumeMovement;
    }

    private void OnDestroy()
    {
        // 取消订阅事件，防止内存泄漏
        GameManager1.OnGameOver -= PauseMovement;
        GameManager1.OnGameRestart -= ResumeMovement;
    }

    private void Update()
    {
        if (isPaused || !isMoving) return;

        // 这里实现障碍物的移动逻辑
        // 例如：左右移动
        float movement = Mathf.Sin(Time.time * moveSpeed) * 2f;
        transform.position = originalPosition + Vector3.right * movement;
    }

    private void PauseMovement()
    {
        isPaused = true;
    }

    private void ResumeMovement()
    {
        isPaused = false;
        // 重置位置
        transform.position = originalPosition;
    }
} 