using UnityEngine;
using UnityEngine.UI;

public class SimpleTeamPanel2 : MonoBehaviour
{
    [Header("团队成员面板")]
    public GameObject teamPanel;  // 团队成员信息面板
    public Button closeButton;    // 关闭按钮

    private void Start()
    {
        // 初始隐藏团队成员面板
        if (teamPanel != null)
            teamPanel.SetActive(false);

        // 设置关闭按钮事件
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(CloseTeamPanel);

            // 添加按钮效果组件
            if (!closeButton.GetComponent<SimpleButtonEffect>())
            {
                closeButton.gameObject.AddComponent<SimpleButtonEffect>();
            }
        }
    }

    // 打开团队成员面板
    public void OpenTeamPanel()
    {
        if (teamPanel != null)
            teamPanel.SetActive(true);
    }

    // 关闭团队成员面板
    public void CloseTeamPanel()
    {
        if (teamPanel != null)
            teamPanel.SetActive(false);
    }
}