using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class EndScreen : MonoBehaviour
{
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text bestScoreText;

    private void Start()
    {
        scoreText.text = $"Score final : {GameResult.LastScore}";

        bestScoreText.text = GameResult.IsNewBest
            ? $"Nouveau record : {GameResult.BestScore} !"
            : $"Meilleur score : {GameResult.BestScore}";
    }

    public void Replay()
    {
        SceneManager.LoadScene("Garden");
    }

    public void BackToTitle()
    {
        SceneManager.LoadScene("Title");
    }
}
