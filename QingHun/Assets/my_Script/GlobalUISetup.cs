using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class GlobalUISetup : MonoBehaviour
{
    private void Start()
    {
        if (GlobalUIManager.Instance != null)
        {
            Button button = GetComponent<Button>();
            if (button != null)
            {
                // 保存原始点击事件
                var clickEvents = new System.Collections.Generic.List<UnityEngine.Events.UnityAction>();
                for (int i = 0; i < button.onClick.GetPersistentEventCount(); i++)
                {
                    var target = button.onClick.GetPersistentTarget(i);
                    var methodName = button.onClick.GetPersistentMethodName(i);
                    
                    if (target != null && !string.IsNullOrEmpty(methodName))
                    {
                        // 记录原始点击事件，以便在设置效果后保留
                        button.onClick.AddListener(() => {
                            GlobalUIManager.Instance.PlayClickSound();
                        });
                    }
                }
                
                // 应用全局效果
                GlobalUIManager.Instance.SetupButtonEffects(button);
            }
        }
        else
        {
            Debug.LogWarning("GlobalUIManager未找到。无法应用按钮效果。");
        }
    }
} 