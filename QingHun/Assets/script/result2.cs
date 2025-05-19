using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class ResultManager2 : MonoBehaviour
{
    public AudioSource musicSource;
    public TMP_Text mainText;    // 主文本（大字）
    public TMP_Text subText;     // 副文本（小字）
    public float delayAfterText = 1f;

    [System.Serializable]
    public class TierConfig
    {
        [TextArea(3, 10)] public string mainText;
        [TextArea(3, 10)] public string subText;
        public string firstScene;
        public string nextScene;
    }

    [Header("层级配置")]
    public TierConfig tier0;
    public TierConfig tier1;
    public TierConfig tier2;

    private bool isProcessing;   // 修正字段名
    private TierConfig currentTier;

    void Update()
    {
        if (ShouldStartResult() && !isProcessing) // 使用 isProcessing
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

    // 在ResultManager.cs的Awake方法中添加
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

        // 修改场景切换逻辑
        yield return StartCoroutine(ExecuteSceneTransitions());

        isProcessing = false;
    }

    private IEnumerator ExecuteSceneTransitions()
    {
        // 第一次场景切换（使用回调确保完成）
        bool firstTransitionDone = false;
        SceneTransitionManager1.Instance.LoadSceneWithTransition(currentTier.firstScene, () => {
            firstTransitionDone = true;
        });

        // 等待第一次切换完成
        yield return new WaitUntil(() => firstTransitionDone);

        // 等待4秒（原代码为2秒，按需求修改）
        yield return new WaitForSeconds(4f);

        // 第二次场景切换
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