using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayAnimationOnSceneLoad : MonoBehaviour
{
    public string targetSceneName = "NextScene";

    void OnEnable()
    {
        // ���������ƥ�䣬ֱ�Ӳ��Ŷ���
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

    // �����¼����õķ���
    public void OnAnimationComplete()
    {
        SceneTransitionManager1.Instance.LoadSceneWithTransition(targetSceneName);
    }
}