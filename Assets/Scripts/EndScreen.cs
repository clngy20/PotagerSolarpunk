using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class EndScreen : MonoBehaviour
{
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text bestScoreText;

    private void Start()
    {
        scoreText.text = $"Score : {GameResult.LastScore}";

        if (GameResult.IsNewBest)
        {
            // Record battu : texte doré
            bestScoreText.text = $"Nouveau record ! {GameResult.BestScore}";
            bestScoreText.color = new Color(0.91f, 0.64f, 0.09f); // or
        }
        else
        {
            bestScoreText.text = $"Meilleur score : {GameResult.BestScore}";
        }
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
