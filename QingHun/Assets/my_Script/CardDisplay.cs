using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class CardDisplay : MonoBehaviour, IPointerClickHandler
{
    public Image cardImage;
    public TMP_Text numberText;
    public Image suitImage;
    // 新增的Joker图标组件
    [Header("Joker Icons")]
    public Image jokerTopLeftIcon;        // 左上角图标（小王/大王通用）
    public Image jokerBottomRightIcon;    // 右下角图标（小王/大王通用）

    // 新增的图标资源字段
    [Header("Joker Icon Sprites")]
    public Sprite smallJokerTopLeft;      // 小王左上图标
    public Sprite smallJokerBottomRight; // 小王右下图标（已旋转）
    public Sprite bigJokerTopLeft;        // 大王左上图标
    public Sprite bigJokerBottomRight;   // 大王右下图标（已旋转）

    // 左上角组件
    [Header("Top-Left Components")]
    public TMP_Text topLeftNumber;
    public Image topLeftSuit;

    // 右下角组件（自动翻转）
    [Header("Bottom-Right Components")]
    public TMP_Text bottomRightNumber;
    public Image bottomRightSuit;

    [Header("Card Colors")]
    public Color spadeColor = Color.black;
    public Color heartColor = Color.red;
    public Color diamondColor = Color.red;
    public Color clubColor = Color.black;
    public Color jokerColor = Color.red;

    [Header("Card Sprites")]
    public Sprite spadeSprite;
    public Sprite heartSprite;
    public Sprite diamondSprite;
    public Sprite clubSprite;
    public Sprite jokerSprite;

    private Card card;
    private bool isSelected = false;

    [Header("Card Face Sprites")]
    [SerializeField] private Sprite[] numberFaces;  // 按顺序存储3-2的卡面
    [SerializeField] private Sprite jokerFace;      // 通用joker卡面
    [SerializeField] private Sprite bigJokerFace;   // 大王专用卡面

    [Header("Special Card Settings")]
    [SerializeField] private Sprite[] specialFaces; // J/Q/K/A的卡面
    public void Initialize(Card card)
    {
        // 默认隐藏所有Joker图标
        jokerTopLeftIcon.gameObject.SetActive(false);
        jokerBottomRightIcon.gameObject.SetActive(false);

        this.card = card;

        // Joker特殊处理
        if (card.type == "Joker")
        {
            HandleJokerSpecial(card);
        }
        else
        {
            InitializeNormalCard(card);
        }

        RotateBottomRight();
        cardImage.sprite = GetCardFace(card);
    }
    private void InitializeNormalCard(Card card)
    {
        // 显示普通卡牌文字和花色
        numberText.gameObject.SetActive(true);
        suitImage.gameObject.SetActive(true);
        topLeftNumber.gameObject.SetActive(true);
        topLeftSuit.gameObject.SetActive(true);
        bottomRightNumber.gameObject.SetActive(true);
        bottomRightSuit.gameObject.SetActive(true);

        // 设置文字内容
        numberText.text = GetNumberText(card.number);
        numberText.color = GetSuitColor(card.type);
        suitImage.sprite = GetSuitSprite(card.type);

        // 初始化左上角和右下角
        InitializeCorner(topLeftNumber, topLeftSuit, card);
        InitializeCorner(bottomRightNumber, bottomRightSuit, card);
    }


    private void InitializeCorner(TMP_Text numberText, Image suitImage, Card card)
    {
        // 设置数字和花色
        numberText.text = GetNumberText(card.number);
        numberText.color = GetSuitColor(card.type);
        suitImage.sprite = GetSuitSprite(card.type);
    }

    private void RotateBottomRight()
    {
        // 旋转组件
        bottomRightNumber.transform.localRotation = Quaternion.Euler(0, 0, 180);
        bottomRightSuit.transform.localRotation = Quaternion.Euler(0, 0, 180);

        // 调整文本对齐方式
        bottomRightNumber.alignment = TextAlignmentOptions.Bottom;
        bottomRightNumber.rectTransform.pivot = new Vector2(1, 0);
    }

    public void ToggleSelection()
    {
        isSelected = !isSelected;
        Vector3 offset = new Vector3(0, isSelected ? 30 : -30, 0);

        // 同步移动两个位置的父对象
        transform.localPosition += offset;

        // 通知GameManager
        GameManager.Instance?.SelectCard(card, gameObject, isSelected);
    }

    private Sprite GetCardFace(Card card)
    {
        // 数字牌（3-10）
        if (card.number >= 3 && card.number <= 10)
        {
            int index = card.number - 3;

            // 安全访问数组
            if (index < 0 || index >= numberFaces.Length)
            {
                Debug.LogError($"卡面配置错误！请检查numberFaces数组长度");
                return null;
            }
            return numberFaces[index];
        }

        // 字母牌
        switch (card.number)
        {
            case 1:  // A
                return specialFaces[3];
            case 11: // J
                return specialFaces[0];
            case 12: // Q
                return specialFaces[1];
            case 13: // K
                return specialFaces[2];
            case 14: // 小王
                return jokerFace;
            case 15: // 大王
                return bigJokerFace;
            case 2:  // 数字2
                return numberFaces[8]; // 确保数组有第9个元素
            default:
                return numberFaces[0];
        }
    }
    private string GetNumberText(int number)
    {
        return number switch
        {
            1 => "A",
            11 => "J",
            12 => "Q",
            13 => "K",
            _ => number.ToString()
        };
    }

    private void HandleJokerSpecial(Card card)
    {
        // 隐藏所有文字和花色
        numberText.gameObject.SetActive(false);
        suitImage.gameObject.SetActive(false);
        topLeftNumber.gameObject.SetActive(false);
        topLeftSuit.gameObject.SetActive(false);
        bottomRightNumber.gameObject.SetActive(false);
        bottomRightSuit.gameObject.SetActive(false);

        // 根据数值显示对应图标
        if (card.number == 14) // 小王
        {
            jokerTopLeftIcon.sprite = smallJokerTopLeft;
            jokerBottomRightIcon.sprite = smallJokerBottomRight;
        }
        else if (card.number == 15) // 大王
        {
            jokerTopLeftIcon.sprite = bigJokerTopLeft;
            jokerBottomRightIcon.sprite = bigJokerBottomRight;
        }

        // 显示图标
        jokerTopLeftIcon.gameObject.SetActive(true);
        jokerBottomRightIcon.gameObject.SetActive(true);
    }
    private Color GetSuitColor(string suit)
    {
        return suit switch
        {
            "Spade" => spadeColor,
            "Heart" => heartColor,
            "Diamond" => diamondColor,
            "Club" => clubColor,
            "Joker" => jokerColor,
            _ => Color.black
        };
    }

    private Sprite GetSuitSprite(string suit)
    {
        return suit switch
        {
            "Spade" => spadeSprite,
            "Heart" => heartSprite,
            "Diamond" => diamondSprite,
            "Club" => clubSprite,
            "Joker" => jokerSprite,
            _ => null
        };
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            isSelected = !isSelected;
            transform.localPosition += new Vector3(0, isSelected ? 30 : -30, 0);

            // 直接通知GameManager更新选择状态
            if (isSelected)
            {
                GameManager.Instance?.AddSelectedCard(card, gameObject);
            }
            else
            {
                GameManager.Instance?.RemoveSelectedCard(card, gameObject);
            }
        }
    }

    
    public void ResetSelection()
    {
        if (isSelected)
        {
            isSelected = false;
            transform.localPosition -= new Vector3(0, 30, 0);
        }
    }

    public void SetSortingOrder(int order)
    {
        Canvas canvas = GetComponent<Canvas>();
        if (canvas == null)
        {
            canvas = gameObject.AddComponent<Canvas>();
        }
        canvas.overrideSorting = true;
        canvas.sortingOrder = order;
    }

}