//using UnityEngine;
//using UnityEngine.UI;
//using UnityEngine.EventSystems;
//using UnityEngine.SceneManagement;
//using TMPro;

//public class PauseMenu : MonoBehaviour
//{
//    [Header("菜单面板")]
//    public GameObject pauseMenuPanel;  // 整个暂停菜单面板
//    public GameObject creditsPanel;    // 鸣谢面板

//    [Header("按钮引用")]
//    public Button continueButton;
//    public Button returnToMenuButton;
//    public Button creditsButton;
//    public Button quitButton;
//    public Button backButton;

//    [Header("按钮效果")]
//    public float hoverScale = 1.05f;   // 轻微放大效果
//    public float pressedScale = 0.98f; // 轻微缩小效果

//    [Header("颜色效果")]
//    public Color normalColor = Color.white;
//    public Color hoverColor = new Color(0.95f, 0.95f, 0.95f, 1f);
//    public Color pressedColor = new Color(0.9f, 0.9f, 0.9f, 1f);

//    [Header("音效")]
//    public AudioClip hoverSound;
//    public AudioClip clickSound;
//    private AudioSource audioSource;

//    // 场景保存键名
//    private const string LAST_SCENE_KEY = "LastPlayedScene";
    
//    // Canvas组件引用
//    private Canvas menuCanvas;
//    private GraphicRaycaster menuRaycaster;

//    private void Awake()
//    {
//        // 初始化Canvas组件
//        InitializeCanvas();
        
//        // 确保在游戏开始时菜单面板是隐藏的
//        HidePauseMenu();
//        HideCreditsPanel();
//    }

//    private void Start()
//    {
//        // 添加音效组件
//        audioSource = gameObject.AddComponent<AudioSource>();
//        audioSource.playOnAwake = false;
//        audioSource.volume = 0.3f; // 降低音量避免刺耳

//        // 设置按钮事件
//        SetupButton(continueButton, ContinueGame);
//        SetupButton(returnToMenuButton, ReturnToMainMenu);
//        SetupButton(creditsButton, ShowCredits);
//        SetupButton(quitButton, QuitGame);
//        SetupButton(backButton, HideCredits);
        
//        // 注册场景加载事件
//        SceneManager.sceneLoaded += OnSceneLoaded;
//    }
    
//    private void OnDestroy()
//    {
//        // 取消注册事件，防止内存泄漏
//        SceneManager.sceneLoaded -= OnSceneLoaded;
//    }
    
//    // 初始化Canvas组件，确保菜单显示在最上层
//    private void InitializeCanvas()
//    {
//        // 获取Canvas组件
//        menuCanvas = GetComponentInParent<Canvas>();
//        if (menuCanvas == null)
//        {
//            // 如果父级没有Canvas，查找当前对象
//            menuCanvas = GetComponent<Canvas>();
//        }
        
//        // 如果找到Canvas
//        if (menuCanvas != null)
//        {
//            // 确保Canvas是屏幕空间-覆盖模式
//            menuCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            
//            // 设置为最高排序顺序
//            menuCanvas.sortingOrder = 100;
            
//            // 获取GraphicRaycaster组件用于交互
//            menuRaycaster = menuCanvas.GetComponent<GraphicRaycaster>();
//            if (menuRaycaster == null)
//            {
//                menuRaycaster = menuCanvas.gameObject.AddComponent<GraphicRaycaster>();
//            }
//        }
//        else
//        {
//            Debug.LogWarning("未找到Canvas组件，菜单可能无法正确显示在最上层。");
//        }
//    }
    
//    // 场景加载时调用，确保菜单被隐藏
//    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
//    {
//        // 重新初始化Canvas，确保在新场景中也能显示在最上层
//        InitializeCanvas();
        
//        // 隐藏所有菜单，无论是什么场景
//        HidePauseMenu();
        
//        // 确保时间正常流动
//        Time.timeScale = 1f;
        
//        // 重置GameSystemManager的暂停状态
//        if (GameSystemManager.Instance != null)
//        {
//            GameSystemManager.Instance.ResumeGame();
//        }
//    }
    
//    // 隐藏暂停菜单
//    private void HidePauseMenu()
//    {
//        if (pauseMenuPanel != null)
//            pauseMenuPanel.SetActive(false);
//    }
    
//    // 显示暂停菜单
//    private void ShowPauseMenu()
//    {
//        if (pauseMenuPanel != null)
//        {
//            pauseMenuPanel.SetActive(true);
//            EnsureMenuOnTop();
//        }
//    }
    
//    // 隐藏鸣谢面板
//    private void HideCreditsPanel()
//    {
//        if (creditsPanel != null)
//            creditsPanel.SetActive(false);
//    }
    
//    // 显示鸣谢面板
//    private void ShowCreditsPanel()
//    {
//        if (creditsPanel != null)
//        {
//            creditsPanel.SetActive(true);
//            EnsureMenuOnTop();
//        }
//    }
    
//    // 确保菜单显示在最上层
//    private void EnsureMenuOnTop()
//    {
//        // 将菜单的Canvas置于最顶层
//        if (menuCanvas != null)
//        {
//            // 设置为最高排序顺序
//            menuCanvas.sortingOrder = 100;
            
//            // 确保所有子UI元素都在最顶层
//            foreach (Transform child in menuCanvas.transform)
//            {
//                Canvas childCanvas = child.GetComponent<Canvas>();
//                if (childCanvas != null)
//                {
//                    childCanvas.overrideSorting = true;
//                    childCanvas.sortingOrder = 101; // 比父级高一层
//                }
//            }
            
//            // 为了确保最新的交互状态，让Canvas刷新一下
//            menuCanvas.enabled = false;
//            menuCanvas.enabled = true;
            
//            // 确保有射线检测组件
//            if (menuRaycaster != null)
//            {
//                menuRaycaster.enabled = true;
//            }
//        }
        
//        // 确保菜单在其他UI元素之上
//        if (pauseMenuPanel != null && pauseMenuPanel.activeInHierarchy)
//        {
//            pauseMenuPanel.transform.SetAsLastSibling();
//        }
//        if (creditsPanel != null && creditsPanel.activeInHierarchy)
//        {
//            creditsPanel.transform.SetAsLastSibling();
//        }
//    }

//    private void SetupButton(Button button, UnityEngine.Events.UnityAction action)
//    {
//        if (button == null) return;

//        // 清除旧的事件监听
//        button.onClick.RemoveAllListeners();

//        // 添加点击事件
//        button.onClick.AddListener(action);
//        button.onClick.AddListener(PlayClickSound);

//        // 获取按钮图像和文本组件
//        Image buttonImage = button.GetComponent<Image>();
//        TextMeshProUGUI buttonText = button.GetComponentInChildren<TextMeshProUGUI>();
//        RectTransform buttonRect = button.GetComponent<RectTransform>();
//        Vector3 originalScale = buttonRect.localScale;

//        // 添加事件触发器
//        EventTrigger trigger = button.gameObject.GetComponent<EventTrigger>();
//        if (trigger == null) trigger = button.gameObject.AddComponent<EventTrigger>();
//        trigger.triggers.Clear();

//        // 鼠标进入效果 - 改变颜色和缩放
//        AddEventTrigger(trigger, EventTriggerType.PointerEnter, (data) => {
//            if (buttonImage != null) buttonImage.color = hoverColor;
//            if (buttonText != null) buttonText.color = hoverColor;
//            if (buttonRect != null) buttonRect.localScale = originalScale * hoverScale;
//            PlayHoverSound();
//        });

//        // 鼠标退出效果 - 恢复颜色和大小
//        AddEventTrigger(trigger, EventTriggerType.PointerExit, (data) => {
//            if (buttonImage != null) buttonImage.color = normalColor;
//            if (buttonText != null) buttonText.color = normalColor;
//            if (buttonRect != null) buttonRect.localScale = originalScale;
//        });

//        // 鼠标按下效果 - 颜色和缩放变化
//        AddEventTrigger(trigger, EventTriggerType.PointerDown, (data) => {
//            if (buttonImage != null) buttonImage.color = pressedColor;
//            if (buttonText != null) buttonText.color = pressedColor;
//            if (buttonRect != null) buttonRect.localScale = originalScale * pressedScale;
//        });

//        // 鼠标抬起效果 - 恢复悬停状态
//        AddEventTrigger(trigger, EventTriggerType.PointerUp, (data) => {
//            if (buttonImage != null) buttonImage.color = hoverColor;
//            if (buttonText != null) buttonText.color = hoverColor;
//            if (buttonRect != null) buttonRect.localScale = originalScale * hoverScale;
//        });
//    }

//    // 辅助方法：添加事件触发器
//    private void AddEventTrigger(EventTrigger trigger, EventTriggerType type, UnityEngine.Events.UnityAction<BaseEventData> action)
//    {
//        EventTrigger.Entry entry = new EventTrigger.Entry();
//        entry.eventID = type;
//        entry.callback.AddListener(action);
//        trigger.triggers.Add(entry);
//    }

//    // 播放悬停音效
//    private void PlayHoverSound()
//    {
//        if (hoverSound != null && audioSource != null)
//        {
//            audioSource.PlayOneShot(hoverSound);
//        }
//    }

//    // 播放点击音效
//    private void PlayClickSound()
//    {
//        if (clickSound != null && audioSource != null)
//        {
//            audioSource.PlayOneShot(clickSound);
//        }
//    }

//    // 继续游戏
//    public void ContinueGame() => GameSystemManager.Instance.ResumeGame();
    
//    // 返回主菜单并保存当前场景
//    public void ReturnToMainMenu() 
//    {
//        // 保存当前场景，以便下次继续
//        SaveCurrentScene();
        
//        // 隐藏所有菜单
//        HidePauseMenu();
//        HideCreditsPanel();
        
//        // 确保时间正常流动
//        Time.timeScale = 1f;
        
//        // 返回主菜单
//        GameSystemManager.Instance.ReturnToMainMenu();
//    }
    
//    // 显示制作人员名单
//    public void ShowCredits()
//    {
//        HidePauseMenu();
//        ShowCreditsPanel();
//        GameSystemManager.Instance.ShowCredits();
//    }
    
//    // 隐藏制作人员名单
//    public void HideCredits()
//    {
//        HideCreditsPanel();
//        ShowPauseMenu();
//        GameSystemManager.Instance.HideCredits();
//    }
    
//    // 退出游戏前保存当前场景
//    public void QuitGame() 
//    {
//        // 保存当前场景
//        SaveCurrentScene();
        
//        // 隐藏所有菜单
//        HidePauseMenu();
//        HideCreditsPanel();
        
//        // 确保时间正常流动
//        Time.timeScale = 1f;
        
//        // 退出游戏
//        GameSystemManager.Instance.QuitGame();
//    }
    
//    // 保存当前场景
//    private void SaveCurrentScene()
//    {
//        // 获取当前场景名称
//        string currentScene = SceneManager.GetActiveScene().name;
        
//        // 保存到PlayerPrefs
//        PlayerPrefs.SetString(LAST_SCENE_KEY, currentScene);
//        PlayerPrefs.Save();
//    }
//}