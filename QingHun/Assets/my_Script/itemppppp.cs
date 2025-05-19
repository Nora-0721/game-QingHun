using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

public class PanelController : MonoBehaviour
{
    public GameObject panel; // ���������
    public Button triggerButton; // ���������İ�ť
    public TMP_Text dialogText; // ������ʾ�Ի����ı����
    public string targetSceneName = "NextScene"; // ��ת��Ŀ�곡������
    public float fadeInDuration = 1.0f; // ���붯���ĳ���ʱ��

    private CanvasGroup canvasGroup;
    private string dialogContent = "����һ���Ի�ʾ����"; // �Ի�����

    void Start()
    {
        // ����ʼ״̬Ϊ����
        panel.SetActive(false);

        // ��ȡ CanvasGroup ���
        canvasGroup = panel.AddComponent<CanvasGroup>();
        canvasGroup.alpha = 0f; // ��ʼ͸����Ϊ 0
        canvasGroup.interactable = false; // ��ʼ���ɽ���

        if (triggerButton != null)
        {
            // Ϊ��ť��ӵ���¼�������
            triggerButton.onClick.AddListener(ShowPanel);
        }
        else
        {
            Debug.LogError("PanelController: triggerButton δ���ã�����Inspector������Button����");
        }

        // ���Ի��ı�����Ƿ�����
        if (dialogText == null)
        {
            Debug.LogError("PanelController: dialogText δ���ã�����Inspector������Text�������");
        }
    }

    // �����ťʱ������岢��ʾ�Ի�
    public void ShowPanel()
    {
        panel.SetActive(true);
        Debug.Log("����ѵ���");

        // ���öԻ��ı�
        if (dialogText != null)
        {
            dialogText.text = dialogContent;
        }

        // ��ʼ���붯��
        StartCoroutine(FadeInPanel());

        // 30����Զ�������岢��ת����
        StartCoroutine(DelayedHideAndLoadScene());
    }

    // ���붯��
    IEnumerator FadeInPanel()
    {
        float elapsedTime = 0f;
        while (elapsedTime < fadeInDuration)
        {
            canvasGroup.alpha = elapsedTime / fadeInDuration;
            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }
        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
    }

    // 30����Զ�������岢��ת����
    IEnumerator DelayedHideAndLoadScene()
    {
        yield return new WaitForSeconds(30f);

        // ��ʼ��������
        StartCoroutine(FadeOutPanelAndLoadScene());
    }

    // ������������ת����
    IEnumerator FadeOutPanelAndLoadScene()
    {
        // ������彻��
        canvasGroup.interactable = false;

        // ���������߼�
        float elapsedTime = 0f;
        float startAlpha = canvasGroup.alpha;

        while (elapsedTime < fadeInDuration)
        {
            canvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, elapsedTime / fadeInDuration);
            elapsedTime += Time.unscaledDeltaTime; // ʹ�ò���ʱ������Ӱ�������ʱ��
            yield return null;
        }

        // ȷ����ȫ͸��
        canvasGroup.alpha = 0f;
        panel.SetActive(false);
        Debug.Log("��彥�����");

        // ������ת��֤�߼�
        if (string.IsNullOrEmpty(targetSceneName))
        {
            Debug.LogError("��������δ���ã�����Inspector��ָ��Ŀ�곡������");
            yield break;
        }

        // ʹ��Unity���ó��������Լ��
        if (Application.CanStreamedLevelBeLoaded(targetSceneName))
        {
            Debug.Log($"���ڼ��س���: {targetSceneName}");
            SceneManager.LoadScene(targetSceneName);
        }
        else
        {
            Debug.LogError($"���� '{targetSceneName}' ��������Build Settings�����飺");
            Debug.LogError("- ��������ƴд�����ִ�Сд��");
            Debug.LogError("- Build Settings�еĳ����б����ֶ���ӣ�");
            Debug.LogError("- �����ļ���չ����Build Settings�в�Ӧ����.unity��׺��");
        }

        yield break; // ��ȷ����Э��
    }
}