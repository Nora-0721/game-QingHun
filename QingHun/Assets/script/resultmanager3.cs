using UnityEngine;

public class SceneAutoTransition : MonoBehaviour
{
    private float startTime;
    public string targetSceneName = "SampleScene"; // 在此处填写目标场景名称

    void Start()
    {
        startTime = Time.time;
    }

    void Update()
    {
        // 检查是否达到4秒且没有正在进行的过渡
        if (Time.time - startTime >= 4f)
        {
            // 调用场景过渡管理器切换场景
            SceneTransitionManager1.Instance.LoadSceneWithTransition(targetSceneName);

            // 禁用脚本防止重复调用
            enabled = false;
        }
    }
}