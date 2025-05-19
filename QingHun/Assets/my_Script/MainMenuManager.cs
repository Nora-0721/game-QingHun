using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

public class MainMenuManager : MonoBehaviour
{
    [Header("主菜单按钮")]
    public Button startGameButton;      // 开始游戏按钮
    public Button continueGameButton;   // 继续游戏按钮
    public Button teamMembersButton;    // 团队成员按钮
    public Button quitGameButton;       // 退出游戏按钮
    public Button backButton;           // 返回按钮（团队成员界面）

    [Header("主菜单UI")]
    public GameObject mainMenuPanel;    // 主菜单面板
    public GameObject teamMembersPanel; // 团队成员面板
    public GameObject fadePanel;        // 渐变过渡面板
    public Image backgroundImage;       // 背景图片

    [Header("场景设置")]
    public string firstLevelScene = "Level1"; // 第一个关卡场景名
    public float fadeSpeed = 1.0f;            // 渐变速度

    // 场景存储键名
    private const string LAST_SCENE_KEY = "LastPlayedScene";
    
    // 渐变组件
    private CanvasGroup fadePanelGroup;

    private void Start()
    {
        // 初始化
        InitializeMenu();
        
        // 设置按钮事件
        SetupButtons();
        
        // 显示主菜单
        ShowMainMenu();
    }

    private void InitializeMenu()
    {
        // 初始化渐变面板
        if (fadePanel != null)
        {
            fadePanelGroup = fadePanel.GetComponent<CanvasGroup>();
            if (fadePanelGroup == null)
                fadePanelGroup = fadePanel.gameObject.AddComponent<CanvasGroup>();
            
            // 初始不可见
            fadePanelGroup.alpha = 0;
            fadePanelGroup.blocksRaycasts = false;
        }
        
        // 检查是否有保存的场景
        if (!PlayerPrefs.HasKey(LAST_SCENE_KEY))
        {
            // 没有保存的场景，隐藏继续按钮
            if (continueGameButton != null)
                continueGameButton.gameObject.SetActive(false);
        }
        
        // 隐藏团队成员面板
        if (teamMembersPanel != null)
            teamMembersPanel.SetActive(false);
    }

    private void SetupButtons()
    {
        // 设置开始游戏按钮
        if (startGameButton != null)
            startGameButton.onClick.AddListener(StartNewGame);
        
        // 设置继续游戏按钮
        if (continueGameButton != null)
            continueGameButton.onClick.AddListener(ContinueGame);
        
        // 设置团队成员按钮
        if (teamMembersButton != null)
            teamMembersButton.onClick.AddListener(ShowTeamMembers);
        
        // 设置退出游戏按钮
        if (quitGameButton != null)
            quitGameButton.onClick.AddListener(QuitGame);
        
        // 设置返回按钮
        if (backButton != null)
            backButton.onClick.AddListener(ShowMainMenu);
    }

    public void StartNewGame()
    {
        // 存储第一关为最后游玩场景
        PlayerPrefs.SetString(LAST_SCENE_KEY, firstLevelScene);
        PlayerPrefs.Save();
        
        // 开始淡出效果并加载第一个场景
        StartCoroutine(FadeAndLoadScene(firstLevelScene));
    }

    public void ContinueGame()
    {
        // 获取上次游玩的场景
        string lastScene = PlayerPrefs.GetString(LAST_SCENE_KEY, firstLevelScene);
        
        // 开始淡出效果并加载上次的场景
        StartCoroutine(FadeAndLoadScene(lastScene));
    }

    public void ShowTeamMembers()
    {
        // 隐藏主菜单，显示团队成员页面
        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(false);
        
        if (teamMembersPanel != null)
            teamMembersPanel.SetActive(true);
    }

    public void ShowMainMenu()
    {
        // 隐藏团队成员页面，显示主菜单
        if (teamMembersPanel != null)
            teamMembersPanel.SetActive(false);
        
        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(true);
    }

    public void QuitGame()
    {
        // 开始淡出效果并退出游戏
        StartCoroutine(FadeAndQuit());
    }

    private IEnumerator FadeAndLoadScene(string sceneName)
    {
        // 确保全局UI管理器不会中断过渡
        Time.timeScale = 1.0f;
        
        // 开始渐变效果
        if (fadePanelGroup != null)
        {
            fadePanelGroup.blocksRaycasts = true;
            
            // 淡入黑色
            float t = 0;
            while (t < 1.0f)
            {
                t += Time.deltaTime * fadeSpeed;
                fadePanelGroup.alpha = Mathf.Clamp01(t);
                yield return null;
            }
        }
        
        // 加载场景
        SceneManager.LoadScene(sceneName);
    }

    private IEnumerator FadeAndQuit()
    {
        // 开始渐变效果
        if (fadePanelGroup != null)
        {
            fadePanelGroup.blocksRaycasts = true;
            
            // 淡入黑色
            float t = 0;
            while (t < 1.0f)
            {
                t += Time.deltaTime * fadeSpeed;
                fadePanelGroup.alpha = Mathf.Clamp01(t);
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
} 