using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject pausePanel;

    private PlayerControls controls;
    private bool isPaused;

    private void Awake()
    {
        controls = new PlayerControls();
    }

    private void OnEnable()
    {
        controls.Enable();
        controls.Player.Pause.performed += OnPausePressed;
    }

    private void OnDisable()
    {
        controls.Player.Pause.performed -= OnPausePressed;
        controls.Disable();
    }

    private void OnPausePressed(InputAction.CallbackContext context)
    {
        if (isPaused)
        {
            Resume();
        }
        else
        {
            Pause();
        }
    }

    private void Pause()
    {
        isPaused = true;
        pausePanel.SetActive(true);
        Time.timeScale = 0f; //Figer le jeu
    }

    public void Resume()
    {
        isPaused = false;
        pausePanel.SetActive(false);
        Time.timeScale = 1f; //Reprendre le jeu
    }

    //Appeler par le btn recommencer
    public void Restart()
    {
        Time.timeScale = 1f; //Reprendre le jeu
        SceneManager.LoadScene("Garden");
    }

    //Appeler par le btn quitter
    public void QuitToTitle()
    {
        Time.timeScale = 1f; //Reprendre le jeu
        SceneManager.LoadScene("Title");
    }
}
