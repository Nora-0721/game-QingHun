using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    [Header("音乐设置")]
    public AudioClip musicClip; // 音乐片段
    private AudioSource audioSource;

    private void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>(); // 初始化音频源
        audioSource.clip = musicClip; // 设置音乐片段
        audioSource.loop = true; // 设置为循环播放
        audioSource.Play(); // 播放音乐
    }
}