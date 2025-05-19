using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections.Generic;

public class GlobalUIManager : MonoBehaviour
{
    public static GlobalUIManager Instance { get; private set; }

    [Header("按钮效果设置")]
    public float hoverScale = 1.05f;   // 轻微放大效果
    public float pressedScale = 0.98f; // 轻微缩小效果
    public Color normalColor = Color.white;
    public Color hoverColor = new Color(0.95f, 0.95f, 0.95f, 1f);
    public Color pressedColor = new Color(0.9f, 0.9f, 0.9f, 1f);

    [Header("音效")]
    public AudioClip hoverSound;
    public AudioClip clickSound;
    private AudioSource audioSource;

    [Header("暂停菜单")]
    public GameObject pauseMenuUI;
    public GameObject creditsUI;
    public string mainMenuSceneName = "MainMenu";

    private bool isPaused = false;
    private bool isShowingCredits = false;
    private Dictionary<Button, Vector3> originalScales = new Dictionary<Button, Vector3>();

    private void Awake()
    {
        // 单例模式
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
            SetupAudioSource();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void SetupAudioSource()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.volume = 0.3f;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 在新场景加载后自动查找并设置所有按钮
        FindAndSetupAllButtons();
        
        // 初始化UI状态
        if (pauseMenuUI != null) pauseMenuUI.SetActive(false);
        if (creditsUI != null) creditsUI.SetActive(false);
        
        // 确保游戏不是暂停状态
        if (Time.timeScale != 1.0f) Time.timeScale = 1.0f;
        isPaused = false;
        isShowingCredits = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isShowingCredits) HideCredits();
            else TogglePause();
        }
    }

    // 查找场景中所有按钮并应用效果
    public void FindAndSetupAllButtons()
    {
        Button[] allButtons = FindObjectsOfType<Button>();
        foreach (Button button in allButtons)
        {
            SetupButtonEffects(button);
        }
    }

    // 为单个按钮应用效果
    public void SetupButtonEffects(Button button)
    {
        if (button == null) return;

        // 获取按钮组件
        Image buttonImage = button.GetComponent<Image>();
        TextMeshProUGUI buttonText = button.GetComponentInChildren<TextMeshProUGUI>();
        RectTransform buttonRect = button.GetComponent<RectTransform>();
        
        // 保存原始缩放
        if (!originalScales.ContainsKey(button))
        {
            originalScales[button] = buttonRect.localScale;
        }
        Vector3 originalScale = originalScales[button];

        // 添加事件触发器
        EventTrigger trigger = button.gameObject.GetComponent<EventTrigger>();
        if (trigger == null) trigger = button.gameObject.AddComponent<EventTrigger>();
        
        // 清除旧的触发器避免重复
        trigger.triggers.Clear();

        // 鼠标进入效果 - 改变颜色和缩放
        AddEventTrigger(trigger, EventTriggerType.PointerEnter, (data) => {
            if (buttonImage != null) buttonImage.color = hoverColor;
            if (buttonText != null) buttonText.color = hoverColor;
            if (buttonRect != null) buttonRect.localScale = originalScale * hoverScale;
            PlayHoverSound();
        });

        // 鼠标退出效果 - 恢复颜色和大小
        AddEventTrigger(trigger, EventTriggerType.PointerExit, (data) => {
            if (buttonImage != null) buttonImage.color = normalColor;
            if (buttonText != null) buttonText.color = normalColor;
            if (buttonRect != null) buttonRect.localScale = originalScale;
        });

        // 鼠标按下效果 - 颜色和缩放变化
        AddEventTrigger(trigger, EventTriggerType.PointerDown, (data) => {
            if (buttonImage != null) buttonImage.color = pressedColor;
            if (buttonText != null) buttonText.color = pressedColor;
            if (buttonRect != null) buttonRect.localScale = originalScale * pressedScale;
        });

        // 鼠标抬起效果 - 恢复悬停状态
        AddEventTrigger(trigger, EventTriggerType.PointerUp, (data) => {
            if (buttonImage != null) buttonImage.color = hoverColor;
            if (buttonText != null) buttonText.color = hoverColor;
            if (buttonRect != null) buttonRect.localScale = originalScale * hoverScale;
        });
    }

    // 辅助方法：添加事件触发器
    private void AddEventTrigger(EventTrigger trigger, EventTriggerType type, UnityEngine.Events.UnityAction<BaseEventData> action)
    {
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = type;
        entry.callback.AddListener(action);
        trigger.triggers.Add(entry);
    }

    // 播放悬停音效
    private void PlayHoverSound()
    {
        if (hoverSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(hoverSound);
        }
    }

    // 播放点击音效
    public void PlayClickSound()
    {
        if (clickSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(clickSound);
        }
    }

    #region 暂停功能
    public void TogglePause()
    {
        if (isPaused) ResumeGame();
        else PauseGame();
    }

    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;
        if (pauseMenuUI != null) pauseMenuUI.SetActive(true);
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
        if (pauseMenuUI != null) pauseMenuUI.SetActive(false);
        if (creditsUI != null) creditsUI.SetActive(false);
    }
    #endregion

    #region 菜单功能
    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }

    public void ShowCredits()
    {
        isShowingCredits = true;
        if (pauseMenuUI != null) pauseMenuUI.SetActive(false);
        if (creditsUI != null) creditsUI.SetActive(true);
    }

    public void HideCredits()
    {
        isShowingCredits = false;
        if (creditsUI != null) creditsUI.SetActive(false);
        if (pauseMenuUI != null && isPaused) pauseMenuUI.SetActive(true);
    }
    #endregion

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
} 