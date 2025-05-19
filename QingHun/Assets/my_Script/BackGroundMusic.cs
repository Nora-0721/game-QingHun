//using UnityEngine;
//using UnityEngine.SceneManagement;
//using System.Collections;
//using System.Collections.Generic;

//public class BackGroundMusic : MonoBehaviour
//{
//    public static BackGroundMusic Instance { get; private set; }
//    private AudioSource audioSource;

//    // 临时音乐播放器
//    private AudioSource tempAudioSource;

//    [System.Serializable]
//    public class SceneMusicGroup
//    {
//        public string groupName;
//        public AudioClip musicClip;
//        public string[] sceneNames;
//    }

//    [Header("音乐分组设置")]
//    public SceneMusicGroup[] musicGroups;

//    [Header("过渡设置")]
//    public float fadeDuration = 1.5f;

//    [Header("音量设置")]
//    [Range(0f, 1f)]
//    public float defaultVolume = 1f;

//    [Header("全局选项")]
//    public bool playMusicOnStart = true;
//    public bool persistAcrossScenes = true;

//    private Dictionary<string, AudioClip> sceneToMusicMap = new Dictionary<string, AudioClip>();
//    private AudioClip currentPlayingClip;
//    private bool isMuted = false;
//    private float previousVolume;

//    // 临时音乐相关
//    private AudioClip tempMusicClip;   // 临时播放的音乐
//    private AudioClip pausedMusicClip; // 被暂停的原背景音乐
//    private float pausedMusicTime;     // 被暂停的音乐播放位置
//    private bool isPlayingTempMusic = false;

//    // 特殊关键字，表示不播放音乐
//    private const string NO_MUSIC_KEYWORD = "none";

//    // 在任何场景中调用此静态方法来确保背景音乐管理器存在
//    public static BackGroundMusic CreateMusicManagerIfNeeded()
//    {
//        if (Instance == null)
//        {
//            GameObject musicManagerObject = new GameObject("BackgroundMusicManager");
//            BackGroundMusic musicManager = musicManagerObject.AddComponent<BackGroundMusic>();
//            AudioSource audioSource = musicManagerObject.AddComponent<AudioSource>();
//            audioSource.loop = true;
//            audioSource.playOnAwake = false;
//            Debug.Log("已创建新的BackgroundMusicManager");
//            return musicManager;
//        }
//        return Instance;
//    }

//    void Awake()
//    {
//        if (Instance == null)
//        {
//            Instance = this;

//            if (persistAcrossScenes)
//            {
//                DontDestroyOnLoad(gameObject);
//                Debug.Log("BackGroundMusic: 已设置为不随场景销毁");
//            }

//            audioSource = GetComponent<AudioSource>();
//            if (audioSource == null)
//            {
//                audioSource = gameObject.AddComponent<AudioSource>();
//                Debug.Log("BackGroundMusic: 已添加AudioSource组件");
//            }

//            audioSource.loop = true;
//            audioSource.volume = defaultVolume;
//            previousVolume = defaultVolume;

//            // 创建临时音乐播放器
//            GameObject tempAudioObj = new GameObject("TempMusicPlayer");
//            tempAudioObj.transform.SetParent(transform);
//            tempAudioSource = tempAudioObj.AddComponent<AudioSource>();
//            tempAudioSource.loop = false; // 临时音乐默认不循环
//            tempAudioSource.playOnAwake = false;
//            tempAudioSource.volume = defaultVolume;

//            InitializeSceneMusicMap();

//            SceneManager.sceneLoaded -= OnSceneLoaded;
//            SceneManager.sceneLoaded += OnSceneLoaded;
//            Debug.Log("BackGroundMusic: 已注册场景加载事件");
//        }
//        else
//        {
//            Debug.Log("BackGroundMusic: 已存在实例，销毁当前对象");
//            Destroy(gameObject);
//            return;
//        }
//    }

//    void Start()
//    {
//        if (playMusicOnStart)
//        {
//            PlayMusicForCurrentScene();
//        }
//    }

//    void Update()
//    {
//        // 检查临时音乐是否播放完毕
//        if (isPlayingTempMusic && tempAudioSource != null && !tempAudioSource.isPlaying)
//        {
//            RestoreOriginalMusic();
//        }
//    }

//    void InitializeSceneMusicMap()
//    {
//        sceneToMusicMap.Clear();
//        Debug.Log("初始化场景音乐映射...");

//        foreach (var group in musicGroups)
//        {
//            Debug.Log("处理音乐组: " + group.groupName);

//            // 检查是否是"none"组
//            if (group.groupName.ToLower() == NO_MUSIC_KEYWORD || group.musicClip == null)
//            {
//                foreach (var sceneName in group.sceneNames)
//                {
//                    if (string.IsNullOrEmpty(sceneName)) continue;

//                    // 将场景映射到null表示不播放音乐
//                    sceneToMusicMap[sceneName] = null;
//                    Debug.Log($"场景 '{sceneName}' 设置为无背景音乐");
//                }
//                continue;
//            }

//            foreach (var sceneName in group.sceneNames)
//            {
//                if (string.IsNullOrEmpty(sceneName)) continue;

//                sceneToMusicMap[sceneName] = group.musicClip;
//                Debug.Log("添加场景映射: '" + sceneName + "' -> " + group.musicClip.name);
//            }
//        }

//        Debug.Log("场景音乐映射初始化完成，共 " + sceneToMusicMap.Count + " 个场景");
//    }

//    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
//    {
//        Debug.Log("BackGroundMusic: 场景加载 - " + scene.name);

//        // 如果正在播放临时音乐，不切换场景背景音乐
//        if (isPlayingTempMusic)
//        {
//            Debug.Log("正在播放临时音乐，不切换场景背景音乐");
//            return;
//        }

//        StartCoroutine(DelayedPlayMusic());
//    }

//    IEnumerator DelayedPlayMusic()
//    {
//        yield return null;

//        // 检查场景中是否已有其他音乐在播放
//        if (SceneHasOtherMusicSources())
//        {
//            Debug.Log("检测到场景中已有其他音乐源，不播放背景音乐");
//            yield break;
//        }

//        PlayMusicForCurrentScene();
//    }

//    // 检查场景中是否有其他音乐源
//    private bool SceneHasOtherMusicSources()
//    {
//        // 获取场景中所有的AudioSource（不包括自己）
//        AudioSource[] allAudioSources = FindObjectsOfType<AudioSource>();
//        foreach (AudioSource source in allAudioSources)
//        {
//            // 忽略自己的AudioSource、临时音乐源和不播放的AudioSource
//            if (source != audioSource && source != tempAudioSource && source.isPlaying && source.clip != null)
//            {
//                // 检查这个AudioSource是否是音乐（可以根据需要调整判断条件）
//                if (source.loop || source.clip.length > 10f) // 假设长度大于10秒的是音乐
//                {
//                    Debug.Log($"发现其他音乐源: {source.gameObject.name} 正在播放 {source.clip.name}");
//                    return true;
//                }
//            }
//        }
//        return false;
//    }

//    public void PlayMusicForCurrentScene()
//    {
//        // 如果正在播放临时音乐，不切换
//        if (isPlayingTempMusic)
//        {
//            Debug.Log("正在播放临时音乐，不切换场景背景音乐");
//            return;
//        }

//        string currentSceneName = SceneManager.GetActiveScene().name;
//        Debug.Log("当前场景: " + currentSceneName);

//        if (sceneToMusicMap.TryGetValue(currentSceneName, out AudioClip targetClip))
//        {
//            if (targetClip == null)
//            {
//                Debug.Log("场景配置为不播放背景音乐");
//                StartCoroutine(FadeOutAndStop());
//                return;
//            }

//            Debug.Log("找到了对应的音乐剪辑");
//            if (currentPlayingClip != targetClip || !audioSource.isPlaying)
//            {
//                StartCoroutine(TransitionToMusic(targetClip));
//            }
//        }
//        else
//        {
//            Debug.Log("未找到对应的音乐配置");
//            // 如果没有找到对应的音乐，停止当前播放
//            if (audioSource.isPlaying)
//            {
//                StartCoroutine(FadeOutAndStop());
//            }
//        }
//    }

//    IEnumerator TransitionToMusic(AudioClip newClip)
//    {
//        if (audioSource.isPlaying)
//        {
//            yield return StartCoroutine(FadeOut());
//        }

//        audioSource.clip = newClip;
//        currentPlayingClip = newClip;
//        audioSource.Play();

//        if (!isMuted)
//        {
//            yield return StartCoroutine(FadeIn());
//        }
//    }

//    IEnumerator FadeOut()
//    {
//        float startVolume = audioSource.volume;

//        while (audioSource.volume > 0)
//        {
//            audioSource.volume -= startVolume * Time.deltaTime / fadeDuration;
//            yield return null;
//        }

//        audioSource.Stop();
//        audioSource.volume = isMuted ? 0 : previousVolume;
//    }

//    IEnumerator FadeIn()
//    {
//        float targetVolume = previousVolume;
//        audioSource.volume = 0;

//        while (audioSource.volume < targetVolume)
//        {
//            audioSource.volume += targetVolume * Time.deltaTime / fadeDuration;
//            yield return null;
//        }
//    }

//    IEnumerator FadeOutAndStop()
//    {
//        yield return StartCoroutine(FadeOut());
//        currentPlayingClip = null;
//    }

//    // 临时音乐相关方法

//    // 播放临时音乐，暂停当前背景音乐
//    public void PlayTemporaryMusic(AudioClip tempClip, bool loop = false)
//    {
//        if (tempClip == null)
//        {
//            Debug.LogWarning("尝试播放空的临时音乐");
//            return;
//        }

//        Debug.Log($"播放临时音乐: {tempClip.name}, 循环: {loop}");

//        // 保存当前正在播放的背景音乐状态
//        if (audioSource.isPlaying)
//        {
//            pausedMusicClip = audioSource.clip;
//            pausedMusicTime = audioSource.time;
//            StartCoroutine(FadeOut()); // 渐弱当前音乐
//        }

//        // 设置并播放临时音乐
//        tempAudioSource.clip = tempClip;
//        tempAudioSource.loop = loop;
//        tempAudioSource.volume = 0f;
//        tempAudioSource.Play();

//        // 渐入临时音乐
//        StartCoroutine(FadeInTemp());

//        isPlayingTempMusic = true;
//        tempMusicClip = tempClip;
//    }

//    // 停止临时音乐，恢复原背景音乐
//    public void StopTemporaryMusic()
//    {
//        if (!isPlayingTempMusic) return;

//        StartCoroutine(FadeOutTemp());
//    }

//    // 手动恢复原背景音乐
//    private void RestoreOriginalMusic()
//    {
//        Debug.Log("临时音乐播放完毕，恢复原背景音乐");

//        isPlayingTempMusic = false;

//        // 如果有被暂停的音乐，恢复播放
//        if (pausedMusicClip != null)
//        {
//            audioSource.clip = pausedMusicClip;
//            audioSource.time = pausedMusicTime;
//            audioSource.volume = 0f;
//            audioSource.Play();
//            StartCoroutine(FadeIn());

//            pausedMusicClip = null;
//            pausedMusicTime = 0f;
//        }
//        else
//        {
//            // 如果没有被暂停的音乐，恢复当前场景应该播放的音乐
//            PlayMusicForCurrentScene();
//        }
//    }

//    // 临时音乐渐入
//    IEnumerator FadeInTemp()
//    {
//        float targetVolume = previousVolume;
//        tempAudioSource.volume = 0;

//        while (tempAudioSource.volume < targetVolume)
//        {
//            tempAudioSource.volume += targetVolume * Time.deltaTime / (fadeDuration * 0.5f); // 临时音乐渐入更快
//            yield return null;
//        }

//        tempAudioSource.volume = targetVolume;
//    }

//    // 临时音乐渐出
//    IEnumerator FadeOutTemp()
//    {
//        float startVolume = tempAudioSource.volume;

//        while (tempAudioSource.volume > 0)
//        {
//            tempAudioSource.volume -= startVolume * Time.deltaTime / (fadeDuration * 0.5f); // 临时音乐渐出更快
//            yield return null;
//        }

//        tempAudioSource.Stop();

//        // 恢复原背景音乐
//        RestoreOriginalMusic();
//    }

//    // 公共方法供其他脚本控制音乐
//    public void MuteMusic(bool mute)
//    {
//        if (mute != isMuted)
//        {
//            isMuted = mute;
//            if (mute)
//            {
//                previousVolume = audioSource.volume;
//                StartCoroutine(FadeOut());

//                // 也静音临时音乐
//                if (isPlayingTempMusic && tempAudioSource != null)
//                {
//                    StartCoroutine(FadeOutTemp());
//                }
//            }
//            else
//            {
//                StartCoroutine(FadeIn());
//            }
//        }
//    }

//    public void SetVolume(float volume)
//    {
//        volume = Mathf.Clamp01(volume);
//        previousVolume = volume;
//        if (!isMuted)
//        {
//            audioSource.volume = volume;

//            // 同时设置临时音乐的音量
//            if (tempAudioSource != null)
//            {
//                tempAudioSource.volume = volume;
//            }
//        }
//    }

//    public void PlayMusic(AudioClip clip)
//    {
//        if (clip != null)
//        {
//            // 停止临时音乐
//            if (isPlayingTempMusic && tempAudioSource.isPlaying)
//            {
//                StopTemporaryMusic();
//            }

//            StartCoroutine(TransitionToMusic(clip));
//        }
//    }

//    public void StopMusic()
//    {
//        StartCoroutine(FadeOutAndStop());
//    }

//    public bool IsPlaying()
//    {
//        return audioSource.isPlaying;
//    }

//    public bool IsPlayingTemporaryMusic()
//    {
//        return isPlayingTempMusic && tempAudioSource != null && tempAudioSource.isPlaying;
//    }

//    public AudioClip GetCurrentClip()
//    {
//        return currentPlayingClip;
//    }

//    public AudioClip GetTemporaryClip()
//    {
//        return tempMusicClip;
//    }

//    void OnDestroy()
//    {
//        SceneManager.sceneLoaded -= OnSceneLoaded;
//    }
//}