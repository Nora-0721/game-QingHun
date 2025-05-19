using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Button))]
public class ButtonScaleEffect : MonoBehaviour
{
    [Header("鼠标悬停效果")]
    public float hoverScale = 1.05f;   // 鼠标悬停时放大倍数
    public float scaleDuration = 0.1f;  // 缩放过渡时间
    
    private Button button;
    private RectTransform rectTransform;
    private Vector3 originalScale;
    private bool isScaling = false;
    private Vector3 targetScale;
    private float scaleTime = 0f;
    
    private void Awake()
    {
        button = GetComponent<Button>();
        rectTransform = GetComponent<RectTransform>();
        originalScale = rectTransform.localScale;
    }
    
    private void Start()
    {
        AddHoverEffect();
    }
    
    private void Update()
    {
        // 平滑缩放效果
        if (isScaling)
        {
            scaleTime += Time.deltaTime;
            float t = Mathf.Clamp01(scaleTime / scaleDuration);
            rectTransform.localScale = Vector3.Lerp(rectTransform.localScale, targetScale, t);
            
            if (t >= 1.0f)
                isScaling = false;
        }
    }
    
    private void AddHoverEffect()
    {
        // 添加事件触发器
        EventTrigger trigger = GetComponent<EventTrigger>();
        if (trigger == null)
            trigger = gameObject.AddComponent<EventTrigger>();
            
        trigger.triggers.Clear();
        
        // 鼠标进入 - 放大效果
        EventTrigger.Entry enterEntry = new EventTrigger.Entry();
        enterEntry.eventID = EventTriggerType.PointerEnter;
        enterEntry.callback.AddListener((data) => {
            SetTargetScale(originalScale * hoverScale);
        });
        trigger.triggers.Add(enterEntry);
        
        // 鼠标退出 - 恢复原始大小
        EventTrigger.Entry exitEntry = new EventTrigger.Entry();
        exitEntry.eventID = EventTriggerType.PointerExit;
        exitEntry.callback.AddListener((data) => {
            SetTargetScale(originalScale);
        });
        trigger.triggers.Add(exitEntry);
    }
    
    private void SetTargetScale(Vector3 newScale)
    {
        targetScale = newScale;
        isScaling = true;
        scaleTime = 0f;
    }
    
    private void OnDisable()
    {
        // 恢复原始大小
        if (rectTransform != null)
            rectTransform.localScale = originalScale;
    }
} 