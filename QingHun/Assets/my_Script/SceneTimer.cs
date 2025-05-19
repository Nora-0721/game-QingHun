// SceneTimer.cs
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
public class SceneTimer : MonoBehaviour
{
    [Header("��������")]
    public string targetSceneName = "NextScene";
    public float delayTime = 3f; // �ӳ�ʱ�䣨�룩

    void Start()
    {
        StartCoroutine(DelaySceneTransition());
    }

    private IEnumerator DelaySceneTransition()
    {
        Debug.Log($"��������{delayTime}����л�");
        yield return new WaitForSeconds(delayTime);

        // ͨ���������ɹ������л�����
        if (SceneTransitionManager1.Instance != null)
        {
            SceneTransitionManager1.Instance.LoadSceneWithTransition(targetSceneName);
        }
        else
        {
            Debug.LogError("δ�ҵ��������ɹ�����");
            SceneManager.LoadScene(targetSceneName);
        }
    }
}