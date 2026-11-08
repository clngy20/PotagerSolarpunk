using UnityEngine;

public static class GameResult
{
    private const string BestScoreKey = "BestScore";

    public static int LastScore;
    public static bool IsNewBest { get; private set; }

    //0 de base si y'au aucun score enregistré
    public static int BestScore => PlayerPrefs.GetInt(BestScoreKey, 0);

    public static void Submit(int score)
    {
        LastScore = score;
        IsNewBest = score > BestScore;
        if (IsNewBest)
        {
            PlayerPrefs.SetInt(BestScoreKey, score);
            PlayerPrefs.Save();
        }
    }
}
