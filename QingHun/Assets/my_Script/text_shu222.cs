using System.Collections;
using UnityEngine;
using TMPro;
using System.Text;
using UnityEngine.SceneManagement; // ȷ�����볡������

public class text_shu222 : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text textComponent;
    public CanvasGroup canvasGroup; // ���ڵ��뵭��

    [Header("Timing")]
    public float charDelay = 0.05f; // �ַ���ʾ���
    public float fadeDuration = 0.5f; // ���뵭������ʱ��

    [Header("Navigation")]
    public string nextSceneName; // ����ʱ���ص���һ��������

    // Ԥ����İ�����
    private readonly string[] textPassages = new string[]
    {
        // �İ�һ (ʹ�� @ ʹ�ַ������Կ��У�����������)
        @"С�ɾ�������
......
�����ǲ������ļ�����",
        // �İ���
        @"����˵Ц�ˣ�
�Ҽ������������е�����ġ�
���������ģ�
���Ҵ��ɡ�ظ��ɺã�"
    };

    private int currentPassageIndex = 0;
    private bool isTyping = false;
    private bool canClickToAdvance = false;
    private bool isLastPassage = false;
    private Coroutine displayCoroutine; // ���ڸ��ٴ���Э��

    void Awake()
    {
        if (textComponent == null)
        {
            Debug.LogError("Text Component δ�� Inspector ��ָ��!");
            enabled = false;
            return;
        }
        if (canvasGroup == null)
        {
            canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }
        }

        // ��ʼ�� TextMeshPro ����
        // ������ת�ı���RectTransform �� Height ��Ҫ�㹻����������ת���ı���"����"
        // Width ��Ҫ�㹻������ת���ı���"����"����ԭʼ�ı���������
        RectTransform rect = textComponent.GetComponent<RectTransform>();
        if (rect != null)
        {
            // ���������ı����Ⱥ���������������Ҫ������Щֵ
            // ���磬�����Ķ�����ת����Ҫ800����Ļ�߶ȣ�����ת���ɿ�Ⱥ���Ҫ400
            // rect.sizeDelta = new Vector2(400, 800); 
        }
        textComponent.enableWordWrapping = false;
        textComponent.text = "";
        canvasGroup.alpha = 1f;
    }

    void Start()
    {
        if (textPassages.Length > 0)
        {
            displayCoroutine = StartCoroutine(DisplayCurrentPassage());
        }
        else
        {
            Debug.LogWarning("û���ṩ�κ��ı����ݡ�");
        }
    }

    void Update()
    {
        // ȷ�����ڴ���ʱ���ܵ�������ҿ��Ե��
        if (canClickToAdvance && Input.GetMouseButtonDown(0) && !isTyping)
        {
            if (isLastPassage)
            {
                HandleEndOfPassages();
            }
            else
            {
                // ֹͣ�κο����������еľɵĴ���Э��
                if (displayCoroutine != null)
                {
                    StopCoroutine(displayCoroutine);
                }
                StartCoroutine(AdvanceToNextPassage());
            }
        }
    }

    IEnumerator DisplayCurrentPassage()
    {
        isTyping = true;
        canClickToAdvance = false;
        textComponent.text = ""; // ��գ�ȷ���ɵ� rotate ��ǩ���Ƴ�

        if (currentPassageIndex < textPassages.Length)
        {
            string passageToShow = textPassages[currentPassageIndex];
            StringBuilder currentTypedText = new StringBuilder();

            foreach (char c in passageToShow)
            {
                currentTypedText.Append(c);
                textComponent.text = "<rotate=90>" + currentTypedText.ToString() + "</rotate>";
                yield return new WaitForSeconds(charDelay);
            }

            // �������Ϊ�գ�ȷ����ǩ�պ�
            if (string.IsNullOrEmpty(passageToShow))
            {
                textComponent.text = "<rotate=90></rotate>";
            }

            isLastPassage = (currentPassageIndex == textPassages.Length - 1);
            canClickToAdvance = true; // ������������һ��
        }
        isTyping = false;
    }

    IEnumerator AdvanceToNextPassage()
    {
        canClickToAdvance = false;

        // ������ǰ�ı�
        float timer = 0f;
        while (timer < fadeDuration)
        {
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, timer / fadeDuration);
            timer += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = 0f; // ȷ����ȫ����

        currentPassageIndex++;

        // ��ʼ��ʾ��һ�� (����������ʼ���֣�����Ϊ alpha=0 ������ʱ���ɼ�)
        displayCoroutine = StartCoroutine(DisplayCurrentPassage());

        // �����µ��ı�
        timer = 0f;
        while (timer < fadeDuration)
        {
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, timer / fadeDuration);
            timer += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = 1f; // ȷ����ȫ����
    }

    void HandleEndOfPassages()
    {
        Debug.Log("�����İ���ʾ��ϡ�");
        canClickToAdvance = false; // ��ֹ�ٴε��
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneTransitionManager1.Instance.LoadSceneWithTransition(nextSceneName);
        }
        // ���û��ָ�� nextSceneName�����ı���ͣ�����һ�䣬������Ӧ���
    }
}