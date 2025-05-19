using UnityEngine;

/// <summary>
/// 添加到任何菜单面板上，确保它在游戏开始时是隐藏的
/// 比MenuHideManager更直接，不依赖查找
/// </summary>
public class InitialMenuHider : MonoBehaviour
{
    [Tooltip("是否在Awake时隐藏")]
    public bool hideOnAwake = true;
    
    [Tooltip("是否在Start时隐藏")]
    public bool hideOnStart = true;
    
    [Tooltip("是否在启用时隐藏")]
    public bool hideOnEnable = true;
    
    [Tooltip("是否在场景加载后隐藏")]
    public bool hideOnSceneLoad = true;
    
    [Tooltip("隐藏延迟，防止闪烁")]
    public float hideDelay = 0.1f;
    
    private void Awake()
    {
        if (hideOnAwake)
        {
            HideThisPanel();
        }
    }
    
    private void Start()
    {
        if (hideOnStart)
        {
            HideThisPanel();
            
            // 延迟再次隐藏，确保不会被其他脚本意外显示
            Invoke("HideThisPanel", hideDelay);
        }
    }
    
    private void OnEnable()
    {
        if (hideOnEnable)
        {
            // 延迟隐藏，确保不会被其他脚本覆盖
            Invoke("HideThisPanel", hideDelay);
        }
    }
    
    // 在Unity编辑器中重置组件时调用
    private void Reset()
    {
        // 如果此组件包含"Menu"或"Panel"关键词，则默认启用所有选项
        if (gameObject.name.Contains("Menu") || gameObject.name.Contains("Panel"))
        {
            hideOnAwake = true;
            hideOnStart = true;
            hideOnEnable = true;
            hideOnSceneLoad = true;
        }
    }
    
    /// <summary>
    /// 隐藏此面板
    /// </summary>
    public void HideThisPanel()
    {
        // 如果在编辑器中直接预览时不隐藏（便于设计）
#if UNITY_EDITOR
        if (!Application.isPlaying) return;
#endif

        // 检查是否处于主菜单场景
        if (IsMainMenuScene())
        {
            // 主菜单场景中，只隐藏暂停相关面板，而不隐藏主菜单界面
            if (gameObject.name.Contains("Pause") || gameObject.name.Contains("Credits"))
            {
                gameObject.SetActive(false);
            }
        }
        else
        {
            // 游戏场景中，隐藏所有菜单面板
            gameObject.SetActive(false);
        }
    }
    
    /// <summary>
    /// 检查当前是否在主菜单场景
    /// </summary>
    private bool IsMainMenuScene()
    {
        string currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        return currentScene.Contains("MainMenu") || currentScene.Contains("Start") || currentScene.Contains("Title");
    }
} 