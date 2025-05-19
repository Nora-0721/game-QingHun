using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class CardDisplay5 : MonoBehaviour, IPointerClickHandler
{
    public Image cardImage;
    public TMP_Text numberText;
    public Image suitImage;
    // ������Jokerͼ�����
    [Header("Joker Icons")]
    public Image jokerTopLeftIcon;        // ���Ͻ�ͼ�꣨С��/����ͨ�ã�
    public Image jokerBottomRightIcon;    // ���½�ͼ�꣨С��/����ͨ�ã�

    // ������ͼ����Դ�ֶ�
    [Header("Joker Icon Sprites")]
    public Sprite smallJokerTopLeft;      // С������ͼ��
    public Sprite smallJokerBottomRight; // С������ͼ�꣨����ת��
    public Sprite bigJokerTopLeft;        // ��������ͼ��
    public Sprite bigJokerBottomRight;   // ��������ͼ�꣨����ת��

    // ���Ͻ����
    [Header("Top-Left Components")]
    public TMP_Text topLeftNumber;
    public Image topLeftSuit;

    // ���½�������Զ���ת��
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
    [SerializeField] private Sprite[] numberFaces;  // ��˳��洢3-2�Ŀ���
    [SerializeField] private Sprite jokerFace;      // ͨ��joker����
    [SerializeField] private Sprite bigJokerFace;   // ����ר�ÿ���

    [Header("Special Card Settings")]
    [SerializeField] private Sprite[] specialFaces; // J/Q/K/A�Ŀ���
    public void Initialize(Card card)
    {
        // Ĭ����������Jokerͼ��
        jokerTopLeftIcon.gameObject.SetActive(false);
        jokerBottomRightIcon.gameObject.SetActive(false);

        this.card = card;

        // Joker���⴦��
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
        // ��ʾ��ͨ�������ֺͻ�ɫ
        numberText.gameObject.SetActive(true);
        suitImage.gameObject.SetActive(true);
        topLeftNumber.gameObject.SetActive(true);
        topLeftSuit.gameObject.SetActive(true);
        bottomRightNumber.gameObject.SetActive(true);
        bottomRightSuit.gameObject.SetActive(true);

        // ������������
        numberText.text = GetNumberText(card.number);
        numberText.color = GetSuitColor(card.type);
        suitImage.sprite = GetSuitSprite(card.type);

        // ��ʼ�����ϽǺ����½�
        InitializeCorner(topLeftNumber, topLeftSuit, card);
        InitializeCorner(bottomRightNumber, bottomRightSuit, card);
    }


    private void InitializeCorner(TMP_Text numberText, Image suitImage, Card card)
    {
        // �������ֺͻ�ɫ
        numberText.text = GetNumberText(card.number);
        numberText.color = GetSuitColor(card.type);
        suitImage.sprite = GetSuitSprite(card.type);
    }

    private void RotateBottomRight()
    {
        // ��ת���
        bottomRightNumber.transform.localRotation = Quaternion.Euler(0, 0, 180);
        bottomRightSuit.transform.localRotation = Quaternion.Euler(0, 0, 180);

        // �����ı����뷽ʽ
        bottomRightNumber.alignment = TextAlignmentOptions.Bottom;
        bottomRightNumber.rectTransform.pivot = new Vector2(1, 0);
    }

    public void ToggleSelection()
    {
        isSelected = !isSelected;
        Vector3 offset = new Vector3(0, isSelected ? 30 : -30, 0);

        // ͬ���ƶ�����λ�õĸ�����
        transform.localPosition += offset;

        // ֪ͨGameManager
        GameManager5.Instance?.SelectCard(card, gameObject, isSelected);
    }

    private Sprite GetCardFace(Card card)
    {
        // �����ƣ�3-10��
        if (card.number >= 3 && card.number <= 10)
        {
            int index = card.number - 3;

            // ��ȫ��������
            if (index < 0 || index >= numberFaces.Length)
            {
                Debug.LogError($"�������ô�������numberFaces���鳤��");
                return null;
            }
            return numberFaces[index];
        }

        // ��ĸ��
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
            case 14: // С��
                return jokerFace;
            case 15: // ����
                return bigJokerFace;
            case 2:  // ����2
                return numberFaces[8]; // ȷ�������е�9��Ԫ��
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
        // �����������ֺͻ�ɫ
        numberText.gameObject.SetActive(false);
        suitImage.gameObject.SetActive(false);
        topLeftNumber.gameObject.SetActive(false);
        topLeftSuit.gameObject.SetActive(false);
        bottomRightNumber.gameObject.SetActive(false);
        bottomRightSuit.gameObject.SetActive(false);

        // ������ֵ��ʾ��Ӧͼ��
        if (card.number == 14) // С��
        {
            jokerTopLeftIcon.sprite = smallJokerTopLeft;
            jokerBottomRightIcon.sprite = smallJokerBottomRight;
        }
        else if (card.number == 15) // ����
        {
            jokerTopLeftIcon.sprite = bigJokerTopLeft;
            jokerBottomRightIcon.sprite = bigJokerBottomRight;
        }

        // ��ʾͼ��
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

            // ֱ��֪ͨGameManager����ѡ��״̬
            if (isSelected)
            {
                GameManager5.Instance?.AddSelectedCard(card, gameObject);
            }
            else
            {
                GameManager5.Instance?.RemoveSelectedCard(card, gameObject);
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