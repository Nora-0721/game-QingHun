using UnityEngine;

public class ObstacleCollision : MonoBehaviour
{
    [Tooltip("每秒造成的伤害值")]
    [SerializeField] private float damagePerSecond = 20f;

    private void Start()
    {
        Debug.Log($"障碍物 {gameObject.name} 已初始化");
        
        // 检查必要组件
        Collider2D collider = GetComponent<Collider2D>();
        if (collider == null)
        {
            Debug.LogError($"障碍物 {gameObject.name} 缺少 Collider2D 组件！");
        }
        else
        {
            Debug.Log($"障碍物 {gameObject.name} Collider2D类型: {collider.GetType()}, IsTrigger: {collider.isTrigger}");
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log($"发生碰撞！与物体: {collision.gameObject.name}, Tag: {collision.gameObject.tag}");
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        Debug.Log($"持续碰撞中！与物体: {collision.gameObject.name}, Tag: {collision.gameObject.tag}");
        
        // 检查碰撞的对象是否是玩家
        if (collision.gameObject.CompareTag("Player"))
        {
            // 调用GameManager的TakeDamage方法
            GameManager1.Instance.TakeDamage(damagePerSecond);
            Debug.Log($"对玩家造成伤害，伤害值: {damagePerSecond}");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"触发器被触发！与物体: {other.gameObject.name}, Tag: {other.gameObject.tag}");
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        Debug.Log($"触发器持续触发中！与物体: {other.gameObject.name}, Tag: {other.gameObject.tag}");
        
        // 如果使用触发器而不是碰撞器，也添加触发器检测
        if (other.CompareTag("Player"))
        {
            GameManager1.Instance.TakeDamage(damagePerSecond);
            Debug.Log($"触发器对玩家造成伤害，伤害值: {damagePerSecond}");
        }
    }
} 