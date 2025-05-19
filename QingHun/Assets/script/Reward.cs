// HealthReward.cs�������棩
using UnityEngine;

public class HealthReward : MonoBehaviour
{
    [Header("��������")]
    public int healAmount = 20;
    private float _moveSpeed;

    void Update()
    {
        transform.Translate(Vector2.left * _moveSpeed * Time.deltaTime);
    }

    public void Initialize(float speed)
    {
        _moveSpeed = speed;
        Debug.Log($"�����ٶ�: {_moveSpeed}");
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        Debug.Log("���������ɹ���"); // ����������־
        movement player = other.GetComponent<movement>();
        if (player != null)
        {
            player.Heal(healAmount);
            Debug.Log($"��һָ� {healAmount}HP");
        }
        Destroy(gameObject);
    }
}