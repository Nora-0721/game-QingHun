// HealthReward.cs（完整版）
using UnityEngine;

public class HealthReward : MonoBehaviour
{
    [Header("治疗设置")]
    public int healAmount = 20;
    private float _moveSpeed;

    void Update()
    {
        transform.Translate(Vector2.left * _moveSpeed * Time.deltaTime);
    }

    public void Initialize(float speed)
    {
        _moveSpeed = speed;
        Debug.Log($"奖励速度: {_moveSpeed}");
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        Debug.Log("奖励触碰成功！"); // 新增调试日志
        movement player = other.GetComponent<movement>();
        if (player != null)
        {
            player.Heal(healAmount);
            Debug.Log($"玩家恢复 {healAmount}HP");
        }
        Destroy(gameObject);
    }
}