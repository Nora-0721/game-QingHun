using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI; // 新增Image组件引用

[System.Serializable]
public class DialogueEntry
{
    public string C;
    public string D;
}

public class DialogueManager : MonoBehaviour // 确保继承MonoBehaviour
{
    [Header("UI References")]
    public GameObject leftPanel;    // 左侧A+C的父物体
    public GameObject rightPanel;   // 右侧B+D的父物体
    public TMP_Text textC;          // 左侧文本框
    public TMP_Text textD;          // 右侧文本框
    public Image spiritToChange;    // 需要变换的精灵Image组件
    public Sprite originalSprite;   // 原始精灵图案
    public Sprite transformedSprite;// 变换后的精灵图案

    [Header("Settings")]
    public float charDelay = 0.05f; // 单字显示间隔
    public float switchDelay = 10f;// 切换对话间隔
    public float spriteRevertDelay = 0.5f; // 图案恢复延迟

    private List<DialogueEntry> dialogues = new List<DialogueEntry>();
    private bool isDialogueActive;
    private int specialDialogueIndex = 5; // 特殊对话的索引(从0开始)

    void Start()
    {
        InitializeDialogues();
        StartCoroutine(RunDialogues());
        // 初始隐藏所有文本
        spiritToChange.sprite = originalSprite; // 确保初始为原图案
    }

    void InitializeDialogues()
    {
        // 根据表格数据初始化对话
        dialogues = new List<DialogueEntry> {
            new DialogueEntry{C="白素贞！你执念成孽，枉修千年。", D=""},
            new DialogueEntry{C="", D="我呸！人家你情我愿，碍了谁人嘅眼，关了哪佛的事，逆了哪里的天！"},
            new DialogueEntry{C="", D="你凭什么要来主宰！什么佛法慈悲，却是冷血无情。"},
            new DialogueEntry{C="", D="姐姐，我们水漫金山，看他放不放人。"},
            new DialogueEntry{C="你敢？你再为一己之私扰攘众生，看我这金刚伏魔杖，定必收了你！", D=""},
            new DialogueEntry{C="", D="我百般隐忍，你屡屡相逼。也罢！你有金刚伏魔杖，我有天地揭諦剑一双。今尽与你拼一场看这法理规千行。管得了众生凡心向。缘生缘聚，情未央！"},
        };
    }

    IEnumerator RunDialogues()
    {
        isDialogueActive = true;
        bool showLeft = true;

        for (int i = 0; i < dialogues.Count; i++)
        {
            var entry = dialogues[i];

            // 自动方向纠正 ---------------------------
            string currentText;
            TMP_Text targetText;
            int retryCount = 0;

            do
            {
                currentText = showLeft ? entry.C : entry.D;
                targetText = showLeft ? textC : textD;

                if (string.IsNullOrEmpty(currentText)) {
                    showLeft = !showLeft; // 自动切换方向
                    retryCount++;
                }
            } while (string.IsNullOrEmpty(currentText) && retryCount < 2);
            // 最多尝试左右两侧 ---------------------------------

            // 激活面板
            leftPanel.SetActive(showLeft);
            rightPanel.SetActive(!showLeft);
            textC.gameObject.SetActive(showLeft);
            textD.gameObject.SetActive(!showLeft);

            // 清空文本
            textC.text = textD.text = "";


            // 特殊对话处理
            if (i == specialDialogueIndex)
            {
                // 变换精灵图案
                spiritToChange.sprite = transformedSprite;
                
            }

            targetText.text = currentText;
            //// 逐字显示
            //foreach (char c in currentText)
            //{
            //    targetText.text += c;
            //    yield return new WaitForSeconds(charDelay);
            //}

            yield return new WaitForSeconds(switchDelay);
            showLeft = !showLeft;

            // 恢复精灵图案
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

        // 结束对话时隐藏所有元素
        leftPanel.SetActive(false);
        rightPanel.SetActive(false);
        textC.gameObject.SetActive(false);
        textD.gameObject.SetActive(false);
        isDialogueActive = false;
    }
}