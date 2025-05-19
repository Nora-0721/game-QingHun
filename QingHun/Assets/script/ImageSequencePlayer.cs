using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ImageSequencePlayer : MonoBehaviour
{
    [Header("图片序列")]
    public List<Sprite> frames; // 拖入所有图片帧

    [Header("播放设置")]
    public Image displayImage;  // 用于显示图片的UI Image组件
    public float frameRate = 24f; // 每秒播放帧数
    public bool loop = true;     // 是否循环播放

    private int currentFrame = 0;
    private bool isPlaying = false;

    void Start()
    {
        if (frames == null || frames.Count == 0)
        {
            Debug.LogError("未设置图片帧！");
            return;
        }

        StartCoroutine(PlayAnimation());
    }

    IEnumerator PlayAnimation()
    {
        isPlaying = true;
        while (loop || currentFrame < frames.Count)
        {
            // 更新当前帧
            displayImage.sprite = frames[currentFrame];
            currentFrame = (currentFrame + 1) % frames.Count;

            // 计算帧间隔时间
            float delay = 1f / frameRate;
            yield return new WaitForSeconds(delay);

            // 非循环模式且播放完毕时退出
            if (!loop && currentFrame == 0) break;
        }
        isPlaying = false;
    }

    // 外部调用重启动画（可选）
    public void Restart()
    {
        if (isPlaying) return;
        currentFrame = 0;
        StartCoroutine(PlayAnimation());
    }
}