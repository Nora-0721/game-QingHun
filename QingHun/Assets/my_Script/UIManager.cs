//using UnityEngine;
//using UnityEngine.UI;

//public class UIManager : MonoBehaviour
//{
//    // 单例实例
//    public static UIManager Instance { get; private set; }

//    [Header("UI References")]
//    public GameObject panel;  // 控制显示/隐藏的Panel
//    public Button button1;    // 第一个按钮
//    public Button button2;    // 第二个按钮

//    void Awake()
//    {
//        // 单例初始化
//        if (Instance == null)
//        {
//            Instance = this;
//            // DontDestroyOnLoad(gameObject); // 如果需要跨场景保留则取消注释
//        }
//        else
//        {
//            Destroy(gameObject);
//        }
//    }

//    void Start()
//    {
//        // 初始隐藏Panel
//        panel.SetActive(false);

//        // 绑定按钮事件
//        RebindUI();
//    }

//    // 重新绑定UI事件（场景跳转后调用）
//    public void RebindUI()
//    {
//        // 清除旧事件
//        button1.onClick.RemoveAllListeners();
//        button2.onClick.RemoveAllListeners();

//        // 绑定新事件
//        button1.onClick.AddListener(OnButton1Click);
//        button2.onClick.AddListener(OnButton2Click);
//    }

//    // === 按钮事件处理 ===
//    private void OnButton1Click()
//    {
//        Debug.Log("按钮1被点击");
//        panel.SetActive(true);  // 显示Panel
//    }

//    private void OnButton2Click()
//    {
//        Debug.Log("按钮2被点击");
//        panel.SetActive(false); // 隐藏Panel
//    }

//    // 外部控制Panel显示/隐藏
//    public void TogglePanel(bool show)
//    {
//        panel.SetActive(show);
//    }
//}