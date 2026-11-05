using TMPro;
using UnityEngine;

public class ScoreDisplay : MonoBehaviour
{
    [SerializeField] private ScoreManager scoreManager;
    [SerializeField] private TMP_Text scoreText;

    private void OnEnable()
    {
        scoreManager.OnScoreChanged += UpdateScoreText;
        UpdateScoreText(scoreManager.Score); // Initialiser l'affichage au démarrage
    }

    private void OnDisable()
    {
        scoreManager.OnScoreChanged -= UpdateScoreText;
    }

    private void UpdateScoreText(int score)
    {
        scoreText.text = $"Score: {score}";
    }
}
