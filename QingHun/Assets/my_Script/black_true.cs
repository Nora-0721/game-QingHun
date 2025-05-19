using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class BlackScreenManager : MonoBehaviour
{
    [Header("UI References")]
    public static BlackScreenManager Instance;
    public Image blackScreen;
    public TMP_Text centerText;

    [Header("Settings")]
    public float fadeDuration = 1f;

    private Coroutine currentRoutine;
    private bool shouldFadeOut = false;
    private string nextNormalDialogId; // 用于记录下一行正常对话的ID
    private bool isWaitingForClick = false;
    private bool shouldFadeOutAfterClick = false;
    private void Awake() => Instance = this;

    [Range(0.01f, 0.2f)]
    public float typingSpeed = 0.1f;
    private Coroutine typingCoroutine;

    private void Start()
    {
        blackScreen.color = new Color(0, 0, 0, 0);
        centerText.alpha = 0;
        blackScreen.gameObject.SetActive(false);
        centerText.gameObject.SetActive(false);
    }

    public void ShowBlackScreen(string text, bool isLastEmptyLine)
    {
        DiaLogmanager.Instance.dialogText.text = "";

        // 停止当前所有协程
        if (currentRoutine != null)
        {
            StopCoroutine(currentRoutine);
        }
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        shouldFadeOutAfterClick = isLastEmptyLine;

        if (!blackScreen.gameObject.activeSelf)
        {
            blackScreen.gameObject.SetActive(true);
            centerText.gameObject.SetActive(true);
            currentRoutine = StartCoroutine(FadeInThenType(text));
        }
        else
        {
            currentRoutine = StartCoroutine(TypeThenWait(text));
        }
    }


    IEnumerator FadeEffect(float startAlpha, float endAlpha, float duration)
    {
        float timer = 0f;
        blackScreen.color = new Color(0, 0, 0, startAlpha);
        centerText.alpha = startAlpha;

        while (timer < duration)
        {
            float alpha = Mathf.Lerp(startAlpha, endAlpha, timer / duration);
            blackScreen.color = new Color(0, 0, 0, alpha);
            centerText.alpha = alpha;
            timer += Time.deltaTime;
            yield return null;
        }

        blackScreen.color = new Color(0, 0, 0, endAlpha);
        centerText.alpha = endAlpha;
    }

    IEnumerator WaitForClick()
    {
        isWaitingForClick = true;

        // 等待点击
        yield return new WaitUntil(() => Input.GetMouseButtonDown(0));

        isWaitingForClick = false;

        if (shouldFadeOutAfterClick)
        {
            // 淡出效果
            yield return StartCoroutine(FadeEffect(1f, 0f, fadeDuration));
            blackScreen.gameObject.SetActive(false);
            centerText.gameObject.SetActive(false);
            centerText.text = "";

            // 通知继续对话
            DiaLogmanager.Instance.ContinueDialog();
        }
        else
        {
            // 直接继续下一黑屏文本
            DiaLogmanager.Instance.ContinueToNextBlackScreen();
        }
    }


    IEnumerator FadeInThenType(string text)
    {
        // 淡入效果
        yield return StartCoroutine(FadeEffect(0f, 1f, fadeDuration));
        yield return StartCoroutine(TypeText(text));

        // 如果是最后一个"无"行，等待点击后再淡出
        if (shouldFadeOutAfterClick)
        {
            yield return StartCoroutine(WaitForClick());
        }
    }

    IEnumerator TypeText(string text)
    {
        centerText.text = "";
        foreach (char letter in text.ToCharArray())
        {
            centerText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        // 仅当不是最后一个"无"行时才自动等待点击
        if (!shouldFadeOutAfterClick)
        {
            StartCoroutine(WaitForClick());
        }
    }

    IEnumerator TypeThenWait(string text)
    {
        yield return StartCoroutine(TypeText(text));
        yield return StartCoroutine(WaitForClick());
    }
}