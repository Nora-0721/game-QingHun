using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using System.Collections;

public class VideoController6 : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public GameObject rawImage;
    public string targetSceneName = "NextScene";

    void Start()
    {
        if (videoPlayer == null)
        {
            Debug.LogError("VideoController: videoPlayer 未设置！请在Inspector中设置视频播放器引用");
            return;
        }

        if (rawImage == null)
        {
            Debug.LogError("VideoController: rawImage 未设置！请在Inspector中设置RawImage引用");
            return;
        }

        rawImage.SetActive(false); // 初始时隐藏视频画面
        // 开始协程，四秒后播放视频
        StartCoroutine(PlayVideoAfterDelay());
    }


    IEnumerator PlayVideoAfterDelay()
    {
        // 等待四秒
        yield return new WaitForSeconds(0.1f);

        // 重置视频播放器状态
        videoPlayer.Stop();
        videoPlayer.playOnAwake = false;

        Debug.Log("开始准备视频... 视频URL: " + videoPlayer.url);

        // 准备视频
        videoPlayer.Prepare();

        // 等待视频准备完成
        while (!videoPlayer.isPrepared)
        {
            Debug.Log("视频准备中...");
            yield return null;
        }

        Debug.Log("视频准备完成！开始播放，视频长度: " + videoPlayer.length + "秒");

        // 显示画面并播放视频
        rawImage.SetActive(true);
        videoPlayer.Play();

        // 监听视频播放结束事件
        videoPlayer.loopPointReached += OnVideoEnd;
    }

    private void OnVideoEnd(VideoPlayer vp)
    {
        Debug.Log("VideoController: 视频播放结束");

        // 隐藏画面
        if (rawImage != null)
        {
            rawImage.SetActive(false);
        }

        // 切换场景
        SceneManager.LoadScene(targetSceneName);

        // 取消监听事件
        videoPlayer.loopPointReached -= OnVideoEnd;
    }
}