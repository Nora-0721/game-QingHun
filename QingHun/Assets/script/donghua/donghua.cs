using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayAnimationOnSceneLoad : MonoBehaviour
{
    public string targetSceneName = "NextScene";

    void OnEnable()
    {
        // 如果场景名匹配，直接播放动画
        if (SceneManager.GetActiveScene().name == "donghua")
        {
            PlayAnimation();
        }
    }

    private void PlayAnimation()
    {
        Animation animationComponent = GetComponent<Animation>();
        if (animationComponent != null)
        {
            animationComponent.Play("dong");
        }
    }

    // 动画事件调用的方法
    public void OnAnimationComplete()
    {
        SceneTransitionManager1.Instance.LoadSceneWithTransition(targetSceneName);
    }
}