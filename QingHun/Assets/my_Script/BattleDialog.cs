using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleDialogManager : MonoBehaviour
{
    public static BattleDialogManager Instance;

    [Header("UI Elements")]
    public GameObject dialogPanel;
    public TMP_Text nameText;
    public TMP_Text dialogText;
    public Image leftCharacter;
    public Image rightCharacter;

    [Header("Settings")]
    [Range(0.01f, 0.2f)]
    public float typingSpeed = 0.05f;
    public float displayDuration = 2f;

    private Coroutine typingCoroutine;
    private bool isTyping = false;
    private Queue<DialogData> dialogQueue = new Queue<DialogData>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        dialogPanel.SetActive(false);
    }

    public void ShowBossDialog(string characterName, string dialog, Sprite characterSprite, bool isLeft)
    {
        dialogQueue.Enqueue(new DialogData
        {
            characterName = characterName,
            dialog = dialog,
            characterSprite = characterSprite,
            isLeft = isLeft
        });

        if (!isTyping && dialogQueue.Count == 1)
        {
            StartCoroutine(ProcessDialogQueue());
        }
    }

    private IEnumerator ProcessDialogQueue()
    {
        while (dialogQueue.Count > 0)
        {
            var currentDialog = dialogQueue.Peek();

            // 设置UI
            dialogPanel.SetActive(true);
            nameText.text = currentDialog.characterName;

            if (currentDialog.isLeft)
            {
                leftCharacter.sprite = currentDialog.characterSprite;
                leftCharacter.gameObject.SetActive(true);
                rightCharacter.gameObject.SetActive(false);
            }
            else
            {
                rightCharacter.sprite = currentDialog.characterSprite;
                rightCharacter.gameObject.SetActive(true);
                leftCharacter.gameObject.SetActive(false);
            }

            // 打字机效果
            isTyping = true;
            dialogText.text = "";
            foreach (char letter in currentDialog.dialog.ToCharArray())
            {
                dialogText.text += letter;
                yield return new WaitForSeconds(typingSpeed);
            }
            isTyping = false;

            // 显示一段时间
            yield return new WaitForSeconds(displayDuration);

            // 关闭对话框
            dialogPanel.SetActive(false);
            dialogQueue.Dequeue();
        }
    }

    public void SkipTyping()
    {
        if (isTyping && typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            isTyping = false;
            dialogText.text = dialogQueue.Peek().dialog;
        }
    }

    private class DialogData
    {
        public string characterName;
        public string dialog;
        public Sprite characterSprite;
        public bool isLeft;
    }

    public bool IsDialogActive()
{
    return dialogPanel.activeSelf || dialogQueue.Count > 0;
}
}