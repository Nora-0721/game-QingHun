using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections;

public class MenuSystem : MonoBehaviour
{
    public static MenuSystem Instance { get; private set; }

    [Header("主菜单UI")]
    public GameObject mainMenuPanel;
    public Button startButton;
    public Button continueButton;
    public Button creditsButton;
    public Button quitButton;

    [Header("暂停菜单UI")]
    public GameObject pauseMenuPanel;
    public Button resumeButton;
    public Button returnToMenuButton;

    [Header("制作人员UI")]
    public GameObject creditsPanel;
    public Button backButton;

    [Header("过渡效果")]
    public GameObject fadePanel;
    public float fadeSpeed = 1.0f;
    private CanvasGroup fadePanelGroup;

    [Header("场景设置")]
    public string mainMenuSceneName = "MainMenu";
    public string firstLevelSceneName = "Level1";

    private const string LAST_SCENE_KEY = "LastPlayedScene";
    private bool isPaused = false;

    private void Awake()
    {
        // 单例模式初始化
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeUI();
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeUI()
    {
        // 初始化渐变面板
        if (fadePanel != null)
        {
            fadePanelGroup = fadePanel.GetComponent<CanvasGroup>();
            if (fadePanelGroup == null)
                fadePanelGroup = fadePanel.AddComponent<CanvasGroup>();

            fadePanelGroup.alpha = 0;
            fadePanelGroup.blocksRaycasts = false;
        }

        // 绑定按钮事件
        BindAllButtons();
    }

    private void BindAllButtons()
    {
        // 主菜单按钮
        if (startButton != null) startButton.onClick.AddListener(StartNewGame);
        if (continueButton != null) continueButton.onClick.AddListener(ContinueGame);
        if (creditsButton != null) creditsButton.onClick.AddListener(ShowCredits);
        if (quitButton != null) quitButton.onClick.AddListener(QuitGame);

        // 暂停菜单按钮
        if (resumeButton != null) resumeButton.onClick.AddListener(ResumeGame);
        if (returnToMenuButton != null) returnToMenuButton.onClick.AddListener(ReturnToMainMenu);

        // 制作人员返回按钮
        if (backButton != null) backButton.onClick.AddListener(HideCredits);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 重置游戏状态
        Time.timeScale = 1f;
        isPaused = false;

        // 根据场景类型设置UI
        if (scene.name == mainMenuSceneName)
        {
            ShowMainMenu();
            HidePauseMenu();
            HideCredits();

            // 检查是否有继续游戏存档
            continueButton.interactable = PlayerPrefs.HasKey(LAST_SCENE_KEY);
        }
        else
        {
            HideMainMenu();
            HidePauseMenu();
            HideCredits();
        }

        // 确保Canvas在最上层
        EnsureCanvasOnTop();
    }

    private void Update()
    {
        // ESC键控制暂停菜单
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (SceneManager.GetActiveScene().name != mainMenuSceneName)
            {
                TogglePauseMenu();
            }
        }
    }

    #region 菜单控制
    private void ShowMainMenu()
    {
        if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
    }

    private void HideMainMenu()
    {
        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
    }

    private void ShowPauseMenu()
    {
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(true);
            EnsureCanvasOnTop();
        }
    }

    private void HidePauseMenu()
    {
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
    }

    private void ShowCredits()
    {
        if (creditsPanel != null)
        {
            creditsPanel.SetActive(true);
            EnsureCanvasOnTop();
        }
    }

    private void HideCredits()
    {
        if (creditsPanel != null) creditsPanel.SetActive(false);
    }

    private void EnsureCanvasOnTop()
    {
        // 确保UI显示在最上层
        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas != null)
        {
            canvas.sortingOrder = 100;
            canvas.transform.SetAsLastSibling();
        }
    }
    #endregion

    #region 游戏流程控制
    public void TogglePauseMenu()
    {
        if (isPaused) ResumeGame();
        else PauseGame();
    }

    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;
        ShowPauseMenu();
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
        HidePauseMenu();
    }

    public void StartNewGame()
    {
        // 清除继续游戏存档
        PlayerPrefs.DeleteKey(LAST_SCENE_KEY);
        StartCoroutine(FadeAndLoadScene(firstLevelSceneName));
    }

    public void ContinueGame()
    {
        // 从存档加载最后玩的场景
        string lastScene = PlayerPrefs.GetString(LAST_SCENE_KEY, firstLevelSceneName);
        StartCoroutine(FadeAndLoadScene(lastScene));
    }

    public void ReturnToMainMenu()
    {
        // 保存当前场景
        SaveCurrentScene();
        StartCoroutine(FadeAndLoadScene(mainMenuSceneName));
    }

    private void SaveCurrentScene()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        if (currentScene != mainMenuSceneName)
        {
            PlayerPrefs.SetString(LAST_SCENE_KEY, currentScene);
            PlayerPrefs.Save();
        }
    }

    public void QuitGame()
    {
        SaveCurrentScene();
        StartCoroutine(FadeAndQuit());
    }
    #endregion

    #region 场景过渡
    private IEnumerator FadeAndLoadScene(string sceneName)
    {
        // 淡入
        if (fadePanelGroup != null)
        {
            fadePanelGroup.blocksRaycasts = true;
            float t = 0;
            while (t < 1f)
            {
                t += Time.deltaTime * fadeSpeed;
                fadePanelGroup.alpha = t;
                yield return null;
            }
        }

        // 加载场景
        SceneManager.LoadScene(sceneName);

        // 淡出
        if (fadePanelGroup != null)
        {
            float t = 1f;
            while (t > 0f)
            {
                t -= Time.deltaTime * fadeSpeed;
                fadePanelGroup.alpha = t;
                yield return null;
            }
            fadePanelGroup.blocksRaycasts = false;
        }
    }

    private IEnumerator FadeAndQuit()
    {
        // 淡入
        if (fadePanelGroup != null)
        {
            fadePanelGroup.blocksRaycasts = true;
            float t = 0;
            while (t < 1f)
            {
                t += Time.deltaTime * fadeSpeed;
                fadePanelGroup.alpha = t;
                yield return null;
            }
        }

        // 退出游戏
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
    #endregion
}