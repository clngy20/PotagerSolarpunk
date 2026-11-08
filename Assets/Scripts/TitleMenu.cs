using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleMenu : MonoBehaviour
{
    public void Play()
    {
        SceneManager.LoadScene("Garden");
    }

    public void Quit()
    {
        Application.Quit();
    }
}
