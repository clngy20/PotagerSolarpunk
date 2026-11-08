using UnityEngine;
using TMPro;

public class TimerDisplay : MonoBehaviour
{
    [SerializeField] private GameTimer timer;
    [SerializeField] private TMP_Text timerText;

    private void OnEnable()
    {
        timer.OnTimeChanged += UpdateText;
    }

    private void OnDisable()
    {
        timer.OnTimeChanged -= UpdateText;
    }

    private void UpdateText(float timeLeft)
    {
        timerText.text = $"Temps : {Mathf.CeilToInt(timeLeft)}";
    }
}
