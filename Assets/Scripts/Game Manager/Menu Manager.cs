using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{


    public void QuitGame()
    {
        Application.Quit();
    }

    public void LevelOneLoad()
    {
        Time.timeScale = 1.0f;
        SceneManager.LoadScene("Level 1");
    }
}
