using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameTimer : MonoBehaviour
{
    [SerializeField] private float gameDuration = 60f;
    [SerializeField] private ScoreManager scoreManager;

    public event Action<float> OnTimeChanged;

    private float timeLeft;
    private bool ended;

    private void Start()
    {
        timeLeft = gameDuration;
        OnTimeChanged?.Invoke(timeLeft);
    }

    private void Update()
    {
        if (ended) return;

        timeLeft -= Time.deltaTime;
        if (timeLeft <= 0f)
        {
            timeLeft = 0f;
            OnTimeChanged?.Invoke(timeLeft);
            EndGame();
            return;
        }
        OnTimeChanged?.Invoke(timeLeft);
    }

    private void EndGame()
    {
        ended = true;
        GameResult.Submit(scoreManager.Score);
        SceneManager.LoadScene("End");
    }
}