using System.Collections;
using UnityEngine;
using TMPro;
using System.Text;
using UnityEngine.SceneManagement; // 确保引入场景管理

public class VerticalTextDisplay : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text textComponent;
    public CanvasGroup canvasGroup; // 用于淡入淡出

    [Header("Timing")]
    public float charDelay = 0.05f; // 字符显示间隔
    public float fadeDuration = 0.5f; // 淡入淡出持续时间

    [Header("Navigation")]
    public string nextSceneName; // 结束时加载的下一个场景名

    // 预设的文案内容
    private readonly string[] textPassages = new string[]
    {
        // 文案一 (使用 @ 使字符串可以跨行，并处理引号)
        @"『州府正逢春雨时，
惊鸿一瞥启奇缘』
魂牵伞柄殷勤递，
面赧先通姓氏踪：
保和堂内悬壶客，
愿借清荫护玉容。
若记今朝萍水遇，
药香深处待君逢。",
        // 文案二
        @"二位姑娘，可否需要借伞|
小生唐突，二位姑娘容禀了，
若要还伞，
到保和堂寻坐堂大夫许仙便可！",
        // 文案三
        @"语毕径自疾行远，
惟余一伞留手中。
涟漪漫过西湖岸，
一段尘缘自此浓。",
        // 文案四
        @"哈哈，姐姐，
你瞧这个人真有意思，
把伞借给我们，
自己淋雨跑回去了！
......姐姐？
姐姐你说句话阿"
    };

    private int currentPassageIndex = 0;
    private bool isTyping = false;
    private bool canClickToAdvance = false;
    private bool isLastPassage = false;
    private Coroutine displayCoroutine; // 用于跟踪打字协程

    void Awake()
    {
        if (textComponent == null)
        {
            Debug.LogError("Text Component 未在 Inspector 中指定!");
            enabled = false;
            return;
        }
        if (canvasGroup == null)
        {
            canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }
        }

        // 初始化 TextMeshPro 设置
        // 对于旋转文本，RectTransform 的 Height 需要足够大以容纳旋转后文本的"长度"
        // Width 需要足够容纳旋转后文本的"行数"（即原始文本的行数）
        RectTransform rect = textComponent.GetComponent<RectTransform>();
        if (rect != null)
        {
            // 根据您的文本长度和行数，您可能需要调整这些值
            // 例如，如果最长的段落旋转后需要800的屏幕高度，行数转换成宽度后需要400
            // rect.sizeDelta = new Vector2(400, 800); 
        }
        textComponent.enableWordWrapping = false; 
        textComponent.text = "";
        canvasGroup.alpha = 1f; 
    }

    void Start()
    {
        if (textPassages.Length > 0)
        {
            displayCoroutine = StartCoroutine(DisplayCurrentPassage());
        }
        else
        {
            Debug.LogWarning("没有提供任何文本内容。");
        }
    }

    void Update()
    {
        // 确保不在打字时才能点击，并且可以点击
        if (canClickToAdvance && Input.GetMouseButtonDown(0) && !isTyping)
        {
            if (isLastPassage)
            {
                HandleEndOfPassages();
            }
            else
            {
                // 停止任何可能正在运行的旧的打字协程
                if (displayCoroutine != null)
                {
                    StopCoroutine(displayCoroutine);
                }
                StartCoroutine(AdvanceToNextPassage());
            }
        }
    }

    IEnumerator DisplayCurrentPassage()
    {
        isTyping = true;
        canClickToAdvance = false;
        textComponent.text = ""; // 清空，确保旧的 rotate 标签被移除

        if (currentPassageIndex < textPassages.Length)
        {
            string passageToShow = textPassages[currentPassageIndex];
            StringBuilder currentTypedText = new StringBuilder();

            foreach (char c in passageToShow)
            {
                currentTypedText.Append(c);
                textComponent.text = "<rotate=90>" + currentTypedText.ToString() + "</rotate>";
                yield return new WaitForSeconds(charDelay);
            }
            
            // 如果段落为空，确保标签闭合
            if(string.IsNullOrEmpty(passageToShow))
            {
                textComponent.text = "<rotate=90></rotate>";
            }

            isLastPassage = (currentPassageIndex == textPassages.Length - 1);
            canClickToAdvance = true; // 允许点击进入下一段
        }
        isTyping = false;
    }

    IEnumerator AdvanceToNextPassage()
    {
        canClickToAdvance = false; 

        // 淡出当前文本
        float timer = 0f;
        while (timer < fadeDuration)
        {
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, timer / fadeDuration);
            timer += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = 0f; // 确保完全淡出

        currentPassageIndex++;
        
        // 开始显示下一段 (它会立即开始打字，但因为 alpha=0 所以暂时不可见)
        displayCoroutine = StartCoroutine(DisplayCurrentPassage());

        // 淡入新的文本
        timer = 0f;
        while (timer < fadeDuration)
        {
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, timer / fadeDuration);
            timer += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = 1f; // 确保完全淡入
    }

    void HandleEndOfPassages()
    {
        Debug.Log("所有文案显示完毕。");
        canClickToAdvance = false; // 禁止再次点击
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
        // 如果没有指定 nextSceneName，则文本会停在最后一句，不再响应点击
    }
}