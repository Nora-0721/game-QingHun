using UnityEngine;
using UnityEngine.UI;

public class SimpleTeamPanel2 : MonoBehaviour
{
    [Header("�Ŷӳ�Ա���")]
    public GameObject teamPanel;  // �Ŷӳ�Ա��Ϣ���
    public Button closeButton;    // �رհ�ť

    private void Start()
    {
        // ��ʼ�����Ŷӳ�Ա���
        if (teamPanel != null)
            teamPanel.SetActive(false);

        // ���ùرհ�ť�¼�
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(CloseTeamPanel);

            // ��Ӱ�ťЧ�����
            if (!closeButton.GetComponent<SimpleButtonEffect>())
            {
                closeButton.gameObject.AddComponent<SimpleButtonEffect>();
            }
        }
    }

    // ���Ŷӳ�Ա���
    public void OpenTeamPanel()
    {
        if (teamPanel != null)
            teamPanel.SetActive(true);
    }

    // �ر��Ŷӳ�Ա���
    public void CloseTeamPanel()
    {
        if (teamPanel != null)
            teamPanel.SetActive(false);
    }
}