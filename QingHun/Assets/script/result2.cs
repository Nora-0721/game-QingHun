using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class ResultManager2 : MonoBehaviour
{
    public AudioSource musicSource;
    public TMP_Text mainText;    // ���ı������֣�
    public TMP_Text subText;     // ���ı���С�֣�
    public float delayAfterText = 1f;

    [System.Serializable]
    public class TierConfig
    {
        [TextArea(3, 10)] public string mainText;
        [TextArea(3, 10)] public string subText;
        public string firstScene;
        public string nextScene;
    }

    [Header("�㼶����")]
    public TierConfig tier0;
    public TierConfig tier1;
    public TierConfig tier2;

    private bool isProcessing;   // �����ֶ���
    private TierConfig currentTier;

    void Update()
    {
        if (ShouldStartResult() && !isProcessing) // ʹ�� isProcessing
        {
            StartCoroutine(ResultProcess());
        }
    }

    private bool ShouldStartResult()
    {
        return musicSource != null &&
               !musicSource.isPlaying &&
               ScoreManager.Instance != null;
    }

    // ��ResultManager.cs��Awake���������
    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }
    private IEnumerator ResultProcess()
    {
        isProcessing = true;

        float score = Mathf.Clamp(ScoreManager.Instance.totalScore, 0, 3000);
        currentTier = GetTierConfig(score);

        mainText.text = "";
        subText.text = "";

        yield return StartCoroutine(TypeText(mainText, currentTier.mainText));

        if (score < 1400)
        {
            yield return StartCoroutine(TypeText(subText, currentTier.subText));
        }

        // �޸ĳ����л��߼�
        yield return StartCoroutine(ExecuteSceneTransitions());

        isProcessing = false;
    }

    private IEnumerator ExecuteSceneTransitions()
    {
        // ��һ�γ����л���ʹ�ûص�ȷ����ɣ�
        bool firstTransitionDone = false;
        SceneTransitionManager1.Instance.LoadSceneWithTransition(currentTier.firstScene, () => {
            firstTransitionDone = true;
        });

        // �ȴ���һ���л����
        yield return new WaitUntil(() => firstTransitionDone);

        // �ȴ�4�루ԭ����Ϊ2�룬�������޸ģ�
        yield return new WaitForSeconds(4f);

        // �ڶ��γ����л�
        bool secondTransitionDone = false;
        SceneTransitionManager1.Instance.LoadSceneWithTransition(currentTier.nextScene, () => {
            secondTransitionDone = true;
        });

        yield return new WaitUntil(() => secondTransitionDone);
    }
    private TierConfig GetTierConfig(float score)
    {
        if (score < 1400) return tier0;
        if (score < 1800) return tier1;
        return tier2;
    }

    private IEnumerator TypeText(TMP_Text target, string content)
    {
        target.text = "";
        foreach (char c in content)
        {
            target.text += c;
            yield return new WaitForSeconds(0.05f);
        }
    }
}