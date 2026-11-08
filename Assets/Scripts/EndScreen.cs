using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class EndScreen : MonoBehaviour
{
    [SerializeField] private TMP_Text scoreText;

    private void Start()
    {
        scoreText.text = $"Score final : {GameResult.LastScore}";
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
