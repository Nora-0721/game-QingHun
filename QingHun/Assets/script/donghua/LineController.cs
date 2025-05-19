// LineController.cs
using UnityEngine;

public class LineController : MonoBehaviour
{
    [Header("��������")]
    [Tooltip("�ٶȱ仯���ߣ�X�᣺��׼��ʱ�� 0-1��Y�᣺���ȱ��� 0-1��")]
    public AnimationCurve progressCurve;

    [Tooltip("�ܶ���ʱ�䣨�룩")]
    public float totalDuration = 3f;

    [Header("����")]
    public bool showDebug = true;

    private float timer;
    private Vector3 initialScale;
    public float currentProgress = 0f;
    void Start()
    {
        initialScale = transform.localScale;
        InitializeLine();
    }

    void Update()
    {
        if (timer < totalDuration)
        {
            timer += Time.deltaTime;
            UpdateLineProgress();
        }
    }

    private void InitializeLine()
    {
        // ��ʼ״̬����ȫ����
        transform.localScale = new Vector3(0, initialScale.y, initialScale.z);
    }

    private void UpdateLineProgress()
    {
        // �����׼��ʱ�䣨0-1��
        float normalizedTime = Mathf.Clamp01(timer / totalDuration);

        // ͨ�����߻�ȡ��ǰ����
        float currentProgress = progressCurve.Evaluate(normalizedTime);

        // Ӧ�ú������ţ����������ߣ�
        transform.localScale = new Vector3(
            initialScale.x * currentProgress,
            initialScale.y,
            initialScale.z
        );

        if (showDebug)
        {
            Debug.Log($"���ȣ�{currentProgress:P0} ʱ�䣺{timer:F2}s");
        }
    }

    // ��ѡ�����ö���
    public void ResetAnimation()
    {
        timer = 0f;
        InitializeLine();
    }
}