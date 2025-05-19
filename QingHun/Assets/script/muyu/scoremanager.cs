// ScoreManager.cs
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;
    public TextMeshProUGUI totalScoreText;
    public float totalScore;
    public string currentScene;

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

    public void AddScore(float amount)
    {
        totalScore += amount;
        UpdateTotalScoreDisplay();
    }

    private void UpdateTotalScoreDisplay()
    {
        currentScene = SceneManager.GetActiveScene().name;
        if (totalScoreText != null&& currentScene=="RhythmGame")
        {
            totalScoreText.text = $"得分: {totalScore:F0}";
        }
        if (totalScoreText != null && currentScene == "muyu")
        {
            totalScoreText.text = $"得分: {totalScore:F0}";
        }
    }
}