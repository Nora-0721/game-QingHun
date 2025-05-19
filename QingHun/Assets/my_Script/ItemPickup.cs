using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ItemPickup : MonoBehaviour
{
    [Header("UI References")]
    public GameObject pickupPanel;
    public Button confirmButton;
    public Image itemImage;
    public TMP_Text nameText;
    public TMP_Text desText;

    [Header("鼠标悬停预览")]
    public GameObject hoverPreviewPanel;   // 鼠标悬停时显示的预览面板
    public Image hoverPreviewImage;        // 预览图片
    public float hoverShowDelay = 0.2f;    // 悬停多久后显示预览（秒）
    public Vector2 previewOffset = new Vector2(30, 30);  // 预览面板相对于鼠标的偏移
    public bool debugMode = true;          // 调试模式开关
    public bool fixedPosition = true;      // 预览面板位置是否固定（不跟随鼠标）
    public Vector2 fixedPreviewPosition = new Vector2(200, 200); // 固定位置时的屏幕坐标

    [Header("Item Data")]
    public int itemID; // 每个物品在Inspector中设置唯一ID
    public Sprite itemSprite; // 每个物品单独指定图片
    public Sprite hoverSprite; // 悬停时显示的图片，如果不设置则使用itemSprite

    [Header("Settings")]
    public float textTypeDelay = 0.1f;
    public float displayDuration = 3f;
    [Range(0.1f, 1.5f)]
    public float animationDuration = 0.5f; // 面板动画持续时间
    public AnimationCurve animationCurve = AnimationCurve.EaseInOut(0, 0, 1, 1); // 默认动画曲线

    [Header("Item")]
    public SpriteRenderer Item;

    [Tooltip("要加载的场景名称")]
    public string sceneToLoad;

    // 新增的Image组件，用于改变透明度
    public Image hoverHighlightImage;

    // Canvas引用
    public Canvas itemCanvas;

    // 面板动画控制
    private RectTransform panelRectTransform;
    private Vector2 initialPanelScale;
    private bool isTypingText = false;
    private bool canConfirm = false;

    // 悬停控制
    private bool isHovering = false;
    private Coroutine hoverCoroutine = null;
    private Coroutine alphaCoroutine = null;

    // 新增：跟踪是否点击了特定位置
    private bool isClicked = false;

    private void Awake()
    {
        // 获取面板的RectTransform
        if (pickupPanel != null)
        {
            panelRectTransform = pickupPanel.GetComponent<RectTransform>();
            if (panelRectTransform != null)
            {
                initialPanelScale = panelRectTransform.localScale;
            }
        }

        // 初始隐藏面板
        if (pickupPanel != null)
        {
            pickupPanel.SetActive(false);
        }

        // 初始隐藏悬停预览面板
        if (hoverPreviewPanel != null)
        {
            hoverPreviewPanel.SetActive(false);
        }
        else if (debugMode)
        {
            Debug.LogError("悬停预览面板未设置！请在Inspector中为hoverPreviewPanel赋值。");
        }

        // 初始隐藏和重置悬停高亮Image的透明度
        if (hoverHighlightImage != null)
        {
            hoverHighlightImage.color = new Color(hoverHighlightImage.color.r, hoverHighlightImage.color.g, hoverHighlightImage.color.b, 0);
        }

        if (debugMode)
        {
            Debug.Log("ItemPickup组件已初始化，预览面板状态：" + (hoverPreviewPanel != null ? "已引用" : "未引用"));
        }
    }

    private void Start()
    {
        // 初始化确认按钮事件
        confirmButton.onClick.AddListener(OnConfirmPickup);
        pickupPanel.SetActive(false);

        // 禁用按钮
        SetConfirmButtonActive(false);

        // 确保开始时面板缩放为0
        if (panelRectTransform != null)
        {
            panelRectTransform.localScale = Vector3.zero;
        }

        // 设置悬停预览图片
        if (hoverPreviewImage != null)
        {
            // 如果没有专门为悬停设置图片，就使用物品图片
            if (hoverSprite == null)
            {
                hoverSprite = itemSprite;
            }
            hoverPreviewImage.sprite = hoverSprite;

            if (debugMode && hoverSprite == null && itemSprite == null)
            {
                Debug.LogWarning("预览图片未设置！请为hoverSprite或itemSprite指定图片。");
            }
        }
        else if (debugMode)
        {
            Debug.LogError("预览图片组件未设置！请在Inspector中为hoverPreviewImage赋值。");
        }

        // 检查Canvas设置
        CheckCanvasSetup();
    }

    // 检查Canvas设置是否正确
    private void CheckCanvasSetup()
    {
        if (!debugMode) return;

        if (hoverPreviewPanel != null)
        {
            Canvas parentCanvas = hoverPreviewPanel.GetComponentInParent<Canvas>();
            if (parentCanvas == null)
            {
                Debug.LogError("预览面板不在Canvas下！预览面板必须是Canvas的子物体才能正确显示。");
            }
            else
            {
                Debug.Log("预览面板Canvas检测：成功。Canvas渲染模式：" + parentCanvas.renderMode);

                // 检查Canvas的RenderMode
                if (parentCanvas.renderMode != RenderMode.ScreenSpaceOverlay && parentCanvas.renderMode != RenderMode.ScreenSpaceCamera)
                {
                    Debug.LogWarning("Canvas渲染模式不是ScreenSpace。对于UI元素，推荐使用ScreenSpaceOverlay或ScreenSpaceCamera。");
                }

                // 检查Canvas的排序顺序
                Debug.Log("Canvas排序顺序：" + parentCanvas.sortingOrder);
                if (parentCanvas.sortingOrder < 1)
                {
                    Debug.LogWarning("Canvas排序顺序较低，可能被其他UI元素遮挡。建议设置较高的sortingOrder。");
                }
            }

            // 检查预览面板的位置
            RectTransform rectTransform = hoverPreviewPanel.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                Debug.Log("预览面板位置：" + rectTransform.position);
                Debug.Log("预览面板缩放：" + rectTransform.localScale);

                // 检查预览面板是否有异常的缩放或位置
                if (rectTransform.localScale.x == 0 || rectTransform.localScale.y == 0)
                {
                    Debug.LogError("预览面板的缩放为0，这会导致面板不可见！");
                }
            }
        }

        // 检查新增的hoverHighlightImage
        if (hoverHighlightImage != null)
        {
            Debug.Log("hoverHighlightImage已引用");
        }
        else if (debugMode)
        {
            Debug.LogWarning("hoverHighlightImage未设置！请在Inspector中为hoverHighlightImage赋值。");
        }
    }

    private void Update()
    {
        // 只有在非固定位置模式下才更新预览面板位置
        if (!fixedPosition && hoverPreviewPanel != null && hoverPreviewPanel.activeSelf)
        {
            Vector2 mousePosition = Input.mousePosition;
            hoverPreviewPanel.transform.position = mousePosition + previewOffset;

            if (debugMode && Time.frameCount % 60 == 0) // 每60帧输出一次，避免刷屏
            {
                Debug.Log("预览面板位置更新：" + hoverPreviewPanel.transform.position);
            }
        }
    }

    private void OnMouseDown()
    {
        // 只有在未激活面板时才响应点击
        if (!pickupPanel.activeSelf && !isTypingText)
        {
            StartCoroutine(ShowItem());
        }

        // 新增：鼠标点击时将透明度设置为0，并设置Canvas的SortingLayer为-1
        if (hoverHighlightImage != null)
        {
            hoverHighlightImage.color = new Color(hoverHighlightImage.color.r, hoverHighlightImage.color.g, hoverHighlightImage.color.b, 0);
            isClicked = true;
        }

        // 设置Canvas的SortingLayer为-1
        if (itemCanvas != null)
        {
            itemCanvas.sortingOrder = -1;
        }
    }

    private void OnMouseEnter()
    {
        // 鼠标进入物品区域
        isHovering = true;

        if (debugMode)
        {
            Debug.Log("鼠标进入物品区域");
        }

        // 启动延迟显示预览的协程
        if (hoverCoroutine != null)
        {
            StopCoroutine(hoverCoroutine);
        }
        hoverCoroutine = StartCoroutine(ShowHoverPreview());

        // 如果没有点击过，启动改变透明度的协程
        if (!isClicked)
        {
            if (alphaCoroutine != null)
            {
                StopCoroutine(alphaCoroutine);
            }
            alphaCoroutine = StartCoroutine(ChangeAlpha(255));
        }
    }

    private void OnMouseExit()
    {
        // 鼠标离开物品区域
        isHovering = false;

        if (debugMode)
        {
            Debug.Log("鼠标离开物品区域");
        }

        // 停止悬停预览协程
        if (hoverCoroutine != null)
        {
            StopCoroutine(hoverCoroutine);
            hoverCoroutine = null;
        }

        // 隐藏预览面板
        if (hoverPreviewPanel != null)
        {
            hoverPreviewPanel.SetActive(false);
        }

        // 如果没有点击过，启动改变透明度的协程（恢复到0）
        if (!isClicked)
        {
            if (alphaCoroutine != null)
            {
                StopCoroutine(alphaCoroutine);
            }
            alphaCoroutine = StartCoroutine(ChangeAlpha(0));
        }
    }

    // 显示悬停预览
    private IEnumerator ShowHoverPreview()
    {
        if (debugMode)
        {
            Debug.Log("开始等待预览显示延迟：" + hoverShowDelay + "秒");
        }

        // 等待指定延迟时间
        yield return new WaitForSeconds(hoverShowDelay);

        // 如果还在悬停状态且预览面板和图片都有效
        if (isHovering && hoverPreviewPanel != null && hoverPreviewImage != null)
        {
            // 设置预览图片
            hoverPreviewImage.sprite = hoverSprite != null ? hoverSprite : itemSprite;

            // 显示预览面板
            hoverPreviewPanel.SetActive(true);

            // 设置面板位置
            if (fixedPosition)
            {
                // 使用固定的屏幕位置
                hoverPreviewPanel.transform.position = fixedPreviewPosition;

                if (debugMode)
                {
                    Debug.Log("预览面板使用固定位置：" + fixedPreviewPosition);
                }
            }
            else
            {
                // 使用鼠标位置 + 偏移
                Vector2 mousePosition = Input.mousePosition;
                hoverPreviewPanel.transform.position = mousePosition + previewOffset;

                if (debugMode)
                {
                    Debug.Log("预览面板使用鼠标相对位置：" + hoverPreviewPanel.transform.position);
                }
            }

            if (debugMode)
            {
                Debug.Log("预览面板已激活！最终位置：" + hoverPreviewPanel.transform.position);
                Debug.Log("预览图片：" + (hoverPreviewImage.sprite != null ? hoverPreviewImage.sprite.name : "未设置"));
            }

            // 确保面板在最上层
            Canvas parentCanvas = hoverPreviewPanel.GetComponentInParent<Canvas>();
            if (parentCanvas != null)
            {
                parentCanvas.sortingOrder = 100; // 设置较高的排序顺序
                if (debugMode)
                {
                    Debug.Log("Canvas排序顺序已设为：" + parentCanvas.sortingOrder);
                }
            }

            // 将面板移到其兄弟对象的最上层
            hoverPreviewPanel.transform.SetAsLastSibling();
        }
        else if (debugMode)
        {
            if (!isHovering)
                Debug.LogWarning("无法显示预览：鼠标已离开物品区域");
            else if (hoverPreviewPanel == null)
                Debug.LogError("无法显示预览：预览面板引用为空");
            else if (hoverPreviewImage == null)
                Debug.LogError("无法显示预览：预览图片组件引用为空");
        }
    }

    IEnumerator ShowItem()
    {
        // 设置基本信息
        nameText.text = "物品：" + Item.name; // 替换为你的实际名称逻辑

        // 设置物品图像
        if (itemImage != null && itemSprite != null)
        {
            itemImage.sprite = itemSprite;
            itemImage.enabled = true;

        }

        // 显示面板
        pickupPanel.SetActive(true);

        // 播放面板打开动画
        yield return StartCoroutine(AnimatePanelScale(Vector3.zero, initialPanelScale, animationDuration));

        // 打字机效果显示描述
        string description = GetItemDescription(itemID);
        isTypingText = true;
        canConfirm = false;
        SetConfirmButtonActive(false); // 禁用确认按钮

        yield return StartCoroutine(TypeText(desText, description, textTypeDelay));

        isTypingText = false;
        canConfirm = true;
        SetConfirmButtonActive(true); // 启用确认按钮
    }

    IEnumerator AnimatePanelScale(Vector3 startScale, Vector3 endScale, float duration)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);
            float curveValue = animationCurve.Evaluate(t);

            panelRectTransform.localScale = Vector3.Lerp(startScale, endScale, curveValue);

            yield return null;
        }

        panelRectTransform.localScale = endScale;
    }

    IEnumerator TypeText(TMP_Text textComp, string content, float interval)
    {
        textComp.text = "";
        foreach (char c in content)
        {
            textComp.text += c;
            yield return new WaitForSeconds(interval);
        }
    }

    private void SetConfirmButtonActive(bool active)
    {
        // 设置按钮状态
        confirmButton.interactable = active;

        // 可选：改变按钮颜色提示用户
        ColorBlock colors = confirmButton.colors;
        if (active)
        {
            // 正常颜色
            colors.normalColor = new Color(1f, 1f, 1f, 1f);
        }
        else
        {
            // 禁用状态颜色更灰暗
            colors.disabledColor = new Color(0.7f, 0.7f, 0.7f, 0.5f);
        }
        confirmButton.colors = colors;
    }

    // 手动跳过打字机效果
    public void SkipTyping()
    {
        if (isTypingText)
        {
            StopAllCoroutines();
            isTypingText = false;

            // 直接显示完整文本
            desText.text = GetItemDescription(itemID);

            // 启用确认按钮
            canConfirm = true;
            SetConfirmButtonActive(true);
        }
    }

    public void OnConfirmPickup()
    {
        // 检查是否可以确认
        if (!canConfirm) return;

        // 播放关闭动画
        StartCoroutine(ClosePickupPanel());
    }

    private IEnumerator ClosePickupPanel()
    {
        // 播放面板关闭动画
        yield return StartCoroutine(AnimatePanelScale(initialPanelScale, Vector3.zero, animationDuration));

        // 隐藏面板
        pickupPanel.SetActive(false);

        // 隐藏物品
        Item.gameObject.SetActive(false);

        // 加载新场景
        SceneManager.LoadScene(sceneToLoad);
    }

    private string GetItemDescription(int id)
    {
        return id switch
        {
            1 => "观此伞兮，形制精巧。佳纸为面，若裁青天之云霭；青蛇作饰，栩栩如生之可巧。伞骨架构精巧，伞柄锃亮如新。",
            2 => "嘎嘎嘎，看啥看？！",
            3 => "双面都有诗句的红绸。身形固有桎梏，两颗心却紧密相连，足可以见得祈愿之人的美好愿景。"
        };
    }

    // 在编辑器中运行时的额外检查
    private void OnValidate()
    {
        if (hoverPreviewPanel == null)
        {
            Debug.LogWarning("ItemPickup组件需要设置hoverPreviewPanel引用");
        }

        if (hoverPreviewImage == null)
        {
            Debug.LogWarning("ItemPickup组件需要设置hoverPreviewImage引用");
        }

        if (hoverHighlightImage == null)
        {
            Debug.LogWarning("ItemPickup组件需要设置hoverHighlightImage引用");
        }
    }

    // 新增的方法：改变透明度
    private IEnumerator ChangeAlpha(float targetAlpha)
    {
        if (hoverHighlightImage != null)
        {
            Color currentColor = hoverHighlightImage.color;
            float startAlpha = currentColor.a;
            float elapsedTime = 0f;
            float duration = 0.5f; // 持续时间

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                float t = Mathf.Clamp01(elapsedTime / duration);
                float currentAlpha = Mathf.Lerp(startAlpha, targetAlpha / 255f, t); // 将0-255转换为0-1
                currentColor.a = currentAlpha;
                hoverHighlightImage.color = currentColor;
                yield return null;
            }

            // 确保最终设置为目标值
            currentColor.a = targetAlpha / 255f;
            hoverHighlightImage.color = currentColor;
        }
    }
}