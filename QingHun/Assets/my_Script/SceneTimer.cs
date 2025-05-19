// SceneTimer.cs
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
public class SceneTimer : MonoBehaviour
{
    [Header("场景设置")]
    public string targetSceneName = "NextScene";
    public float delayTime = 3f; // 延迟时间（秒）

    void Start()
    {
        StartCoroutine(DelaySceneTransition());
    }

    private IEnumerator DelaySceneTransition()
    {
        Debug.Log($"场景将在{delayTime}秒后切换");
        yield return new WaitForSeconds(delayTime);

        // 通过场景过渡管理器切换场景
        if (SceneTransitionManager1.Instance != null)
        {
            SceneTransitionManager1.Instance.LoadSceneWithTransition(targetSceneName);
        }
        else
        {
            Debug.LogError("未找到场景过渡管理器");
            SceneManager.LoadScene(targetSceneName);
        }
    }
}