using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Button))]
public class SimpleButtonEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public float hoverScale = 1.05f;  // 鼠标悬停放大倍数
    
    private RectTransform rectTransform;
    private Vector3 originalScale;
    
    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            originalScale = rectTransform.localScale;
        }
    }
    
    // 鼠标进入时调用
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (rectTransform != null)
        {
            rectTransform.localScale = originalScale * hoverScale;
        }
    }
    
    // 鼠标离开时调用
    public void OnPointerExit(PointerEventData eventData)
    {
        if (rectTransform != null)
        {
            rectTransform.localScale = originalScale;
        }
    }
    
    private void OnDisable()
    {
        // 确保退出时恢复原始大小
        if (rectTransform != null)
        {
            rectTransform.localScale = originalScale;
        }
    }
} 