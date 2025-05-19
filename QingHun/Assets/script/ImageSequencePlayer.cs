using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ImageSequencePlayer : MonoBehaviour
{
    [Header("ͼƬ����")]
    public List<Sprite> frames; // ��������ͼƬ֡

    [Header("��������")]
    public Image displayImage;  // ������ʾͼƬ��UI Image���
    public float frameRate = 24f; // ÿ�벥��֡��
    public bool loop = true;     // �Ƿ�ѭ������

    private int currentFrame = 0;
    private bool isPlaying = false;

    void Start()
    {
        if (frames == null || frames.Count == 0)
        {
            Debug.LogError("δ����ͼƬ֡��");
            return;
        }

        StartCoroutine(PlayAnimation());
    }

    IEnumerator PlayAnimation()
    {
        isPlaying = true;
        while (loop || currentFrame < frames.Count)
        {
            // ���µ�ǰ֡
            displayImage.sprite = frames[currentFrame];
            currentFrame = (currentFrame + 1) % frames.Count;

            // ����֡���ʱ��
            float delay = 1f / frameRate;
            yield return new WaitForSeconds(delay);

            // ��ѭ��ģʽ�Ҳ������ʱ�˳�
            if (!loop && currentFrame == 0) break;
        }
        isPlaying = false;
    }

    // �ⲿ����������������ѡ��
    public void Restart()
    {
        if (isPlaying) return;
        currentFrame = 0;
        StartCoroutine(PlayAnimation());
    }
}