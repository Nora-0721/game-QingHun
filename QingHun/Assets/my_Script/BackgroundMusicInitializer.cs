//using UnityEngine;

///// <summary>
///// 将此脚本放在每个需要背景音乐的场景中
///// 它会检查并确保背景音乐管理器存在
///// </summary>
//public class BackgroundMusicInitializer : MonoBehaviour
//{
//    // 预设的音乐管理器，可从项目中拖拽
//    public BackGroundMusic musicManagerPrefab;

//    // 在不使用预设时，可以直接指定场景使用的音乐
//    public AudioClip sceneMusic;

//    // 是否强制播放指定的音乐，无论场景配置如何
//    public bool forcePlaySpecificMusic = false;

//    private void Awake()
//    {
//        // 确保有背景音乐管理器
//        BackGroundMusic musicManager = EnsureMusicManager();

//        // 如果设置了强制播放特定音乐且提供了音乐剪辑
//        if (forcePlaySpecificMusic && sceneMusic != null)
//        {
//            // 等待一帧确保一切初始化完成
//            StartCoroutine(PlayMusicAfterDelay());
//        }
//    }

//    private System.Collections.IEnumerator PlayMusicAfterDelay()
//    {
//        yield return null; // 等待一帧

//        if (BackGroundMusic.Instance != null && sceneMusic != null)
//        {
//            Debug.Log("为场景强制播放指定音乐: " + sceneMusic.name);
//            BackGroundMusic.Instance.PlayMusic(sceneMusic);
//        }
//    }

//    private BackGroundMusic EnsureMusicManager()
//    {
//        // 检查是否已存在实例
//        if (BackGroundMusic.Instance != null)
//        {
//            return BackGroundMusic.Instance;
//        }

//        // 如果没有实例，且有预设，则实例化预设
//        if (musicManagerPrefab != null)
//        {
//            Debug.Log("从预设实例化背景音乐管理器");
//            return Instantiate(musicManagerPrefab);
//        }

//        // 如果没有预设，则创建新的管理器
//        Debug.Log("创建新的背景音乐管理器");
//        return BackGroundMusic.CreateMusicManagerIfNeeded();
//    }
//}