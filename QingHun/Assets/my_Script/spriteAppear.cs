using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // 添加此行以使用 UI

public class ClickToReveal : MonoBehaviour
{
    public string scenename;

    [Header("目标Sprite")]
    public SpriteRenderer targetSprite; // 要浮现的Sprite

    [Header("浮现设置")]
    public float fadeDuration = 1.0f; // 淡入持续时间
    public bool startInvisible = true; // 是否初始不可见

    [Header("音乐设置")]
    public AudioClip musicClip; // 音乐片段
    private AudioSource audioSource;

    [Header("过渡设置")]
    public Image transitionImage; // 用于场景过渡的黑色图像

    private void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>(); // 初始化音频源

        if (targetSprite == null)
        {
            Debug.LogError("未指定目标Sprite!");
            return;
        }

        // 初始状态设置
        if (startInvisible)
        {
            Color color = targetSprite.color;
            color.a = 0f;
            targetSprite.color = color;
        }

        // 初始化过渡图像为全透明
        if (transitionImage != null)
        {
            Color imgColor = transitionImage.color;
            imgColor.a = 0f;
            transitionImage.color = imgColor;
        }
    }

    private void OnMouseDown()
    {
        if (targetSprite != null)
        {
            StartCoroutine(FadeInSprite());
        }
    }

    IEnumerator FadeInSprite()
    {
        float elapsedTime = 0f;
        Color originalColor = targetSprite.color;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration);

            Color newColor = originalColor;
            newColor.a = alpha;
            targetSprite.color = newColor;

            yield return null;
        }

        // 确保最终完全显示
        Color finalColor = originalColor;
        finalColor.a = 1f;
        targetSprite.color = finalColor;

        // 等待 5 秒
        yield return new WaitForSeconds(0.1f);

        // 播放音乐
        audioSource.clip = musicClip;
        audioSource.volume = 0.4f;
        audioSource.Play();

        // 等待音乐播放一小段时间（例如 3 秒）
        yield return new WaitForSeconds(7f);

        // 开始场景过渡
        yield return StartCoroutine(FadeToBlack());

        // 加载新场景
        if(scenename!="none")
        SceneManager.LoadScene(scenename); // 替换为你的场景名称
    }

    IEnumerator FadeToBlack()
    {
        if (transitionImage != null)
        {
            float elapsedTime = 0f;
            while (elapsedTime < fadeDuration)
            {
                elapsedTime += Time.deltaTime;
                float alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration);

                Color imgColor = transitionImage.color;
                imgColor.a = alpha;
                transitionImage.color = imgColor;

                yield return null;
            }

            // 确保最终完全黑色
            Color finalColor = transitionImage.color;
            finalColor.a = 1f;
            transitionImage.color = finalColor;

            // 等待一小段时间
            yield return new WaitForSeconds(0.5f);

            // 从黑色变回透明
            elapsedTime = 0f;
            while (elapsedTime < fadeDuration)
            {
                elapsedTime += Time.deltaTime;
                float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);

                Color imgColor = transitionImage.color;
                imgColor.a = alpha;
                transitionImage.color = imgColor;

                yield return null;
            }
        }
    }
}