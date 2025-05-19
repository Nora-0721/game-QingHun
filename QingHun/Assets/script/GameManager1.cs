using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

public class GameManager1 : MonoBehaviour
{
    [Header("游戏核心参数")]
    [Tooltip("游戏时长（秒）")]
    [SerializeField] private float gameDuration = 30f;
    [SerializeField] private float gameOverDelay = 2f; // 游戏结束后重启延迟

    [Header("血量系统")]
    [SerializeField] private float maxHealth = 80f;
    [SerializeField] private float damagePerSecond = 20f;  // 每秒扣血量

    [Header("UI组件")]
    [SerializeField] private Slider healthSlider;  // 血条
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private GameObject gameOverUI;
    [SerializeField] private GameObject victoryUI;
    [SerializeField] private TextMeshProUGUI gameOverText;  // 游戏结束文本

    private float _currentHealth;
    private float currentTime;
    private bool isGameOver;
    private bool isVictory;
    private bool needUpdateHealthBar = false;

    // 静态事件用于通知其他组件游戏状态改变
    public static event System.Action OnGameOver;
    public static event System.Action OnGameRestart;

    // 属性用于管理血量变化
    private float CurrentHealth
    {
        get => _currentHealth;
        set
        {
            _currentHealth = Mathf.Clamp(value, 0, maxHealth);
            UpdateHealthUI();
        }
    }

    public static GameManager1 Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }

    void Start()
    {
        InitializeGameState();
        SetupUIComponents();

        // 确保在所有组件设置完成后初始化血量
        CurrentHealth = maxHealth;
    }

    void InitializeGameState()
    {
        Time.timeScale = 1f;
        isGameOver = false;
        isVictory = false;
        currentTime = 0;

        InitializeHealthBar();
        SetUIActiveState(false);
    }

    private void InitializeHealthBar()
    {
        if (healthSlider != null)
        {
            healthSlider.minValue = 0;
            healthSlider.maxValue = maxHealth;
            Debug.Log($"初始化血条 - 最大血量: {maxHealth}");
        }
        else
        {
            Debug.LogError("血条组件未找到！");
        }
    }

    private void UpdateHealthUI()
    {
        if (healthSlider != null)
        {
            healthSlider.value = _currentHealth;
            Debug.Log($"更新血条UI - 当前血量: {_currentHealth}, Slider值: {healthSlider.value}");
        }
    }

    void SetupUIComponents()
    {
        // 尝试通过Canvas查找Timer Text
        if (timerText == null)
        {
            Canvas canvas = FindObjectOfType<Canvas>();
            if (canvas != null)
            {
                Transform timerTransform = canvas.transform.Find("TimerText");
                if (timerTransform != null)
                {
                    timerText = timerTransform.GetComponent<TextMeshProUGUI>();
                }
            }

            if (timerText == null)
            {
                Debug.LogWarning("无法找到TimerText组件，请确保Canvas下有名为'TimerText'的TextMeshProUGUI组件");
            }
        }

        // 尝试通过Canvas查找Health Slider
        if (healthSlider == null)
        {
            Canvas canvas = FindObjectOfType<Canvas>();
            if (canvas != null)
            {
                Transform sliderTransform = canvas.transform.Find("Slider");
                if (sliderTransform != null)
                {
                    healthSlider = sliderTransform.GetComponent<Slider>();
                }
            }

            if (healthSlider == null)
            {
                Debug.LogWarning("无法找到HealthSlider组件，请确保Canvas下有名为'Slider'的Slider组件");
            }
        }
    }

    public bool IsVictory => isVictory;
    public bool IsGameOver => isGameOver;
    void LateUpdate()
    {
        if (needUpdateHealthBar)
        {
            if (healthSlider != null)
            {
                healthSlider.value = CurrentHealth;
                Debug.Log($"LateUpdate中更新血条 - 当前血量: {CurrentHealth}, Slider值: {healthSlider.value}");
            }
            needUpdateHealthBar = false;
        }
    }

    void Update()
    {
        if (isGameOver || isVictory) return;

        UpdateGameTimer();

        // 确保血条显示正确的血量
        UpdateHealthBar();
    }

    private void UpdateHealthBar()
    {
        if (healthSlider != null)
        {
            healthSlider.value = CurrentHealth;
            Debug.Log($"更新血条 - 当前血量: {CurrentHealth}, Slider值: {healthSlider.value}");
        }
    }

    void UpdateGameTimer()
    {
        currentTime += Time.deltaTime;

        if (timerText != null)
        {
            timerText.text = $"剩余时间: {Mathf.CeilToInt(gameDuration - currentTime)}";
        }

        if (currentTime >= gameDuration && CurrentHealth > 0)
        {
            TriggerVictory();
        }
    }

    public void TakeDamage(float damage)
    {
        if (isGameOver || isVictory) return;

        float damageAmount = damage * Time.deltaTime;
        float newHealth = CurrentHealth - damageAmount;

        PlayerAppearance player = FindObjectOfType<PlayerAppearance>();
        if (player != null)
        {
            player.TriggerDamageEffect();
        }
        // 使用更严格的浮点数比较
        if (newHealth <= Mathf.Epsilon || Mathf.Approximately(newHealth, 0f))
        {
            CurrentHealth = 0;
            GameOver();
        }
        else
        {
            CurrentHealth = newHealth;
        }

        Debug.Log($"扣除伤害: {damageAmount}, 当前血量: {CurrentHealth}"); // 添加日志监控
    }


    public void GameOver()
    {
        if (isGameOver) return;

        isGameOver = true;

        // 显示失败UI
        SetUIActiveState(true, isGameOver: true);

        // 调用场景切换
        SceneTransitionManager1.Instance.LoadSceneWithTransition("cuoshi1");

        // 可选：是否需要暂停游戏？
        // Time.timeScale = 0f; 
    }

    IEnumerator RestartAfterDelay(float delay)
    {
        // 使用 unscaledTime 来计时，因为游戏已经暂停
        yield return new WaitForSecondsRealtime(delay);
        RestartGame();
    }

    void TriggerVictory()
    {
        if (isGameOver || isVictory) return;

        isVictory = true;
        SetUIActiveState(true, isVictory: true);
        SceneTransitionManager1.Instance.LoadSceneWithTransition("pochu1");
    }

    private void LoadNextScene()
    {
        // 使用场景名称更安全（需提前配置）
        SceneManager.LoadScene("pochu");

        // 或使用场景索引
        // SceneManager.LoadScene(1);
    }
    void SetUIActiveState(bool isActive, bool isVictory = false, bool isGameOver = false)
    {
        if (victoryUI != null) victoryUI.SetActive(isActive && isVictory);
        if (gameOverUI != null) gameOverUI.SetActive(isActive && isGameOver);
    }

    public void RestartGame()
    {
        // 恢复正常时间流逝
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void AddHealth(float healAmount)
    {
        if (isGameOver || isVictory) return;

        float oldHealth = CurrentHealth;
        CurrentHealth += healAmount;

        Debug.Log($"恢复血量 - 恢复值: {healAmount}, 原血量: {oldHealth}, 现血量: {CurrentHealth}");
    }
}