// CombinedPhotoAnimator.cs
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
public class CombinedPhotoAnimator : MonoBehaviour
{
    [Header("动画配置")]
    [SerializeField] private Image[] photos;    // 所有图片对象
    [SerializeField] private float fadeDuration = 1f;  // 单次淡入淡出时间
    [SerializeField] private float displayDuration = 2f; // 图片保持显示时间

    void Start()
    {
        // 同时启动所有图片的动画
        foreach (var photo in photos)
        {
            StartCoroutine(PlayPhotoAnimation(photo));
        }
    }

    private IEnumerator PlayPhotoAnimation(Image photo)
    {
        // 随机偏移量（避免完全同步）
        float offset = Random.Range(0f, 0.3f);
        yield return new WaitForSeconds(offset);

        while (true)
        {
            // 淡入
            yield return StartCoroutine(Fade(photo, 0f, 1f, fadeDuration));
            yield return new WaitForSeconds(displayDuration);

            // 淡出
            yield return StartCoroutine(Fade(photo, 1f, 0f, fadeDuration));
            yield return new WaitForSeconds(displayDuration);
        }
    }

    private IEnumerator Fade(Image target, float startAlpha, float endAlpha, float duration)
    {
        float elapsed = 0f;
        Color color = target.color;

        while (elapsed < duration)
        {
            color.a = Mathf.Lerp(startAlpha, endAlpha, elapsed / duration);
            target.color = color;
            elapsed += Time.deltaTime;
            yield return null;
        }

        color.a = endAlpha;
        target.color = color;
    }
}