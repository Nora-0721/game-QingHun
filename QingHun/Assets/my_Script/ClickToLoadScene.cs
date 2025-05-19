using UnityEngine;
using UnityEngine.SceneManagement;

public class ClickToLoadScene : MonoBehaviour
{
    [Tooltip("要加载的场景名称")]
    public string sceneToLoad;
    
    [Tooltip("需要点击几次才能跳转场景")]
    public int clicksRequired = 1;
    
    [Tooltip("是否只在特定对象上点击才生效")]
    public bool requireObjectClick = false;

    private int clickCount = 0;

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // 检测鼠标左键点击
        {
            if (requireObjectClick)
            {
                // 发射射线检测点击的对象
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                
                if (Physics.Raycast(ray, out hit))
                {
                    // 如果点击到的是当前游戏对象
                    if (hit.transform == this.transform)
                    {
                        ProcessClick();
                    }
                }
            }
            else
            {
                // 任意点击都计数
                ProcessClick();
            }
        }
    }
    
    private void ProcessClick()
    {
        clickCount++;
        
        if (clickCount >= clicksRequired)
        {
            LoadTargetScene();
        }
    }
    
    public void LoadTargetScene()
    {
        if (string.IsNullOrEmpty(sceneToLoad))
        {
            Debug.LogWarning("没有设置目标场景名称！请在Inspector中设置sceneToLoad。");
            return;
        }
        
        // 检查场景是否存在于构建设置中
        if (!Application.CanStreamedLevelBeLoaded(sceneToLoad))
        {
            Debug.LogError($"场景 {sceneToLoad} 不存在或未添加到Build Settings中！");
            return;
        }
        
        // 直接加载场景，不使用过渡效果
        SceneManager.LoadScene(sceneToLoad);
    }
} 