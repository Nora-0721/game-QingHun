using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Text.RegularExpressions;
using static DiaLogmanager;

public class DiaLogmanager : MonoBehaviour
{
    public static DiaLogmanager Instance; // 静态引用
    [Header("内容")]
    public TextAsset dialogDataFile;
    public SpriteRenderer sprite;
    public TMP_Text nameText;
    public TMP_Text dialogText;
    [Header("多个精灵")]
    public List<Sprite> sprites = new List<Sprite>();
    Dictionary<string, Sprite> imageDic = new Dictionary<string, Sprite>();
    [Header("索引等等")]
    public int dialogIndex;
    public string[] dialogRows;
    [Header("按钮")]
    public Button next;
    public GameObject optionButton;
    public Transform buttonGroup;
    [Header("打字机效果设置")]
    [Range(0.01f, 0.2f)]
    public float typingSpeed = 0.05f; // 通过Inspector面板的滑动条调节
    private Coroutine typingCoroutine;
    public bool isTyping = false;

    [Header("背景音乐设置")]
    public AudioClip[] musicClips; // 在Inspector中设置可用的音乐片段
    private Dictionary<string, AudioClip> musicDictionary = new Dictionary<string, AudioClip>();
    private AudioSource musicAudioSource; // 背景音乐播放器

    // 音乐标记的正则表达式模式
    private readonly string musicTagPattern = @"\[music:([^\]]+)\]";

    public SceneSwitcher sceneSwitcher;

    private void Awake()
    {
        Instance = this; // 在 Awake 中设置静态引用
        imageDic["姆教"] = sprites[0];
        imageDic["小宛"] = sprites[1];
        imageDic["青蛇"] = sprites[2];
        imageDic["？？"] = sprites[2];
        imageDic["白蛇"] = sprites[3];
        imageDic["法海"] = sprites[4];
        imageDic["许仙"] = sprites[5];

        // 初始化音乐字典和播放器
        SetupMusicSystem();
    }

    void SetupMusicSystem()
    {
        // 创建音乐播放器
        GameObject musicPlayerObj = new GameObject("MusicPlayer");
        musicPlayerObj.transform.SetParent(transform);
        musicAudioSource = musicPlayerObj.AddComponent<AudioSource>();
        musicAudioSource.loop = true;
        musicAudioSource.playOnAwake = false;

        // 初始化音乐字典
        if (musicClips != null)
        {
            foreach (AudioClip clip in musicClips)
            {
                if (clip != null)
                {
                    musicDictionary[clip.name] = clip;
                    Debug.Log($"添加音乐: {clip.name}");
                }
            }
        }
    }

    void Start()
    {
        ReadText(dialogDataFile);
        ShowDiaLogRow();
        sceneSwitcher = new SceneSwitcher();

        // 如果场景过渡管理器不存在，则创建一个
        if (SceneTransitionManager.Instance == null)
        {
            GameObject transitionManagerObj = new GameObject("SceneTransitionManager");
            transitionManagerObj.AddComponent<SceneTransitionManager>();
        }
    }

    void Update()
    {
    }

    IEnumerator TypeText(string text)
    {
        isTyping = true;

        // 检查并处理文本中的音乐标记
        string originalText = text;
        string displayText = ProcessMusicTags(text);

        dialogText.text = "";
        int totalChars = displayText.Length;
        int currentChar = 0;

        while (currentChar < totalChars)
        {
            // 支持富文本标签（如颜色、大小等）
            if (displayText[currentChar] == '<')
            {
                int tagEnd = displayText.IndexOf('>', currentChar);
                if (tagEnd > 0)
                {
                    dialogText.text = displayText.Substring(0, tagEnd + 1);
                    currentChar = tagEnd + 1;
                    continue;
                }
            }

            dialogText.text = displayText.Substring(0, currentChar + 1);
            currentChar++;

            // 根据字符类型调整速度
            float delay = typingSpeed;
            if (currentChar < totalChars)
            {
                char nextChar = displayText[currentChar];
                if (char.IsPunctuation(nextChar)) delay *= 3f;
                else if (nextChar == '\n') delay = 0f;
            }

            yield return new WaitForSeconds(delay);
        }

        isTyping = false;
    }

    // 处理文本中的音乐标记，返回不含标记的文本
    private string ProcessMusicTags(string text)
    {
        if (string.IsNullOrEmpty(text)) return text;

        // 查找所有音乐标记
        MatchCollection matches = Regex.Matches(text, musicTagPattern);

        foreach (Match match in matches)
        {
            if (match.Success && match.Groups.Count >= 2)
            {
                string fullTag = match.Groups[0].Value; // 完整标记，如[music:战斗音乐]
                string musicName = match.Groups[1].Value.Trim(); // 提取的音乐名称，如"战斗音乐"

                // 处理音乐切换
                PlayMusicByName(musicName);

                // 从文本中移除该标记
                text = text.Replace(fullTag, "");
            }
        }

        return text;
    }

    public void UpdateText(string _name, string _text)
    {
        nameText.text = _name;

        // 停止正在进行的打字效果
        if (isTyping && typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        // 开始新的打字效果
        typingCoroutine = StartCoroutine(TypeText(_text));
    }

    public void UpdateText(string _name)
    {
        nameText.text = _name;
        dialogText.text = "";
    }

    public void UpdateImage(string _name, string _position)
    {
        sprite.sprite = imageDic[_name];
    }

    public void ReadText(TextAsset _textAsset)
    {
        dialogRows = _textAsset.text.Split('\n'); // 以换行来分割
        Debug.Log("读取成果");
    }

    public void ShowDiaLogRow()
    {
        for (int i = 0; i < dialogRows.Length; i++)
        {
            string[] cells = dialogRows[i].Split(',');
            if (cells.Length < 5) continue;

            if (cells[0] == "#" && int.Parse(cells[1]) == dialogIndex)
            {
                // 检查是否为场景跳转命令
                if (cells[5].Trim().StartsWith("end"))
                {
                    string[] endParams = cells[5].Split(':');
                    if (endParams.Length >= 2)
                    {
                        string targetScene = endParams[1].Trim();
                        Debug.Log("跳转到场景：" + targetScene);

                        // 停止当前音乐
                        StopMusic();

                        // 检查是否需要过渡动画（通过添加!前缀表示不要过渡）
                        bool useTransition = !targetScene.StartsWith("!");

                        // 如果指定了不要过渡，则移除!标记
                        if (!useTransition)
                        {
                            targetScene = targetScene.Substring(1);
                        }

                        // 检查场景是否存在
                        if (!Application.CanStreamedLevelBeLoaded(targetScene))
                        {
                            Debug.LogError($"场景{targetScene}未添加到Build Settings！");
                            return;
                        }

                        // 根据条件选择跳转方式
                        if (useTransition && SceneTransitionManager.Instance != null)
                        {
                            SceneTransitionManager.Instance.LoadSceneWithTransition(targetScene);
                        }
                        else
                        {
                            SceneManager.LoadScene(targetScene);
                        }
                        return;
                    }
                }

                if (cells[2] == "无")
                {
                    // 确保对话文本组件是空的，避免文本显示异常
                    dialogText.text = "";
                    // 隐藏下一步按钮
                    next.gameObject.SetActive(false);

                    // 检查是否是最后一个"无"行
                    bool isLastEmptyLine = !CheckIfNextIsAlsoEmpty(i);

                    // 显示黑屏
                    if (BlackScreenManager.Instance != null)
                    {
                        // 处理文本中可能存在的音乐标记
                        string processedText = ProcessMusicTags(cells[4]);
                        BlackScreenManager.Instance.ShowBlackScreen(processedText, isLastEmptyLine);
                    }
                    else
                    {
                        Debug.LogError("BlackScreenManager实例不存在！");
                    }

                    // 更新索引
                    dialogIndex = int.Parse(cells[5]);
                    return;
                }
                else
                {
                    // 正常对话处理
                    UpdateText(cells[2], cells[4]);
                    UpdateImage(cells[2], cells[3]);
                    dialogIndex = int.Parse(cells[5]);
                    next.gameObject.SetActive(true);
                    break;
                }
            }
            else if (cells[0] == "&" && int.Parse(cells[1]) == dialogIndex)
            {
                next.gameObject.SetActive(false); // 隐藏原来的按钮
                UpdateImage(cells[2], cells[3]);
                UpdateText(cells[2]);
                GenerateOption(i);
                break;
            }
        }
    }

    public void OnClickNext()
    {
        // 如果正在打字，立即完成
        if (isTyping && typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            // 显示完整文本
            string[] cells = dialogRows[dialogIndex].Split(',');
            if (cells.Length >= 5)
            {
                // 处理可能存在的音乐标记
                string processedText = ProcessMusicTags(cells[4]);
                dialogText.text = processedText;
            }
            isTyping = false;
            return;
        }

        ShowDiaLogRow();
    }

    public void GenerateOption(int _index) // 生成按钮
    {
        if (_index >= dialogRows.Length) return; // 检查索引是否超出范围

        string[] cells = dialogRows[_index].Split(',');

        if (cells[0] == "&")
        {
            GameObject button = Instantiate(optionButton, buttonGroup);
            button.GetComponentInChildren<TMP_Text>().text = cells[4];
            button.GetComponent<Button>().onClick.AddListener(delegate
            {
                OnOptionClick(int.Parse(cells[5]));
            });
            GenerateOption(_index + 1);
        }
    }

    public void OnOptionClick(int _id)
    {
        dialogIndex = _id;
        for (int i = 0; i < buttonGroup.childCount; i++)
        {
            Destroy(buttonGroup.GetChild(i).gameObject);
        }
        ShowDiaLogRow();
    }

    public void ContinueDialog()
    {
        next.gameObject.SetActive(true);
        ShowDiaLogRow();
    }

    private bool CheckIfNextIsAlsoEmpty(int currentIndex)
    {
        if (currentIndex + 1 >= dialogRows.Length) return false;

        string[] nextCells = dialogRows[currentIndex + 1].Split(',');
        if (nextCells.Length < 5) return false;

        // 检查是否是连续的黑屏行
        return nextCells[0] == "#" &&
               int.Parse(nextCells[1]) == int.Parse(dialogRows[currentIndex].Split(',')[5]) && // 使用当前行指定的下一个索引
               nextCells[2] == "无";
    }

    public void ContinueToNextBlackScreen()
    {
        // 确保对话文本组件是空的
        dialogText.text = "";
        // 直接显示下一行而不淡出黑屏
        ShowDiaLogRow();
    }

    // 播放指定名称的音乐
    public void PlayMusicByName(string musicName)
    {
        if (string.IsNullOrEmpty(musicName)) return;

        Debug.Log($"播放背景音乐: {musicName}");

        // 特殊指令处理
        if (musicName.ToLower() == "stop" || musicName.ToLower() == "none")
        {
            StopMusic();
            return;
        }

        // 尝试在字典中查找音乐
        if (musicDictionary.TryGetValue(musicName, out AudioClip clip))
        {
            // 淡出当前音乐并播放新音乐
            StartCoroutine(FadeAndPlayMusic(clip));
        }
        else
        {
            Debug.LogWarning($"未找到背景音乐: {musicName}");
        }
    }

    // 淡入淡出切换音乐
    private IEnumerator FadeAndPlayMusic(AudioClip newClip)
    {
        float fadeDuration = 1.0f;
        float targetVolume = 0.7f; // 默认音量

        // 如果当前有音乐在播放，先淡出
        if (musicAudioSource.isPlaying)
        {
            float startVolume = musicAudioSource.volume;
            float timer = 0;

            while (timer < fadeDuration)
            {
                timer += Time.deltaTime;
                musicAudioSource.volume = Mathf.Lerp(startVolume, 0, timer / fadeDuration);
                yield return null;
            }

            musicAudioSource.Stop();
        }

        // 播放新音乐并淡入
        musicAudioSource.clip = newClip;
        musicAudioSource.volume = 0;
        musicAudioSource.Play();

        float timer2 = 0;
        while (timer2 < fadeDuration)
        {
            timer2 += Time.deltaTime;
            musicAudioSource.volume = Mathf.Lerp(0, targetVolume, timer2 / fadeDuration);
            yield return null;
        }

        musicAudioSource.volume = targetVolume;
    }

    // 停止当前音乐
    public void StopMusic()
    {
        if (musicAudioSource != null && musicAudioSource.isPlaying)
        {
            StartCoroutine(FadeOutMusic());
        }
    }

    // 淡出并停止音乐
    private IEnumerator FadeOutMusic()
    {
        float fadeDuration = 1.0f;
        float startVolume = musicAudioSource.volume;
        float timer = 0;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            musicAudioSource.volume = Mathf.Lerp(startVolume, 0, timer / fadeDuration);
            yield return null;
        }

        musicAudioSource.Stop();
        musicAudioSource.volume = 0;
    }

    // 在销毁时停止音乐
    private void OnDestroy()
    {
        StopMusic();
    }

    public class SceneSwitcher : MonoBehaviour
    {
        public void LoadScene(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }
    }
}