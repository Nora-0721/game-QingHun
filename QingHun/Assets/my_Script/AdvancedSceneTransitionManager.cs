using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AdvancedSceneTransitionManager : MonoBehaviour
{
    public static AdvancedSceneTransitionManager Instance;

    // 过渡效果类型
    public enum TransitionType
    {
        Fade,       // 淡入淡出
        Wipe,       // 擦除效果
        Dissolve,   // 溶解效果
        CircleWipe  // 圆形擦除
    }

    // 过渡方向（用于Wipe效果）
    public enum TransitionDirection
    {
        Left,
        Right,
        Up,
        Down
    }

    [Header("过渡设置")]
    public TransitionType transitionType = TransitionType.Fade;
    public TransitionDirection transitionDirection = TransitionDirection.Right;
    public float transitionTime = 1.0f;
    public AnimationCurve transitionCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    public Color transitionColor = Color.black;

    [Header("高级设置")]
    [Range(0.1f, 3.0f)]
    public float dissolveNoiseScale = 1.0f;  // 溶解噪声缩放
    [Range(0.5f, 3.0f)]
    public float circleExpansionMultiplier = 1.5f;  // 圆形扩张倍数

    private CanvasGroup transitionPanel;
    private Image panelImage;
    private Material transitionMaterial;
    private bool isTransitioning = false;

    // 用于Shader的属性ID
    private int progressID;
    private int directionID;
    private int noiseScaleID;

    private void Awake()
    {
        // 单例模式
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeTransitionPanel();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeTransitionPanel()
    {
        // 创建Canvas
        GameObject canvasObj = new GameObject("TransitionCanvas");
        canvasObj.transform.SetParent(transform);
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 999; // 确保在最上层

        // 添加CanvasScaler
        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);

        // 添加过渡面板
        GameObject panelObj = new GameObject("TransitionPanel");
        panelObj.transform.SetParent(canvasObj.transform, false);
        
        panelImage = panelObj.AddComponent<Image>();
        panelImage.color = transitionColor;
        panelImage.raycastTarget = false;
        
        // 设置面板尺寸铺满屏幕
        RectTransform rectTransform = panelObj.GetComponent<RectTransform>();
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.sizeDelta = Vector2.zero;
        
        // 添加CanvasGroup用于控制透明度
        transitionPanel = panelObj.AddComponent<CanvasGroup>();
        transitionPanel.alpha = 0;
        transitionPanel.blocksRaycasts = false;

        // 创建或加载过渡材质
        SetupTransitionMaterial();
        
        // 缓存Shader属性ID
        progressID = Shader.PropertyToID("_Progress");
        directionID = Shader.PropertyToID("_Direction");
        noiseScaleID = Shader.PropertyToID("_NoiseScale");
    }

    private void SetupTransitionMaterial()
    {
        // 根据过渡类型设置不同材质
        switch (transitionType)
        {
            case TransitionType.Fade:
                // 淡入淡出使用默认图像
                break;
                
            case TransitionType.Wipe:
                transitionMaterial = new Material(Shader.Find("UI/Default"));
                // 如果您有自定义Shader，可以加载：
                // transitionMaterial = new Material(Resources.Load<Shader>("Shaders/WipeTransition"));
                panelImage.material = transitionMaterial;
                break;
                
            case TransitionType.Dissolve:
                transitionMaterial = new Material(Shader.Find("UI/Default"));
                // 如果您有自定义Shader，可以加载：
                // transitionMaterial = new Material(Resources.Load<Shader>("Shaders/DissolveTransition"));
                panelImage.material = transitionMaterial;
                break;
                
            case TransitionType.CircleWipe:
                transitionMaterial = new Material(Shader.Find("UI/Default"));
                // 如果您有自定义Shader，可以加载：
                // transitionMaterial = new Material(Resources.Load<Shader>("Shaders/CircleWipeTransition"));
                panelImage.material = transitionMaterial;
                break;
        }
    }
    
    /// <summary>
    /// 动态设置过渡类型
    /// </summary>
    public void SetTransitionType(TransitionType type)
    {
        transitionType = type;
        SetupTransitionMaterial();
    }
    
    /// <summary>
    /// 设置过渡方向
    /// </summary>
    public void SetTransitionDirection(TransitionDirection direction)
    {
        transitionDirection = direction;
    }

    /// <summary>
    /// 加载场景并带有过渡效果
    /// </summary>
    public void LoadSceneWithTransition(string sceneName)
    {
        if (!isTransitioning)
        {
            StartCoroutine(Transition(sceneName));
        }
        else
        {
            Debug.LogWarning("已经在进行过渡，请等待当前过渡完成");
        }
    }

    private IEnumerator Transition(string sceneName)
    {
        isTransitioning = true;
        
        // 淡入（黑屏出现）
        yield return StartCoroutine(TransitionIn());

        // 加载新场景
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        
        // 等待场景加载完成
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
        
        // 短暂等待，确保场景完全加载
        yield return new WaitForSeconds(0.1f);
        
        // 淡出（黑屏消失）
        yield return StartCoroutine(TransitionOut());
        
        isTransitioning = false;
    }

    private IEnumerator TransitionIn()
    {
        switch (transitionType)
        {
            case TransitionType.Fade:
                yield return StartCoroutine(FadeIn());
                break;
                
            case TransitionType.Wipe:
                yield return StartCoroutine(WipeIn());
                break;
                
            case TransitionType.Dissolve:
                yield return StartCoroutine(DissolveIn());
                break;
                
            case TransitionType.CircleWipe:
                yield return StartCoroutine(CircleWipeIn());
                break;
        }
    }

    private IEnumerator TransitionOut()
    {
        switch (transitionType)
        {
            case TransitionType.Fade:
                yield return StartCoroutine(FadeOut());
                break;
                
            case TransitionType.Wipe:
                yield return StartCoroutine(WipeOut());
                break;
                
            case TransitionType.Dissolve:
                yield return StartCoroutine(DissolveOut());
                break;
                
            case TransitionType.CircleWipe:
                yield return StartCoroutine(CircleWipeOut());
                break;
        }
    }

    #region 淡入淡出效果
    private IEnumerator FadeIn()
    {
        float elapsedTime = 0;
        transitionPanel.alpha = 0;
        
        while (elapsedTime < transitionTime)
        {
            elapsedTime += Time.deltaTime;
            float normalizedTime = Mathf.Clamp01(elapsedTime / transitionTime);
            float curveValue = transitionCurve.Evaluate(normalizedTime);
            
            transitionPanel.alpha = curveValue;
            
            yield return null;
        }
        
        transitionPanel.alpha = 1f;
        transitionPanel.blocksRaycasts = true;
    }

    private IEnumerator FadeOut()
    {
        float elapsedTime = 0;
        transitionPanel.alpha = 1;
        
        while (elapsedTime < transitionTime)
        {
            elapsedTime += Time.deltaTime;
            float normalizedTime = Mathf.Clamp01(elapsedTime / transitionTime);
            float curveValue = transitionCurve.Evaluate(1 - normalizedTime);
            
            transitionPanel.alpha = curveValue;
            
            yield return null;
        }
        
        transitionPanel.alpha = 0f;
        transitionPanel.blocksRaycasts = false;
    }
    #endregion

    #region 擦除效果
    private IEnumerator WipeIn()
    {
        // 由于没有自定义Shader，这里使用简单淡入淡出代替
        // 实际项目中，您应该使用自定义Shader实现擦除效果
        yield return StartCoroutine(FadeIn());
        
        /* 
        // 使用自定义Shader的示例代码：
        float elapsedTime = 0;
        transitionPanel.alpha = 1;
        
        // 设置方向
        int dir = (int)transitionDirection;
        transitionMaterial.SetInt(directionID, dir);
        
        while (elapsedTime < transitionTime)
        {
            elapsedTime += Time.deltaTime;
            float normalizedTime = Mathf.Clamp01(elapsedTime / transitionTime);
            float curveValue = transitionCurve.Evaluate(normalizedTime);
            
            // 更新Shader中的进度值
            transitionMaterial.SetFloat(progressID, curveValue);
            
            yield return null;
        }
        
        transitionMaterial.SetFloat(progressID, 1);
        transitionPanel.blocksRaycasts = true;
        */
    }

    private IEnumerator WipeOut()
    {
        // 由于没有自定义Shader，这里使用简单淡入淡出代替
        yield return StartCoroutine(FadeOut());
        
        /*
        // 使用自定义Shader的示例代码：
        float elapsedTime = 0;
        transitionPanel.alpha = 1;
        
        // 设置方向
        int dir = (int)transitionDirection;
        transitionMaterial.SetInt(directionID, dir);
        
        while (elapsedTime < transitionTime)
        {
            elapsedTime += Time.deltaTime;
            float normalizedTime = Mathf.Clamp01(elapsedTime / transitionTime);
            float curveValue = transitionCurve.Evaluate(1 - normalizedTime);
            
            // 更新Shader中的进度值
            transitionMaterial.SetFloat(progressID, curveValue);
            
            yield return null;
        }
        
        transitionMaterial.SetFloat(progressID, 0);
        transitionPanel.blocksRaycasts = false;
        */
    }
    #endregion

    #region 溶解效果
    private IEnumerator DissolveIn()
    {
        // 由于没有自定义Shader，这里使用简单淡入淡出代替
        yield return StartCoroutine(FadeIn());
        
        /*
        // 使用自定义Shader的示例代码：
        float elapsedTime = 0;
        transitionPanel.alpha = 1;
        
        // 设置噪声缩放
        transitionMaterial.SetFloat(noiseScaleID, dissolveNoiseScale);
        
        while (elapsedTime < transitionTime)
        {
            elapsedTime += Time.deltaTime;
            float normalizedTime = Mathf.Clamp01(elapsedTime / transitionTime);
            float curveValue = transitionCurve.Evaluate(normalizedTime);
            
            // 更新Shader中的进度值
            transitionMaterial.SetFloat(progressID, curveValue);
            
            yield return null;
        }
        
        transitionMaterial.SetFloat(progressID, 1);
        transitionPanel.blocksRaycasts = true;
        */
    }

    private IEnumerator DissolveOut()
    {
        // 由于没有自定义Shader，这里使用简单淡入淡出代替
        yield return StartCoroutine(FadeOut());
        
        /*
        // 使用自定义Shader的示例代码：
        float elapsedTime = 0;
        transitionPanel.alpha = 1;
        
        // 设置噪声缩放
        transitionMaterial.SetFloat(noiseScaleID, dissolveNoiseScale);
        
        while (elapsedTime < transitionTime)
        {
            elapsedTime += Time.deltaTime;
            float normalizedTime = Mathf.Clamp01(elapsedTime / transitionTime);
            float curveValue = transitionCurve.Evaluate(1 - normalizedTime);
            
            // 更新Shader中的进度值
            transitionMaterial.SetFloat(progressID, curveValue);
            
            yield return null;
        }
        
        transitionMaterial.SetFloat(progressID, 0);
        transitionPanel.blocksRaycasts = false;
        */
    }
    #endregion

    #region 圆形擦除效果
    private IEnumerator CircleWipeIn()
    {
        // 由于没有自定义Shader，这里使用简单淡入淡出代替
        yield return StartCoroutine(FadeIn());
        
        /*
        // 使用自定义Shader的示例代码：
        float elapsedTime = 0;
        transitionPanel.alpha = 1;
        
        while (elapsedTime < transitionTime)
        {
            elapsedTime += Time.deltaTime;
            float normalizedTime = Mathf.Clamp01(elapsedTime / transitionTime);
            float curveValue = transitionCurve.Evaluate(normalizedTime);
            
            // 更新Shader中的进度值
            transitionMaterial.SetFloat(progressID, curveValue * circleExpansionMultiplier);
            
            yield return null;
        }
        
        transitionMaterial.SetFloat(progressID, circleExpansionMultiplier);
        transitionPanel.blocksRaycasts = true;
        */
    }

    private IEnumerator CircleWipeOut()
    {
        // 由于没有自定义Shader，这里使用简单淡入淡出代替
        yield return StartCoroutine(FadeOut());
        
        /*
        // 使用自定义Shader的示例代码：
        float elapsedTime = 0;
        transitionPanel.alpha = 1;
        
        while (elapsedTime < transitionTime)
        {
            elapsedTime += Time.deltaTime;
            float normalizedTime = Mathf.Clamp01(elapsedTime / transitionTime);
            float curveValue = transitionCurve.Evaluate(1 - normalizedTime);
            
            // 更新Shader中的进度值
            transitionMaterial.SetFloat(progressID, curveValue * circleExpansionMultiplier);
            
            yield return null;
        }
        
        transitionMaterial.SetFloat(progressID, 0);
        transitionPanel.blocksRaycasts = false;
        */
    }
    #endregion
} 