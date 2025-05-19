using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class movement : MonoBehaviour
{
    // 移动控制参数
    [Header("移动设置")]
    public float runSpeed = 5f;
    public float targetY = 0.36f;
    public float topY = 3.45f;
    public float bottomY = -2.66f;
    public float middleY = 0.36f;
    public float smoothTime = 0.15f;

    // 生命值系统
    [Header("血量设置")]
    public float maxHealth = 100f;
    public float contactDamage = 20f;    // 每秒伤害量
    public float damageInterval = 0.1f;  // 伤害间隔
    public Slider healthSlider;

    // 得分系统
    [Header("得分设置")]
    public Text scoreText;

    // 私有变量
    private Rigidbody2D rb;
    private Vector2 velocity = Vector2.zero;
    private float currentHealth;
    private int score;
    private float damageTimer;
    private bool isTakingDamage;
    private HashSet<GameObject> contactedObstacles = new HashSet<GameObject>();

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        LockPositionConstraints();
        InitializeGameState();
    }

    void Update()
    {
        HandleInput();
        HandleContinuousDamage();
    }

    void FixedUpdate()
    {
        SmoothMoveToTarget();
    }

    //============== 核心功能方法 ==============//
    private void InitializeGameState()
    {
        currentHealth = maxHealth;
        score = 0;
        UpdateHealthUI();
        UpdateScoreUI();
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            MoveUp();
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            MoveDown();
    }

    private void HandleContinuousDamage()
    {
        if (!isTakingDamage) return;

        damageTimer += Time.deltaTime;
        if (damageTimer >= damageInterval)
        {
            ApplyDamage(contactDamage * damageInterval);
            damageTimer = 0f;
        }
    }

    //============== 移动控制 ==============//
    private void MoveUp()
    {
        targetY = targetY switch
        {
            _ when Mathf.Approximately(targetY, middleY) => topY,
            _ when Mathf.Approximately(targetY, bottomY) => middleY,
            _ => targetY
        };
    }

    private void MoveDown()
    {
        targetY = targetY switch
        {
            _ when Mathf.Approximately(targetY, middleY) => bottomY,
            _ when Mathf.Approximately(targetY, topY) => middleY,
            _ => targetY
        };
    }

    private void SmoothMoveToTarget()
    {
        Vector2 targetPos = new Vector2(rb.position.x, targetY);
        rb.MovePosition(Vector2.SmoothDamp(rb.position, targetPos, ref velocity, smoothTime));
    }

    //============== 碰撞检测 ==============//
    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Obstacle")) return;

        contactedObstacles.Add(other.gameObject);
        if (!isTakingDamage) StartDamageProcess();
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Obstacle")) return;

        contactedObstacles.Remove(other.gameObject);
        if (contactedObstacles.Count == 0)
            StopDamageProcess();
    }

    //============== 游戏状态管理 ==============//
    private void StartDamageProcess()
    {
        isTakingDamage = true;
        damageTimer = damageInterval; // 立即触发第一次伤害
    }

    private void StopDamageProcess() => isTakingDamage = false;

    private void ApplyDamage(float damage)
    {
        currentHealth = Mathf.Max(currentHealth - damage, 0);
        UpdateHealthUI();

        if (currentHealth <= 0)
            TriggerGameOver();
    }

    public void AddScore(int points)
    {
        score += points;
        UpdateScoreUI();
    }

    //============== UI更新 ==============//
    private void UpdateHealthUI()
    {
        if (healthSlider != null)
            healthSlider.value = currentHealth / maxHealth;
#if UNITY_EDITOR
        else
            Debug.LogWarning("Health Slider 未绑定！");
#endif
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
            scoreText.text = $"SCORE: {score}";
#if UNITY_EDITOR
        else
            Debug.LogWarning("Score Text 未绑定！");
#endif
    }

    //============== 工具方法 ==============//
    private void LockPositionConstraints()
    {
        rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
    }

    private void TriggerGameOver()
    {
        GameManager1 gameManager = FindObjectOfType<GameManager1>();
        if (gameManager != null)
            gameManager.GameOver();
        else
            Debug.LogError("未找到 GameManager 实例！");
    }
    [Header("奖励效果")]
    public float speedBoostMultiplier = 1.5f;
    public GameObject shieldEffect;

    private float originalSpeed;
    private bool isShielded;

    public void ActivateShield(float duration)
    {
        if (!isShielded)
        {
            shieldEffect.SetActive(true);
            isShielded = true;
            Invoke(nameof(DeactivateShield), duration);
        }
    }

    public void BoostSpeed(float duration)
    {
        originalSpeed = runSpeed;
        runSpeed *= speedBoostMultiplier;
        Invoke(nameof(ResetSpeed), duration);
    }

    private void DeactivateShield()
    {
        shieldEffect.SetActive(false);
        isShielded = false;
    }

    private void ResetSpeed() => runSpeed = originalSpeed;
    public void Heal(float amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        UpdateHealthUI();
        Debug.Log($"血量恢复：+{amount} HP");
        GameManager1.Instance.AddHealth(amount);
    }
}