using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 添加到任何UI面板对象，使其始终显示在最上层并保证交互正常
/// </summary>
public class MenuLayerFixer : MonoBehaviour
{
    [Tooltip("是否自动提升Canvas层级")]
    public bool autoFixCanvasSorting = true;
    
    [Tooltip("Canvas排序顺序")]
    public int canvasSortingOrder = 100;
    
    [Tooltip("是否在启用时自动修复层级")]
    public bool fixOnEnable = true;
    
    [Tooltip("是否在该组件唤醒时修复层级")]
    public bool fixOnAwake = true;
    
    [Tooltip("是否在该组件启动时修复层级")]
    public bool fixOnStart = true;
    
    private Canvas targetCanvas;
    private GraphicRaycaster targetRaycaster;
    
    private void Awake()
    {
        // 初始化
        Initialize();
        
        // 如果需要在Awake时修复，立即修复
        if (fixOnAwake)
        {
            FixMenuLayer();
        }
    }
    
    private void Start()
    {
        // 如果需要在Start时修复，立即修复
        if (fixOnStart)
        {
            FixMenuLayer();
        }
    }
    
    private void OnEnable()
    {
        // 如果需要在启用时修复，立即修复
        if (fixOnEnable)
        {
            FixMenuLayer();
        }
    }
    
    /// <summary>
    /// 初始化组件
    /// </summary>
    private void Initialize()
    {
        // 获取当前对象或父级的Canvas组件
        targetCanvas = GetComponent<Canvas>();
        if (targetCanvas == null)
        {
            targetCanvas = GetComponentInParent<Canvas>();
        }
        
        // 如果找不到Canvas，尝试在父级查找
        if (targetCanvas == null)
        {
            Canvas[] canvases = GetComponentsInParent<Canvas>();
            if (canvases.Length > 0)
            {
                targetCanvas = canvases[0]; // 使用最近的父级Canvas
            }
        }
        
        // 如果还是找不到，添加一个新的Canvas
        if (targetCanvas == null)
        {
            Debug.Log($"在 {gameObject.name} 上创建新的Canvas组件");
            targetCanvas = gameObject.AddComponent<Canvas>();
            targetCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        }
        
        // 获取或添加GraphicRaycaster组件
        targetRaycaster = targetCanvas.GetComponent<GraphicRaycaster>();
        if (targetRaycaster == null)
        {
            targetRaycaster = targetCanvas.gameObject.AddComponent<GraphicRaycaster>();
        }
    }
    
    /// <summary>
    /// 修复菜单层级，确保它显示在最上层并可以交互
    /// </summary>
    public void FixMenuLayer()
    {
        if (targetCanvas == null)
        {
            Initialize();
        }
        
        if (targetCanvas != null)
        {
            // 设置Canvas属性
            if (autoFixCanvasSorting)
            {
                // 确保是屏幕空间-覆盖模式
                targetCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
                
                // 设置排序顺序
                targetCanvas.sortingOrder = canvasSortingOrder;
                
                // 确保所有子UI元素也在顶层
                Canvas[] childCanvases = GetComponentsInChildren<Canvas>();
                foreach (Canvas childCanvas in childCanvases)
                {
                    if (childCanvas != targetCanvas) // 避免无限循环
                    {
                        childCanvas.overrideSorting = true;
                        childCanvas.sortingOrder = canvasSortingOrder + 1; // 比父级高一层
                    }
                }
            }
            
            // 确保Canvas有射线检测组件
            if (targetRaycaster != null)
            {
                targetRaycaster.enabled = true;
            }
            
            // 刷新Canvas状态
            targetCanvas.enabled = false;
            targetCanvas.enabled = true;
            
            // 将此对象移到兄弟对象的最前面
            transform.SetAsLastSibling();
        }
    }
    
    /// <summary>
    /// 静态方法：修复指定游戏对象的菜单层级
    /// </summary>
    public static void FixLayer(GameObject menuObject)
    {
        if (menuObject == null) return;
        
        // 获取或添加MenuLayerFixer组件
        MenuLayerFixer fixer = menuObject.GetComponent<MenuLayerFixer>();
        if (fixer == null)
        {
            fixer = menuObject.AddComponent<MenuLayerFixer>();
        }
        
        // 执行修复
        fixer.FixMenuLayer();
    }
} 