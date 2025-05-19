using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager3 : MonoBehaviour
{
    public static GameManager3 Instance;
    public static int savedBossHealth = -1;
    public static int savedplayer = -1;
    [Header("Spirit Settings")]
    public Image spiritImage; // ��Spirit��ť��Image���
    public string videoSceneName = "VideoScene"; // ��Ƶ��������
    public string videoSceneName2 = "VideoScene"; // ��Ƶ��������

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

    [Header("Damage Effect Settings")]
    public Image bossImage;        // Boss��ͼƬ
    public Image playerImage;      // ��ҵ�ͼƬ
    public float shakeDuration = 0.5f;  // ��������ʱ��
    public float shakeAmount = 5f;      // ��������
    public float decreaseAmount = 1f;    // �������ȵݼ��ٶ�

    [Header("Health Bar Settings")]
    public Image playerHealthFill;  // �ֶ��� PlayerHealthBar �� Fill ͼ�����
    public Sprite[] fillSprites;    // �洢��ѡ�� Fill ͼ��
    private int currentFillIndex = 0; // ��ǰʹ�õ�ͼ������

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

    public string currentScene;
    private bool sceneSwitched = false; // ������־λ��ֹ�ظ��л�

    private void Awake()
    {
        Debug.Log("GameManager3 Awake ������");

        if (Instance != null && Instance != this)
        {
            Debug.Log("�����ظ��� GameManager3 ʵ��");
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        currentScene = SceneManager.GetActiveScene().name;
        if(currentScene=="boss")
        {
            initialPlayerHealth = 9999;
        }
        Debug.Log("GameManager3 Start ������");
        SceneManager.sceneLoaded += OnSceneLoaded;
        InitializeGame();

        if (spiritImage != null)
        {
            Button spiritButton = spiritImage.GetComponent<Button>();
            spiritButton.onClick.RemoveAllListeners(); // ��ֹ�ظ���
            spiritButton.onClick.AddListener(OnSpiritClicked);
        }
        else
        {
            Debug.LogError("Spirit Imageδ�󶨣�");
        }

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

        if (discardPilePosition == null)
        {
            Debug.LogError("Discard pile position is not set!");
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"�����������: {scene.name}"); // ��Ӵ���
        // ������ص��ǵ�ǰ����
        if (scene.name == gameObject.scene.name)
        {
            // ���³�ʼ����BossѪ�������������
            playerHealth = initialPlayerHealth;
            bossAttack = initialBossAttack;
            turnCount = 0;
            gameOver = false;

            InitializeDeck();
            ShuffleDeck();
            DealInitialCards();
            UpdateUI();
            // ���� Fill ͼ��
            if (playerHealthFill != null && fillSprites != null && fillSprites.Length > 0)
            {
                currentFillIndex = (currentFillIndex + 1) % fillSprites.Length;
                playerHealthFill.sprite = fillSprites[currentFillIndex];
                playerHealthFill.SetNativeSize();

                // ǿ��ˢ�� Image
                playerHealthFill.enabled = false;
                playerHealthFill.enabled = true;

                Debug.Log($"�Ѹ���Ϊ Sprite: {fillSprites[currentFillIndex].name}"); // ��Ӵ���
            }
            else
            {
                Debug.LogError("playerHealthFill �� fillSprites δ��ȷ���ã�");
            }
        }
        if(scene.name == "boss")
        {
            // ���³�ʼ����BossѪ�������������
            playerHealth = 9999;
            bossAttack = initialBossAttack;
            turnCount = 0;
            gameOver = false;

            InitializeDeck();
            ShuffleDeck();
            DealInitialCards();
            UpdateUI();
        }
    }

    private void OnDestroy()
    {
        // �Ƴ��¼�����
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    private void OnSpiritClicked()
    {
        Debug.Log("�����Spirit��ť��");
        savedplayer = 9999;
        savedBossHealth = bossHealth;
        if (SceneTransitionManager1.Instance != null)
        {
            SceneTransitionManager1.Instance.LoadSceneWithTransition(videoSceneName);
        }
        else
        {
            SceneManager.LoadScene(videoSceneName);
        }
    }

    private void InitializeGame()
    {
        playerHealth = savedplayer > 0 ? savedplayer : initialPlayerHealth;
        bossHealth = savedBossHealth > 0 ? savedBossHealth : initialBossHealth;

        // ���ñ����Ѫ������ֹӰ���������Ϸ��
        savedplayer = -1;
        savedBossHealth = -1;
        bossAttack = initialBossAttack;
        turnCount = 0;
        gameOver = false;

        InitializeDeck();
        ShuffleDeck();
        DealInitialCards();
        UpdateUI();

        // ��ʼ��ť״̬
        playButton.gameObject.SetActive(true);
        discardButton.gameObject.SetActive(false); // ��ʼ�������ư�ť
        skipButton.gameObject.SetActive(false);    // ��ʼ����������ť

        playButton.onClick.AddListener(PlayCards);
        discardButton.onClick.AddListener(DiscardCards);
        skipButton.onClick.AddListener(SkipTurn);
    }

    private void InitializeDeck()
    {
        deck.Clear();
        discardPile.Clear();

        string[] types = { "Spade", "Heart", "Diamond", "Club" };

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
        for (int i = 0; i < 10; i++)
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
        cardObj.GetComponent<CardDisplay2>().Initialize(card);
        playerHandCards.Add(cardObj);

        UpdateHandLayout();
        UpdateDeckCount();
    }

    private void UpdateHandLayout()
    {
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
        int[] indices = new int[playerHand.Count];
        for (int i = 0; i < indices.Length; i++)
        {
            indices[i] = i;
        }

        System.Array.Sort(indices, (a, b) => playerHand[a].GetSortValue().CompareTo(playerHand[b].GetSortValue()));

        List<Card> sortedHand = new List<Card>();
        List<GameObject> sortedHandCards = new List<GameObject>();

        foreach (int index in indices)
        {
            sortedHand.Add(playerHand[index]);
            sortedHandCards.Add(playerHandCards[index]);
        }

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
        if (currentScene == "boss")
        {
            damage *= 3;
        }
        UpdateTurnInfo($"������ {GetCardCombinationName(selectedCards)}����� {damage} ���˺���");
        StartCoroutine(MoveCardsToCenterAndDamage(damage));
    }

    public void DiscardCards()
    {
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

        StartCoroutine(WaitAndEndTurn());
    }

    private IEnumerator MoveCardToDiscard(GameObject cardObj, Card card)
    {
        CanvasGroup canvasGroup = cardObj.GetComponent<CanvasGroup>();
        if (canvasGroup != null) canvasGroup.blocksRaycasts = false;

        Vector3 startPos = cardObj.transform.position;
        Vector3 targetPos = discardPilePosition.position;

        float elapsedTime = 0f;

        while (elapsedTime < cardMoveDuration)
        {
            cardObj.transform.position = Vector3.Lerp(startPos, targetPos, elapsedTime / cardMoveDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        cardObj.transform.position = targetPos;

        playerHand.Remove(card);
        playerHandCards.Remove(cardObj);
        discardPile.Add(card);
        Destroy(cardObj);

        UpdateHandLayout();
        discardButton.interactable = true;
    }

    private IEnumerator WaitAndEndTurn()
    {
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

        int cardsNeeded = Mathf.Max(0, 10 - playerHand.Count);
        int cardsToDraw = Mathf.Min(cardsNeeded, deck.Count);

        for (int i = 0; i < cardsToDraw; i++)
        {
            DrawCard();
        }

        playButton.gameObject.SetActive(true);
        discardButton.gameObject.SetActive(false);
        skipButton.gameObject.SetActive(false);
        canPlayCards = true;

        StartCoroutine(BossAttackAtStart());
    }

    private IEnumerator MoveCardsToCenterAndDamage(int damage)
    {
        canPlayCards = false;
        playButton.interactable = false;

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

                originalWorldPositions.Add(cardObj.transform.position);
                originalScales.Add(cardObj.transform.localScale);
            }
        }

        Canvas mainCanvas = transform.parent.GetComponent<Canvas>();
        foreach (GameObject cardObj in cardsToMove)
        {
            Vector3 worldPos = cardObj.transform.position;

            cardObj.transform.SetParent(mainCanvas.transform);
            cardObj.transform.position = worldPos;

            Canvas cardCanvas = cardObj.GetComponent<Canvas>();
            if (cardCanvas != null)
            {
                cardCanvas.overrideSorting = true;
                cardCanvas.sortingOrder = 999;
            }
        }

        indicesToRemove.Sort((a, b) => b.CompareTo(a));
        foreach (int index in indicesToRemove)
        {
            playerHand.RemoveAt(index);
            playerHandCards.RemoveAt(index);
        }

        UpdateHandLayout();

        float staggerDelay = 0.1f; // Delay between each card's animation
        for (int i = 0; i < cardsToMove.Count; i++)
        {
            StartCoroutine(MoveCardFromPosition(cardsToMove[i], originalWorldPositions[i]));
            yield return new WaitForSeconds(staggerDelay);
        }

        yield return new WaitForSeconds(cardMoveDuration);

        bossHealth -= damage;
        UpdateUI();

        StartCoroutine(ShakeCharacter(bossImage));

        foreach (GameObject cardObj in cardsToMove)
        {
            discardPile.Add(cardsToRemove[cardsToMove.IndexOf(cardObj)]);
            Destroy(cardObj);
        }

        selectedCards.Clear();
        UpdateTurnInfo("ѡ�����ƻ�ȡ��");

        playButton.gameObject.SetActive(false);
        discardButton.gameObject.SetActive(true);
        skipButton.gameObject.SetActive(true);
        playButton.interactable = true;
    }

    private IEnumerator MoveCardFromPosition(GameObject cardObj, Vector3 startPosition)
    {
        float elapsedTime = 0f;
        Vector3 targetPos = playAreaCenter.position;

        cardObj.transform.position = startPosition;
        Vector3 originalScale = cardObj.transform.localScale;

        while (elapsedTime < cardMoveDuration)
        {
            float t = elapsedTime / cardMoveDuration;

            float smoothT = t * t * (3f - 2f * t);

            Vector3 pos = Vector3.Lerp(startPosition, targetPos, smoothT);

            float scaleMultiplier = Mathf.Lerp(1f, 0.8f, smoothT);
            cardObj.transform.localScale = originalScale * scaleMultiplier;

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

        StartCoroutine(ShakeCharacter(playerImage));

        if (!gameOver)
        {
            UpdateTurnInfo("��Ļغ� - �����");
        }
    }

    private void ResetAllCardsSelection()
    {
        foreach (GameObject cardObj in playerHandCards)
        {
            CardDisplay2 display = cardObj.GetComponent<CardDisplay2>();
            if (display != null)
            {
                display.ResetSelection();
            }
        }

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
        playerHealthBar.value = Mathf.Clamp01((float)playerHealth / initialPlayerHealth);
        bossHealthBar.value = Mathf.Clamp01((float)bossHealth / initialBossHealth);

        if (spiritImage != null)
        {
            bool shouldShow = playerHealth < 10 && playerHealth > 0;
            spiritImage.gameObject.SetActive(shouldShow);
        }

        if (playerHealth <= 0)
        {
            playerHealthBar.fillRect.gameObject.SetActive(false);
            if (SceneTransitionManager1.Instance != null)
            {
                SceneTransitionManager1.Instance.LoadSceneWithTransition("cuoshi3");
            }
            else
            {
                SceneManager.LoadScene(videoSceneName2);
            }
            GameOver(false);
        }
        if (bossHealth <= 50 && !sceneSwitched && !gameOver)
        {
            sceneSwitched = true;

            // ʹ�ó������ɹ������л�����
            if (SceneTransitionManager1.Instance != null)
            {
                SceneTransitionManager1.Instance.LoadSceneWithTransition(videoSceneName2);
            }
            else
            {
                SceneManager.LoadScene(videoSceneName2);
            }

            // ��ѡ��������ǰ��Ϸ״̬
            savedBossHealth = bossHealth;
            savedplayer = playerHealth;
        }
        UpdateDeckCount();
    }


    public void AddSelectedCard(Card card, GameObject cardObj)
    {
        if (!selectedCards.Contains(card))
        {
            selectedCards.Add(card);
        }
    }

    public void RemoveSelectedCard(Card card, GameObject cardObj)
    {
        if (selectedCards.Contains(card))
        {
            selectedCards.Remove(card);
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
        if (IsZhaDan(cards)) return "ը��";
        return "����";
    }

    private int CalculateDamage(List<Card> cards)
    {
        if (IsWangZha(cards)) return 10;
        if (IsZhaDan(cards)) return 10;
        if (IsTongHuaShun(cards)) return 8 + (cards.Count - 5) * 3;
        if (IsShunZi(cards)) return 5 + (cards.Count - 5) * 1;
        if (IsSanDaiYi(cards)) return 3;
        if (IsDuiZi(cards)) return 2;
        return 1;
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

        string type = cards[0].type;
        foreach (Card card in cards)
        {
            if (card.type != type) return false;
        }

        return IsShunZi(cards);
    }

    private bool IsWangZha(List<Card> cards)
    {
        // ԭ���жϣ��������ƣ���ֵ>=14��
        bool isDoubleJoker = (cards.Count == 2 &&
                             cards[0].number >= 14 &&
                             cards[1].number >= 14);
 
        return isDoubleJoker ;
    }

    private bool IsZhaDan(List<Card> cards)
    {
        // �����жϣ�������ͬ��ֵ����
        bool isFourOfAKind = false;
        if (cards.Count == 4)
        {
            Dictionary<int, int> count = new Dictionary<int, int>();
            foreach (Card card in cards)
            {
                if (count.ContainsKey(card.number))
                    count[card.number]++;
                else
                    count[card.number] = 1;
            }
            isFourOfAKind = count.ContainsValue(4);
        }
        return isFourOfAKind;
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