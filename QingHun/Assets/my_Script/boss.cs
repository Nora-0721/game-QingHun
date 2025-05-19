using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager2 : MonoBehaviour
{
    public static GameManager2 Instance;

    [Header("UI Elements")]
    public GameObject cardPrefab;
    public Transform playerHandArea;
    public Image discardPileArea;
    public Image deckImage;
    public TMP_Text deckCountText;
    public Button playButton;
    public Button discardButton;
    public Button skipButton;
    public Slider playerHealthBar;
    public Slider bossHealthBar;
    public TMP_Text turnInfoText;

    [Header("Game Settings")]
    public int initialPlayerHealth = 15;
    public int initialBossHealth = 100;
    public int initialBossAttack = 2;
    public int cardsToDrawEachTurn = 5;

    [Header("Animation Settings")]
    public float cardMoveDuration = 0.5f;
    public Transform playAreaCenter;
    public Transform discardPilePosition; // ����Ŀ��λ��
    public float dealingDelay = 0.1f; // ���Ƽ��ʱ��
    public Transform deckPosition; // �ƶ�λ�ã�������ʼλ�ã�

    [Header("Dialogue Settings")]
    public GameObject faHaiDialogueBox;  // �����ĶԻ���
    public GameObject xiaoQingDialogueBox;  // С��ĶԻ���
    public TMP_Text faHaiDialogueText;  // �����ĶԻ��ı�
    public TMP_Text xiaoQingDialogueText;  // С��ĶԻ��ı�
    public float typingSpeed = 0.05f;
    public TextAsset dialogueCSV;
    private int currentDialogueIndex = 0;
    private bool isDialogueShowing = false;
    private List<DialogueData> dialogueList = new List<DialogueData>();

    [Header("Damage Effect Settings")]
    public Image bossImage;        // Boss��ͼƬ
    public Image playerImage;      // ��ҵ�ͼƬ
    public float shakeDuration = 0.5f;  // ��������ʱ��
    public float shakeAmount = 5f;      // ��������
    public float decreaseAmount = 1f;    // �������ȵݼ��ٶ�

    [System.Serializable]
    public class DialogueData
    {
        public string mark;      // ��־
        public string id;        // ID
        public string speaker;   // ˵����
        public string position;  // λ��
        public string content;   // ����
        public string jumpTo;    // ��ת

        public DialogueData(string[] data)
        {
            if (data.Length >= 6)
            {
                mark = data[0];      // A��
                id = data[1];        // B��
                speaker = data[2];    // C��
                position = data[3];   // D��
                content = data[4];    // E��
                jumpTo = data[5];     // F��
            }
        }
    }

    private List<Card> deck = new List<Card>();
    private List<Card> discardPile = new List<Card>();
    private List<Card> playerHand = new List<Card>();
    private List<GameObject> playerHandCards = new List<GameObject>();
    private List<Card> selectedCards = new List<Card>();

    private int playerHealth;
    private int bossHealth;
    private int bossAttack;
    private bool canPlayCards = true;
    private int turnCount = 0;
    private bool gameOver = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        InitializeGame();
        LoadDialogueData();
        StartCoroutine(BossAttackAtStart());

        // ȷ�����ư�ť����ȷ����
        if (discardButton != null)
        {
            discardButton.onClick.RemoveAllListeners(); // ������ܵ��ظ�����
            discardButton.onClick.AddListener(DiscardCards);
            Debug.Log("Discard button listener added");
        }
        else
        {
            Debug.LogError("Discard button is not assigned!");
        }

        // �������λ���Ƿ�����
        if (discardPilePosition == null)
        {
            Debug.LogError("Discard pile position is not set!");
        }
    }

    private void InitializeGame()
    {
        playerHealth = initialPlayerHealth;
        bossHealth = initialBossHealth;
        bossAttack = initialBossAttack;
        turnCount = 0;
        gameOver = false;

        InitializeDeck();
        ShuffleDeck();
        DealInitialCards();
        UpdateUI();

        playButton.gameObject.SetActive(true);
        discardButton.gameObject.SetActive(false);
        skipButton.gameObject.SetActive(false);

        playButton.onClick.AddListener(PlayCards);
        discardButton.onClick.AddListener(DiscardCards);
        skipButton.onClick.AddListener(SkipTurn);
    }

    private void InitializeDeck()
    {
        deck.Clear();
        discardPile.Clear();

        string[] types = { "Spade", "Heart", "Diamond", "Club" };

        // Create first deck
        int id = 0;
        foreach (string type in types)
        {
            for (int num = 1; num <= 13; num++)
            {
                deck.Add(new Card(id++, type, num));
            }
        }
        deck.Add(new Card(id++, "Joker", 14));
        deck.Add(new Card(id++, "Joker", 15));

        // Create second deck
        foreach (string type in types)
        {
            for (int num = 1; num <= 13; num++)
            {
                deck.Add(new Card(id++, type, num));
            }
        }
        deck.Add(new Card(id++, "Joker", 14));
        deck.Add(new Card(id++, "Joker", 15));

        Debug.Log($"Total cards in deck: {deck.Count}");
    }

    private void ShuffleDeck()
    {
        for (int i = 0; i < deck.Count; i++)
        {
            Card temp = deck[i];
            int randomIndex = Random.Range(i, deck.Count);
            deck[i] = deck[randomIndex];
            deck[randomIndex] = temp;
        }
    }

    private void DealInitialCards()
    {
        // ֱ�ӷ��ƣ�����Ҫ����
        for (int i = 0; i < 10; i++)  // Changed from 8 to 10
        {
            DrawCard();
        }
    }

    private void DrawCard()
    {
        if (deck.Count == 0)
        {
            deckImage.gameObject.SetActive(false);
            return;
        }

        Card card = deck[0];
        deck.RemoveAt(0);
        playerHand.Add(card);

        GameObject cardObj = Instantiate(cardPrefab, playerHandArea);
        cardObj.GetComponent<CardDisplay>().Initialize(card);
        playerHandCards.Add(cardObj);

        UpdateHandLayout();
        UpdateDeckCount();
    }

    private void UpdateHandLayout()
    {
        // ���ȶ����ƽ�������
        SortCards();

        float cardSpacing = Mathf.Clamp(300f / playerHandCards.Count, 30f, 60f);
        float totalWidth = (playerHandCards.Count - 1) * cardSpacing;
        float startX = -totalWidth / 2;

        for (int i = 0; i < playerHandCards.Count; i++)
        {
            Vector3 newPosition = new Vector3(startX + i * cardSpacing, 0, 0);
            playerHandCards[i].transform.localPosition = newPosition;
            playerHandCards[i].transform.SetSiblingIndex(i);
        }
    }

    private void SortCards()
    {
        // ������������
        int[] indices = new int[playerHand.Count];
        for (int i = 0; i < indices.Length; i++)
        {
            indices[i] = i;
        }

        // ���ݿ���Ȩֵ��������
        System.Array.Sort(indices, (a, b) => playerHand[a].GetSortValue().CompareTo(playerHand[b].GetSortValue()));

        // ���������������������п���
        List<Card> sortedHand = new List<Card>();
        List<GameObject> sortedHandCards = new List<GameObject>();

        foreach (int index in indices)
        {
            sortedHand.Add(playerHand[index]);
            sortedHandCards.Add(playerHandCards[index]);
        }

        // �����б�
        playerHand = sortedHand;
        playerHandCards = sortedHandCards;
    }

    private void UpdateDeckCount()
    {
        deckCountText.text = deck.Count.ToString();
        deckImage.gameObject.SetActive(deck.Count > 0);
    }

    public void SelectCard(Card card, GameObject cardObj, bool isSelecting)
    {
        if (!canPlayCards || gameOver)
        {
            Debug.Log("��ǰ����ѡ����");
            return;
        }

        if (isSelecting)
        {
            if (!selectedCards.Contains(card))
            {
                selectedCards.Add(card);
            }
        }
        else
        {
            if (selectedCards.Contains(card))
            {
                selectedCards.Remove(card);
            }
        }
    }

    private void PlayCards()
    {
        if (selectedCards.Count == 0 || !canPlayCards || gameOver) return;

        int damage = CalculateDamage(selectedCards);
        UpdateTurnInfo($"������ {GetCardCombinationName(selectedCards)}����� {damage} ���˺���");
        StartCoroutine(MoveCardsToCenterAndDamage(damage));
    }

    // ========== �������ƶ��� ==========
    public void DiscardCards()
    {
        Debug.Log("DiscardCards called");
        if (selectedCards.Count == 0)
        {
            Debug.Log($"Discard cancelled - no cards selected");
            return;
        }

        if (discardPilePosition == null)
        {
            Debug.LogError("discardPilePosition is not set!");
            return;
        }

        discardButton.interactable = false;
        List<Card> cardsToDiscard = new List<Card>(selectedCards);
        Debug.Log($"Attempting to discard {cardsToDiscard.Count} cards");

        foreach (Card card in cardsToDiscard)
        {
            int index = playerHand.IndexOf(card);
            if (index != -1)
            {
                GameObject cardObj = playerHandCards[index];
                Debug.Log($"Starting discard animation for card: {card.type}-{card.number}");
                StartCoroutine(MoveCardToDiscard(cardObj, card));
            }
            else
            {
                Debug.LogError($"Card {card.type}-{card.number} not found in playerHand!");
            }
        }

        selectedCards.Clear();

        // ȷ���غϽ�����״̬��ȷ
        StartCoroutine(WaitAndEndTurn());
    }

    private IEnumerator MoveCardToDiscard(GameObject cardObj, Card card)
    {
        Debug.Log($"MoveCardToDiscard started for card: {card.type}-{card.number}");
        CanvasGroup canvasGroup = cardObj.GetComponent<CanvasGroup>();
        if (canvasGroup != null) canvasGroup.blocksRaycasts = false;

        Vector3 startPos = cardObj.transform.position;
        Vector3 targetPos = discardPilePosition.position;
        Debug.Log($"Moving card from {startPos} to {targetPos}");
        float elapsedTime = 0f;

        while (elapsedTime < cardMoveDuration)
        {
            cardObj.transform.position = Vector3.Lerp(startPos, targetPos, elapsedTime / cardMoveDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        cardObj.transform.position = targetPos;
        Debug.Log("Card reached discard position");

        playerHand.Remove(card);
        playerHandCards.Remove(cardObj);
        discardPile.Add(card);
        Destroy(cardObj);

        UpdateHandLayout();
        discardButton.interactable = true;
        Debug.Log("Discard complete");
    }

    private IEnumerator WaitAndEndTurn()
    {
        // �ȴ����п��ƶ������
        yield return new WaitForSeconds(cardMoveDuration + 0.1f);
        EndTurn();
    }

    private void SkipTurn()
    {
        if (gameOver) return;
        EndTurn();
    }

    private void EndTurn()
    {
        if (gameOver) return;

        ResetAllCardsSelection();

        turnCount++;

        // ����
        int cardsNeeded = Mathf.Max(0, 10 - playerHand.Count);  // Changed from 8 to 10
        int cardsToDraw = Mathf.Min(cardsNeeded, deck.Count);

        for (int i = 0; i < cardsToDraw; i++)
        {
            DrawCard();
        }

        // ���ð�ť״̬
        playButton.gameObject.SetActive(true);
        discardButton.gameObject.SetActive(false);
        skipButton.gameObject.SetActive(false);
        canPlayCards = true;

        StartCoroutine(BossAttackAtStart());
    }

    private void LoadDialogueData()
    {
        if (dialogueCSV == null)
        {
            Debug.LogError("�Ի�CSV�ļ�δ���ã�");
            return;
        }

        dialogueList.Clear();
        string[] lines = dialogueCSV.text.Split('\n');

        // ������һ�У������У�
        for (int i = 1; i < lines.Length; i++)
        {
            string line = lines[i].Trim();
            if (string.IsNullOrEmpty(line)) continue;

            // �ָ�ÿһ�У�����CSV�е����źͶ���
            List<string> fields = new List<string>();
            bool inQuotes = false;
            string field = "";

            for (int j = 0; j < line.Length; j++)
            {
                if (line[j] == '"')
                {
                    inQuotes = !inQuotes;
                    continue;
                }

                if (line[j] == ',' && !inQuotes)
                {
                    fields.Add(field.Trim());
                    field = "";
                    continue;
                }

                field += line[j];
            }
            fields.Add(field.Trim());

            // ����Ƿ�Ϊ��Ч�ĶԻ��У���һ��Ӧ����#��
            if (fields.Count > 0 && fields[0] == "#")
            {
                DialogueData dialogue = new DialogueData(fields.ToArray());
                dialogueList.Add(dialogue);
                Debug.Log($"���ضԻ�: ID={dialogue.id}, ˵����={dialogue.speaker}, ����={dialogue.content}, ��ת={dialogue.jumpTo}");
            }
        }

        Debug.Log($"�ܹ������� {dialogueList.Count} ���Ի�");
    }

    private IEnumerator MoveCardsToCenterAndDamage(int damage)
    {
        Debug.Log($"��ʼ���ƶ�������ǰ�Ի�������{currentDialogueIndex}");
        canPlayCards = false;
        playButton.interactable = false;

        // 1. Collect cards to move and their original world positions
        List<GameObject> cardsToMove = new List<GameObject>();
        List<Card> cardsToRemove = new List<Card>();
        List<int> indicesToRemove = new List<int>();
        List<Vector3> originalWorldPositions = new List<Vector3>();
        List<Vector3> originalScales = new List<Vector3>();

        foreach (Card card in selectedCards)
        {
            int index = playerHand.IndexOf(card);
            if (index != -1)
            {
                GameObject cardObj = playerHandCards[index];
                cardsToMove.Add(cardObj);
                cardsToRemove.Add(card);
                indicesToRemove.Add(index);

                // Store original world position and scale before any changes
                originalWorldPositions.Add(cardObj.transform.position);
                originalScales.Add(cardObj.transform.localScale);
            }
        }

        // 2. Move cards to canvas and prepare for animation
        Canvas mainCanvas = transform.parent.GetComponent<Canvas>();
        foreach (GameObject cardObj in cardsToMove)
        {
            // Store original position in world space
            Vector3 worldPos = cardObj.transform.position;

            // Move to canvas
            cardObj.transform.SetParent(mainCanvas.transform);

            // Restore world position
            cardObj.transform.position = worldPos;

            // Ensure proper render order
            Canvas cardCanvas = cardObj.GetComponent<Canvas>();
            if (cardCanvas != null)
            {
                cardCanvas.overrideSorting = true;
                cardCanvas.sortingOrder = 999;
            }
        }

        // 3. Remove cards from hand data (back to front)
        indicesToRemove.Sort((a, b) => b.CompareTo(a));
        foreach (int index in indicesToRemove)
        {
            playerHand.RemoveAt(index);
            playerHandCards.RemoveAt(index);
        }

        // 4. Update layout of remaining cards
        UpdateHandLayout();

        // 5. Animate each card independently
        float staggerDelay = 0.1f; // Delay between each card's animation
        for (int i = 0; i < cardsToMove.Count; i++)
        {
            StartCoroutine(MoveCardFromPosition(cardsToMove[i], originalWorldPositions[i]));
            yield return new WaitForSeconds(staggerDelay);
        }

        // Wait for all animations to complete
        yield return new WaitForSeconds(cardMoveDuration);

        // 6. Update game state and UI
        bossHealth -= damage;
        UpdateUI();

        // 7. Boss damage effect
        StartCoroutine(ShakeCharacter(bossImage));

        // 8. Clean up animated cards
        foreach (GameObject cardObj in cardsToMove)
        {
            discardPile.Add(cardsToRemove[cardsToMove.IndexOf(cardObj)]);
            Destroy(cardObj);
        }

        // 9. Handle dialogue if needed
        if (currentDialogueIndex < dialogueList.Count)
        {
            if (dialogueList[currentDialogueIndex].speaker == "С��")
            {
                yield return StartCoroutine(ShowDialogue(currentDialogueIndex));

                if (dialogueList[currentDialogueIndex].jumpTo != "end")
                {
                    currentDialogueIndex = int.Parse(dialogueList[currentDialogueIndex].jumpTo);
                }
            }
        }

        selectedCards.Clear();
        UpdateTurnInfo("ѡ�����ƻ�����");

        playButton.gameObject.SetActive(false);
        discardButton.gameObject.SetActive(true);
        skipButton.gameObject.SetActive(true);
        playButton.interactable = true;
    }

    private IEnumerator MoveCardFromPosition(GameObject cardObj, Vector3 startPosition)
    {
        float elapsedTime = 0f;
        Vector3 targetPos = playAreaCenter.position;

        // Ensure card is at the correct starting position and has correct scale
        cardObj.transform.position = startPosition;
        Vector3 originalScale = cardObj.transform.localScale;

        while (elapsedTime < cardMoveDuration)
        {
            float t = elapsedTime / cardMoveDuration;

            // Use smooth step for more natural easing
            float smoothT = t * t * (3f - 2f * t);

            // Direct linear movement
            Vector3 pos = Vector3.Lerp(startPosition, targetPos, smoothT);

            // Scale down slightly as the card moves to the center
            float scaleMultiplier = Mathf.Lerp(1f, 0.8f, smoothT);
            cardObj.transform.localScale = originalScale * scaleMultiplier;

            // Apply position
            cardObj.transform.position = pos;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        cardObj.transform.position = targetPos;
        cardObj.transform.localScale = originalScale * 0.8f;
    }

    private IEnumerator BossAttackAtStart()
    {
        if (gameOver) yield break;

        int bossDamage = bossAttack;
        playerHealth -= bossDamage;
        bossAttack++;
        UpdateUI();

        // ������˶���Ч��
        StartCoroutine(ShakeCharacter(playerImage));

        Debug.Log($"Boss������ʼ����ǰ�Ի�������{currentDialogueIndex}");
        if (currentDialogueIndex < dialogueList.Count)
        {
            Debug.Log($"��ǰ�Ի���ɫ��{dialogueList[currentDialogueIndex].speaker}�����ݣ�{dialogueList[currentDialogueIndex].content}");
            if (dialogueList[currentDialogueIndex].speaker == "����")
            {
                Debug.Log("��⵽�����Ի�����ʼ��ʾ");
                yield return StartCoroutine(ShowDialogue(currentDialogueIndex));
                Debug.Log($"�����Ի���ʾ��ɣ���תֵ��{dialogueList[currentDialogueIndex].jumpTo}");

                if (dialogueList[currentDialogueIndex].jumpTo != "end")
                {
                    currentDialogueIndex = int.Parse(dialogueList[currentDialogueIndex].jumpTo);
                    Debug.Log($"�Ի���������Ϊ��{currentDialogueIndex}");
                }
            }
            else
            {
                Debug.Log($"��ǰ���Ƿ����ĶԻ���������ʾ");
            }
        }

        if (!gameOver)
        {
            UpdateTurnInfo("��Ļغ� - �����");
        }
    }

    private IEnumerator ShowDialogue(int index)
    {
        if (index < 0 || index >= dialogueList.Count)
        {
            Debug.LogError($"�Ի����� {index} ������Χ��");
            yield break;
        }

        DialogueData dialogue = dialogueList[index];
        Debug.Log($"��ʾ�Ի�����ɫ={dialogue.speaker}, ����={dialogue.content}");
        isDialogueShowing = true;

        GameObject currentDialogueBox;
        TMP_Text currentDialogueText;

        if (dialogue.speaker == "����")
        {
            currentDialogueBox = faHaiDialogueBox;
            currentDialogueText = faHaiDialogueText;
            Debug.Log("ʹ�÷����Ի���");
        }
        else
        {
            currentDialogueBox = xiaoQingDialogueBox;
            currentDialogueText = xiaoQingDialogueText;
            Debug.Log("ʹ��С��Ի���");
        }

        faHaiDialogueBox.SetActive(false);
        xiaoQingDialogueBox.SetActive(false);

        currentDialogueBox.SetActive(true);
        currentDialogueText.text = "";

        foreach (char c in dialogue.content)
        {
            currentDialogueText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }

        yield return new WaitForSeconds(1.5f);
        currentDialogueBox.SetActive(false);
        isDialogueShowing = false;
        Debug.Log("�Ի���ʾ���");
    }

    private void ResetAllCardsSelection()
    {
        // �������п��Ƶ��Ӿ�״̬
        foreach (GameObject cardObj in playerHandCards)
        {
            CardDisplay display = cardObj.GetComponent<CardDisplay>();
            if (display != null)
            {
                display.ResetSelection();
            }
        }

        // ���ѡ���б�
        selectedCards.Clear();
    }

    private void GameOver(bool playerWon)
    {
        gameOver = true;

        playButton.gameObject.SetActive(false);
        discardButton.gameObject.SetActive(false);
        skipButton.gameObject.SetActive(false);

        playerHealthBar.value = Mathf.Clamp01((float)playerHealth / initialPlayerHealth);
        bossHealthBar.value = Mathf.Clamp01((float)bossHealth / initialBossHealth);

        string sceneName = playerWon ? "VictoryScene" : "GameOverScene";
        if (SceneTransitionManager1.Instance != null)
        {
            SceneTransitionManager1.Instance.LoadSceneWithTransition(sceneName);
        }
        else
        {
            Debug.LogError("SceneTransitionManager1 instance not found!");
            SceneManager.LoadScene(sceneName);
        }

        if (playerWon)
        {
            UpdateTurnInfo("��ϲ��սʤ��Boss��");
        }
        else
        {
            UpdateTurnInfo("��Ϸ�������㱻Boss�����...");
        }
    }

    private IEnumerator FlashGameOver()
    {
        Image background = GameObject.Find("Background").GetComponent<Image>();
        Color originalColor = background.color;

        for (int i = 0; i < 3; i++)
        {
            background.color = Color.red;
            yield return new WaitForSeconds(0.3f);
            background.color = originalColor;
            yield return new WaitForSeconds(0.3f);
        }
    }

    private void UpdateUI()
    {
        // ����Ѫ����ȷ����Сֵ����
        playerHealthBar.value = Mathf.Clamp01((float)playerHealth / initialPlayerHealth);
        bossHealthBar.value = Mathf.Clamp01((float)bossHealth / initialBossHealth);

        // ����Ѫ����ȫ�յ����
        if (playerHealth <= 0)
        {
            playerHealthBar.fillRect.gameObject.SetActive(false);
            GameOver(false);
        }
        if (bossHealth <= 0)
        {
            bossHealthBar.fillRect.gameObject.SetActive(false);
            GameOver(true);
        }
        UpdateDeckCount();
    }

    public void AddSelectedCard(Card card, GameObject cardObj)
    {
        if (!selectedCards.Contains(card))
        {
            selectedCards.Add(card);
            Debug.Log($"Added card: {card.type}-{card.number}");
        }
    }

    public void RemoveSelectedCard(Card card, GameObject cardObj)
    {
        if (selectedCards.Contains(card))
        {
            selectedCards.Remove(card);
            Debug.Log($"Removed card: {card.type}-{card.number}");
        }
    }

    private void UpdateTurnInfo(string message)
    {
        turnInfoText.text = message;
    }

    private string GetCardCombinationName(List<Card> cards)
    {
        if (IsWangZha(cards)) return "��ը";
        if (IsTongHuaShun(cards)) return "ͬ��˳";
        if (IsShunZi(cards)) return "˳��";
        if (IsSanDaiYi(cards)) return "����һ";
        if (IsDuiZi(cards)) return "����";
        return "����";
    }

    // ��Э�̣��ƶ����ſ��Ƶ�����
    private IEnumerator MoveCardToCenter(GameObject cardObj)
    {
        Vector3 startPos = cardObj.transform.position;
        Vector3 endPos = playAreaCenter.position;
        float elapsedTime = 0f;

        while (elapsedTime < cardMoveDuration)
        {
            cardObj.transform.position = Vector3.Lerp(startPos, endPos, elapsedTime / cardMoveDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        cardObj.transform.position = endPos;
    }

    // ========== �����жϷ��� ==========
    private int CalculateDamage(List<Card> cards)
    {
        if (IsWangZha(cards)) return 10;
        if (IsTongHuaShun(cards)) return 8 + (cards.Count - 5) * 3;
        if (IsShunZi(cards)) return 5 + (cards.Count - 5) * 1;
        if (IsSanDaiYi(cards)) return 3;
        if (IsDuiZi(cards)) return 2;
        return 1; // ����
    }

    private bool IsDuiZi(List<Card> cards)
    {
        return cards.Count == 2 && cards[0].number == cards[1].number;
    }

    private bool IsSanDaiYi(List<Card> cards)
    {
        if (cards.Count != 4) return false;

        Dictionary<int, int> count = new Dictionary<int, int>();
        foreach (Card card in cards)
        {
            if (count.ContainsKey(card.number))
                count[card.number]++;
            else
                count[card.number] = 1;
        }

        return (count.Count == 2 && (count.ContainsValue(3) || count.ContainsValue(1)));
    }

    private bool IsShunZi(List<Card> cards)
    {
        if (cards.Count < 5) return false;

        List<Card> sortedCards = new List<Card>(cards);
        sortedCards.Sort((a, b) => a.number.CompareTo(b.number));

        // ����Ƿ�����
        for (int i = 1; i < sortedCards.Count; i++)
        {
            if (sortedCards[i].number != sortedCards[i - 1].number + 1)
                return false;
        }

        return true;
    }

    private bool IsTongHuaShun(List<Card> cards)
    {
        if (cards.Count < 5) return false;

        // ���ͬ��
        string type = cards[0].type;
        foreach (Card card in cards)
        {
            if (card.type != type) return false;
        }

        // ���˳��
        return IsShunZi(cards);
    }

    private bool IsWangZha(List<Card> cards)
    {
        return cards.Count == 2 &&
               cards[0].number >= 14 &&
               cards[1].number >= 14;
    }

    private IEnumerator ShakeCharacter(Image characterImage)
    {
        Vector3 originalPos = characterImage.rectTransform.anchoredPosition;
        float elapsed = 0f;
        float currentShakeAmount = shakeAmount;

        while (elapsed < shakeDuration)
        {
            float x = originalPos.x + Random.Range(-currentShakeAmount, currentShakeAmount);
            float y = originalPos.y + Random.Range(-currentShakeAmount, currentShakeAmount);

            characterImage.rectTransform.anchoredPosition = new Vector3(x, y, originalPos.z);

            currentShakeAmount = Mathf.Max(0, currentShakeAmount - decreaseAmount * Time.deltaTime);
            elapsed += Time.deltaTime;
            yield return null;
        }

        characterImage.rectTransform.anchoredPosition = originalPos;
    }
}