// CombinedPhotoAnimator.cs
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
public class CombinedPhotoAnimator : MonoBehaviour
{
    [Header("��������")]
    [SerializeField] private Image[] photos;    // ����ͼƬ����
    [SerializeField] private float fadeDuration = 1f;  // ���ε��뵭��ʱ��
    [SerializeField] private float displayDuration = 2f; // ͼƬ������ʾʱ��

    void Start()
    {
        // ͬʱ��������ͼƬ�Ķ���
        foreach (var photo in photos)
        {
            StartCoroutine(PlayPhotoAnimation(photo));
        }
    }

    private IEnumerator PlayPhotoAnimation(Image photo)
    {
        // ���ƫ������������ȫͬ����
        float offset = Random.Range(0f, 0.3f);
        yield return new WaitForSeconds(offset);

        while (true)
        {
            // ����
            yield return StartCoroutine(Fade(photo, 0f, 1f, fadeDuration));
            yield return new WaitForSeconds(displayDuration);

            // ����
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