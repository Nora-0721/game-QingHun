//using UnityEngine;
//using UnityEngine.SceneManagement;

///// <summary>
///// 全局菜单隐藏管理器：确保所有场景加载时暂停菜单都是隐藏的
///// 添加到游戏中持久存在的对象上，如GameSystemManager同级
///// </summary>
//public class MenuHideManager : MonoBehaviour
//{
//    [Tooltip("是否启用自动隐藏功能")]
//    public bool enableAutoHide = true;
    
//    [Tooltip("隐藏的时间延迟（秒）")]
//    public float hideDelay = 0.1f;
    
//    [Tooltip("场景开始后检查多少次")]
//    public int checkTimes = 3;
    
//    [Tooltip("检查间隔（秒）")]
//    public float checkInterval = 0.5f;
    
//    private static MenuHideManager _instance;
//    public static MenuHideManager Instance
//    {
//        get { return _instance; }
//    }
    
//    private void Awake()
//    {
//        // 单例模式
//        if (_instance == null)
//        {
//            _instance = this;
//            DontDestroyOnLoad(gameObject);
            
//            // 注册场景加载事件
//            SceneManager.sceneLoaded += OnSceneLoaded;
//        }
//        else
//        {
//            Destroy(gameObject);
//        }
//    }
    
//    private void OnDestroy()
//    {
//        // 取消注册事件
//        SceneManager.sceneLoaded -= OnSceneLoaded;
//    }
    
//    // 场景加载完成时调用
//    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
//    {
//        if (enableAutoHide)
//        {
//            // 延迟执行以确保所有对象都加载完成
//            Invoke("HideAllMenus", hideDelay);
            
//            // 多次检查以确保菜单被隐藏
//            for (int i = 0; i < checkTimes; i++)
//            {
//                Invoke("HideAllMenus", hideDelay + checkInterval * (i + 1));
//            }
//        }
//    }
    
//    // 隐藏所有菜单
//    public void HideAllMenus()
//    {
//        // 查找所有PauseMenu组件
//        PauseMenu[] pauseMenus = FindObjectsOfType<PauseMenu>(true); // true表示包括非激活对象
//        foreach (PauseMenu menu in pauseMenus)
//        {
//            if (menu.pauseMenuPanel != null && menu.pauseMenuPanel.activeInHierarchy)
//            {
//                Debug.Log($"隐藏菜单: {menu.pauseMenuPanel.name} 在场景: {SceneManager.GetActiveScene().name}");
//                menu.pauseMenuPanel.SetActive(false);
//            }
            
//            if (menu.creditsPanel != null && menu.creditsPanel.activeInHierarchy)
//            {
//                Debug.Log($"隐藏鸣谢面板: {menu.creditsPanel.name} 在场景: {SceneManager.GetActiveScene().name}");
//                menu.creditsPanel.SetActive(false);
//            }
//        }
        
//        // 查找名称包含特定关键词的面板
//        string[] menuKeywords = new string[] { "PauseMenu", "PausePanel", "MenuPanel", "CreditsPanel", "SettingsPanel" };
//        foreach (string keyword in menuKeywords)
//        {
//            GameObject[] panels = GameObject.FindObjectsOfType<GameObject>(true);
//            foreach (GameObject panel in panels)
//            {
//                if (panel.name.Contains(keyword) && panel.activeInHierarchy)
//                {
//                    Debug.Log($"通过关键字隐藏菜单: {panel.name}");
//                    panel.SetActive(false);
//                }
//            }
//        }
        
//        // 确保时间正常流动
//        Time.timeScale = 1f;
//    }
    
//    // 公开方法：手动隐藏所有菜单
//    public static void ForceHideAllMenus()
//    {
//        if (Instance != null)
//        {
//            Instance.HideAllMenus();
//        }
//    }
//} 