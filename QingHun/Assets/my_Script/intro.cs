using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneIntroManager : MonoBehaviour
{
    public GameObject introPanel; // 介绍框的 UI 面板
    public Button exitButton;      // 退出按钮
    public float fadeDuration = 1f; // 淡入淡出持续时间
    public string NextScene;

    private CanvasGroup canvasGroup; // 用于控制透明度

    void Start()
    {
        canvasGroup = introPanel.GetComponent<CanvasGroup>();
        introPanel.SetActive(true);
        StartCoroutine(FadeIn());

        exitButton.onClick.AddListener(OnExitButtonClicked);
    }

    private IEnumerator FadeIn()
    {
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            canvasGroup.alpha = Mathf.Lerp(0, 1, elapsedTime / fadeDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = 1; // 确保最终透明度为 1
    }

    public void OnExitButtonClicked()
    {
        StartCoroutine(FadeOut());
    }

    private IEnumerator FadeOut()
    {
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            canvasGroup.alpha = Mathf.Lerp(1, 0, elapsedTime / fadeDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = 0; // 确保最终透明度为 0
        SceneManager.LoadScene(NextScene);
    }
}