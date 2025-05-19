using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TeamMembersUI : MonoBehaviour
{
    [System.Serializable]
    public class TeamMember
    {
        public string name;
        public string role;
        public string description;
        public Sprite avatar;
    }

    [Header("团队成员数据")]
    public TeamMember[] teamMembers;

    [Header("UI组件")]
    public Transform membersContainer;  // 成员项的父对象
    public GameObject memberItemPrefab; // 成员项预制体

    [Header("成员信息详情")]
    public Image selectedAvatar;
    public TextMeshProUGUI selectedName;
    public TextMeshProUGUI selectedRole;
    public TextMeshProUGUI selectedDescription;

    private void Start()
    {
        // 生成团队成员列表
        GenerateTeamMembersList();
        
        // 如果有成员，默认显示第一个
        if (teamMembers != null && teamMembers.Length > 0)
        {
            ShowMemberDetails(0);
        }
    }

    private void GenerateTeamMembersList()
    {
        if (membersContainer == null || memberItemPrefab == null || teamMembers == null)
            return;

        // 清除容器中的现有子项
        foreach (Transform child in membersContainer)
        {
            Destroy(child.gameObject);
        }

        // 为每个团队成员创建一个条目
        for (int i = 0; i < teamMembers.Length; i++)
        {
            TeamMember member = teamMembers[i];
            GameObject item = Instantiate(memberItemPrefab, membersContainer);
            
            // 设置名称
            TextMeshProUGUI nameText = item.GetComponentInChildren<TextMeshProUGUI>();
            if (nameText != null)
                nameText.text = member.name;
            
            // 设置头像
            Image avatar = item.GetComponentInChildren<Image>();
            if (avatar != null && member.avatar != null)
                avatar.sprite = member.avatar;
            
            // 添加点击事件
            Button button = item.GetComponent<Button>();
            if (button != null)
            {
                int index = i; // 捕获当前索引
                button.onClick.AddListener(() => ShowMemberDetails(index));
            }
            
            // 如果有全局UI效果管理器，应用按钮效果
            if (GlobalUIManager.Instance != null)
            {
                GlobalUIManager.Instance.SetupButtonEffects(button);
            }
        }
    }

    public void ShowMemberDetails(int index)
    {
        if (teamMembers == null || index < 0 || index >= teamMembers.Length)
            return;

        TeamMember member = teamMembers[index];

        // 更新详细信息UI
        if (selectedAvatar != null && member.avatar != null)
            selectedAvatar.sprite = member.avatar;
            
        if (selectedName != null)
            selectedName.text = member.name;
            
        if (selectedRole != null)
            selectedRole.text = member.role;
            
        if (selectedDescription != null)
            selectedDescription.text = member.description;
    }
} 