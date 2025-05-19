using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

public class PanelController : MonoBehaviour
{
    public GameObject panel; // 弹出的面板
    public Button triggerButton; // 触发弹出的按钮
    public TMP_Text dialogText; // 用于显示对话的文本组件
    public string targetSceneName = "NextScene"; // 跳转的目标场景名称
    public float fadeInDuration = 1.0f; // 渐入动画的持续时间

    private CanvasGroup canvasGroup;
    private string dialogContent = "这是一个对话示例。"; // 对话内容

    void Start()
    {
        // 面板初始状态为隐藏
        panel.SetActive(false);

        // 获取 CanvasGroup 组件
        canvasGroup = panel.AddComponent<CanvasGroup>();
        canvasGroup.alpha = 0f; // 初始透明度为 0
        canvasGroup.interactable = false; // 初始不可交互

        if (triggerButton != null)
        {
            // 为按钮添加点击事件监听器
            triggerButton.onClick.AddListener(ShowPanel);
        }
        else
        {
            Debug.LogError("PanelController: triggerButton 未设置！请在Inspector中设置Button引用");
        }

        // 检查对话文本组件是否设置
        if (dialogText == null)
        {
            Debug.LogError("PanelController: dialogText 未设置！请在Inspector中设置Text组件引用");
        }
    }

    // 点击按钮时弹出面板并显示对话
    public void ShowPanel()
    {
        panel.SetActive(true);
        Debug.Log("面板已弹出");

        // 设置对话文本
        if (dialogText != null)
        {
            dialogText.text = dialogContent;
        }

        // 开始渐入动画
        StartCoroutine(FadeInPanel());

        // 30秒后自动隐藏面板并跳转场景
        StartCoroutine(DelayedHideAndLoadScene());
    }

    // 渐入动画
    IEnumerator FadeInPanel()
    {
        float elapsedTime = 0f;
        while (elapsedTime < fadeInDuration)
        {
            canvasGroup.alpha = elapsedTime / fadeInDuration;
            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }
        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
    }

    // 30秒后自动隐藏面板并跳转场景
    IEnumerator DelayedHideAndLoadScene()
    {
        yield return new WaitForSeconds(30f);

        // 开始渐出动画
        StartCoroutine(FadeOutPanelAndLoadScene());
    }

    // 渐出动画并跳转场景
    IEnumerator FadeOutPanelAndLoadScene()
    {
        // 禁用面板交互
        canvasGroup.interactable = false;

        // 渐出动画逻辑
        float elapsedTime = 0f;
        float startAlpha = canvasGroup.alpha;

        while (elapsedTime < fadeInDuration)
        {
            canvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, elapsedTime / fadeInDuration);
            elapsedTime += Time.unscaledDeltaTime; // 使用不受时间缩放影响的增量时间
            yield return null;
        }

        // 确保完全透明
        canvasGroup.alpha = 0f;
        panel.SetActive(false);
        Debug.Log("面板渐出完成");

        // 场景跳转验证逻辑
        if (string.IsNullOrEmpty(targetSceneName))
        {
            Debug.LogError("场景名称未设置！请在Inspector中指定目标场景名称");
            yield break;
        }

        // 使用Unity内置场景存在性检查
        if (Application.CanStreamedLevelBeLoaded(targetSceneName))
        {
            Debug.Log($"正在加载场景: {targetSceneName}");
            SceneManager.LoadScene(targetSceneName);
        }
        else
        {
            Debug.LogError($"场景 '{targetSceneName}' 不存在于Build Settings！请检查：");
            Debug.LogError("- 场景名称拼写（区分大小写）");
            Debug.LogError("- Build Settings中的场景列表（需手动添加）");
            Debug.LogError("- 场景文件扩展名（Build Settings中不应包含.unity后缀）");
        }

        yield break; // 明确结束协程
    }
}