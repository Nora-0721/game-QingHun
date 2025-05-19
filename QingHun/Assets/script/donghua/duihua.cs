using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class SimpleDialogAndTransition : MonoBehaviour
{
    public TMP_Text dialogText;
    public string[] dialogLines;
    public GameObject spriteGameObject;
    public string nextSceneName = "SampleScene";

    private int currentLineIndex = 0;
    private bool isTyping = false;

    private void Start()
    {
        // ��ʼ���Ի���
        dialogLines = new string[] {
            "gogogo��"
        };

        // ��ʼ��ʾ��һ�жԻ�
        StartCoroutine(DisplayDialogLine(dialogLines[0]));
    }

    private IEnumerator DisplayDialogLine(string text)
    {
        isTyping = true;
        dialogText.text = "";

        foreach (char c in text)
        {
            dialogText.text += c;
            yield return new WaitForSeconds(0.05f); // �����ٶ�
        }

        isTyping = false;
    }

    private void Update()
    {
        // ��������
        if (Input.GetMouseButtonDown(0) && !isTyping)
        {
            if (currentLineIndex < dialogLines.Length - 1)
            {
                currentLineIndex++;
                StartCoroutine(DisplayDialogLine(dialogLines[currentLineIndex]));
            }
            else
            {
                // ���жԻ���ɺ������һ������
                LoadNextScene();
            }
        }
    }

    private void LoadNextScene()
    {
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            StartCoroutine(LoadSceneWithFade(nextSceneName));
        }
        else
        {
            Debug.LogError("��һ����������δ���ã�");
        }
    }

    private IEnumerator LoadSceneWithFade(string sceneName)
    {
        // ����Ч��
        yield return StartCoroutine(FadeOut());

        // ���س���
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = true;
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        // ����Ч���������Ҫ�Ļ���
        yield return StartCoroutine(FadeIn());
    }

    private IEnumerator FadeOut()
    {
        float duration = 1.0f;
        float elapsedTime = 0;
        Color textColor = dialogText.color;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1, 0, elapsedTime / duration);
            textColor.a = alpha;
            dialogText.color = textColor;
            spriteGameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, alpha);
            yield return null;
        }

        dialogText.color = new Color(1, 1, 1, 0);
        spriteGameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);
    }

    private IEnumerator FadeIn()
    {
        float duration = 1.0f;
        float elapsedTime = 0;
        Color textColor = dialogText.color;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(0, 1, elapsedTime / duration);
            textColor.a = alpha;
            dialogText.color = textColor;
            spriteGameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, alpha);
            yield return null;
        }

        dialogText.color = new Color(1, 1, 1, 1);
        spriteGameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
    }
}