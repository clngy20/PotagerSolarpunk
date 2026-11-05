using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public int Score { get; private set; }

    public void AddScore(int amount)
    {
        Score += amount;
        Debug.Log($"Score : {Score}");
    }
}