using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SimpleMainMenu : MonoBehaviour
{
    [Header("主菜单按钮")]
    public Button startGameButton;     // 开始游戏按钮
    public Button continueGameButton;  // 继续游戏按钮
    public Button teamButton;          // 团队成员按钮
    public Button quitButton;          // 退出游戏按钮
    public Button returnToMenuButton;  // 返回封面按钮

    [Header("团队成员")]
    public GameObject teamPanel;       // 团队成员面板
    public GameObject playPanel;       // 团队成员面板

    [Header("场景设置")]
    public string firstLevelScene = "Level1";  // 第一个场景名称
    public string firstLevelScene2 = "Level1";  // 第一个场景名称

    // 场景记录键名
    private const string LAST_SCENE_KEY = "LastPlayedScene";

    private void Start()
    {
        // 初始化UI状态
        InitializeUI();

        // 设置按钮事件
        SetupButtons();

        // 添加按钮效果
        AddButtonEffects();
    }

    private void InitializeUI()
    {
        // 隐藏团队成员面板
        if (teamPanel != null)
            teamPanel.SetActive(false);
        // 隐藏团队成员面板
        if (playPanel != null)
            playPanel.SetActive(false);

        // 检查是否有保存的场景
        if (!PlayerPrefs.HasKey(LAST_SCENE_KEY))
        {
            // 没有保存的场景，隐藏继续按钮
            if (continueGameButton != null)
                continueGameButton.gameObject.SetActive(false);
        }
    }

    private void SetupButtons()
    {
        // 开始新游戏
        if (startGameButton != null)
            startGameButton.onClick.AddListener(StartNewGame);

        // 继续游戏
        if (continueGameButton != null)
            continueGameButton.onClick.AddListener(ContinueGame);

        // 团队成员
        if (teamButton != null)
            teamButton.onClick.AddListener(ShowTeamPanel);

        // 退出游戏
        if (quitButton != null)
            quitButton.onClick.AddListener(QuitGame);

        // 返回封面
        if (returnToMenuButton != null)
            returnToMenuButton.onClick.AddListener(ReturnToMainMenu);
    }

    private void AddButtonEffects()
    {
        // 为所有按钮添加放大效果
        AddButtonEffect(startGameButton);
        AddButtonEffect(continueGameButton);
        AddButtonEffect(teamButton);
        AddButtonEffect(quitButton);
        AddButtonEffect(returnToMenuButton);
    }

    private void AddButtonEffect(Button button)
    {
        if (button != null && !button.GetComponent<SimpleButtonEffect>())
        {
            button.gameObject.AddComponent<SimpleButtonEffect>();
        }
    }

    // 开始新游戏
    public void StartNewGame()
    {
        // 保存第一关为最后游玩场景
        PlayerPrefs.SetString(LAST_SCENE_KEY, firstLevelScene);
        PlayerPrefs.Save();

        // 加载第一个场景
        SceneManager.LoadScene(firstLevelScene);
    }

    // 继续游戏
    public void ContinueGame()
    {
        if (playPanel != null)
            playPanel.SetActive(true);
    }

    // 显示团队成员面板
    public void ShowTeamPanel()
    {
        if (teamPanel != null)
            teamPanel.SetActive(true);
    }

    // 隐藏团队成员面板
    public void HideTeamPanel()
    {
        if (teamPanel != null)
            teamPanel.SetActive(false);
    }

    // 退出游戏
    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    // 返回封面
    public void ReturnToMainMenu()
    {
        // 保存当前场景，以便下次继续
        SaveCurrentScene();

        // 加载封面场景
        SceneManager.LoadScene("FFFFirst");
    }

    // 保存当前场景
    private void SaveCurrentScene()
    {
        // 获取当前场景名称
        string currentScene = SceneManager.GetActiveScene().name;

        // 保存到PlayerPrefs
        PlayerPrefs.SetString(LAST_SCENE_KEY, currentScene);
        PlayerPrefs.Save();
    }
}