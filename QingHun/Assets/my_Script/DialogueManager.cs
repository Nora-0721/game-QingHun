using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI; // ����Image�������

[System.Serializable]
public class DialogueEntry
{
    public string C;
    public string D;
}

public class DialogueManager : MonoBehaviour // ȷ���̳�MonoBehaviour
{
    [Header("UI References")]
    public GameObject leftPanel;    // ���A+C�ĸ�����
    public GameObject rightPanel;   // �Ҳ�B+D�ĸ�����
    public TMP_Text textC;          // ����ı���
    public TMP_Text textD;          // �Ҳ��ı���
    public Image spiritToChange;    // ��Ҫ�任�ľ���Image���
    public Sprite originalSprite;   // ԭʼ����ͼ��
    public Sprite transformedSprite;// �任��ľ���ͼ��

    [Header("Settings")]
    public float charDelay = 0.05f; // ������ʾ���
    public float switchDelay = 10f;// �л��Ի����
    public float spriteRevertDelay = 0.5f; // ͼ���ָ��ӳ�

    private List<DialogueEntry> dialogues = new List<DialogueEntry>();
    private bool isDialogueActive;
    private int specialDialogueIndex = 5; // ����Ի�������(��0��ʼ)

    void Start()
    {
        InitializeDialogues();
        StartCoroutine(RunDialogues());
        // ��ʼ���������ı�
        spiritToChange.sprite = originalSprite; // ȷ����ʼΪԭͼ��
    }

    void InitializeDialogues()
    {
        // ���ݱ�����ݳ�ʼ���Ի�
        dialogues = new List<DialogueEntry> {
            new DialogueEntry{C="�����꣡��ִ�����������ǧ�ꡣ", D=""},
            new DialogueEntry{C="", D="���ޣ��˼�������Ը������˭�ˆ��ۣ������ķ���£�����������죡"},
            new DialogueEntry{C="", D="��ƾʲôҪ�����ף�ʲô�𷨴ȱ���ȴ����Ѫ���顣"},
            new DialogueEntry{C="", D="��㣬����ˮ����ɽ�������Ų����ˡ�"},
            new DialogueEntry{C="��ң�����Ϊһ��֮˽�����������������շ�ħ�ȣ����������㣡", D=""},
            new DialogueEntry{C="", D="�Ұٰ����̣���������ơ�Ҳ�գ����н�շ�ħ�ȣ�������ؽ��B��һ˫��������ƴһ�����ⷨ���ǧ�С��ܵ�������������Ե��Ե�ۣ���δ�룡"},
        };
    }

    IEnumerator RunDialogues()
    {
        isDialogueActive = true;
        bool showLeft = true;

        for (int i = 0; i < dialogues.Count; i++)
        {
            var entry = dialogues[i];

            // �Զ�������� ---------------------------
            string currentText;
            TMP_Text targetText;
            int retryCount = 0;

            do
            {
                currentText = showLeft ? entry.C : entry.D;
                targetText = showLeft ? textC : textD;

                if (string.IsNullOrEmpty(currentText)) {
                    showLeft = !showLeft; // �Զ��л�����
                    retryCount++;
                }
            } while (string.IsNullOrEmpty(currentText) && retryCount < 2);
            // ��ೢ���������� ---------------------------------

            // �������
            leftPanel.SetActive(showLeft);
            rightPanel.SetActive(!showLeft);
            textC.gameObject.SetActive(showLeft);
            textD.gameObject.SetActive(!showLeft);

            // ����ı�
            textC.text = textD.text = "";


            // ����Ի�����
            if (i == specialDialogueIndex)
            {
                // �任����ͼ��
                spiritToChange.sprite = transformedSprite;
                
            }

            targetText.text = currentText;
            //// ������ʾ
            //foreach (char c in currentText)
            //{
            //    targetText.text += c;
            //    yield return new WaitForSeconds(charDelay);
            //}

            yield return new WaitForSeconds(switchDelay);
            showLeft = !showLeft;

            // �ָ�����ͼ��
            if (i == specialDialogueIndex)
            {
                yield return new WaitForSeconds(spriteRevertDelay);
                spiritToChange.sprite = originalSprite;
                leftPanel.SetActive(false);
                rightPanel.SetActive(false);
                textC.gameObject.SetActive(false);
                textD.gameObject.SetActive(false);
            }

            yield return new WaitForSeconds(switchDelay);
            showLeft = !showLeft;
        }

        // �����Ի�ʱ��������Ԫ��
        leftPanel.SetActive(false);
        rightPanel.SetActive(false);
        textC.gameObject.SetActive(false);
        textD.gameObject.SetActive(false);
        isDialogueActive = false;
    }
}