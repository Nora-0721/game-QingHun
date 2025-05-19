using UnityEngine;

public class SceneAutoTransition : MonoBehaviour
{
    private float startTime;
    public string targetSceneName = "SampleScene"; // �ڴ˴���дĿ�곡������

    void Start()
    {
        startTime = Time.time;
    }

    void Update()
    {
        // ����Ƿ�ﵽ4����û�����ڽ��еĹ���
        if (Time.time - startTime >= 4f)
        {
            // ���ó������ɹ������л�����
            SceneTransitionManager1.Instance.LoadSceneWithTransition(targetSceneName);

            // ���ýű���ֹ�ظ�����
            enabled = false;
        }
    }
}