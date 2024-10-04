using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject winPannel;

    public WinTrigger winTriggerScript;

    public void Quit()
    {
        Application.Quit();
    }

    public void WinMenu()
    {
        winPannel.SetActive(true);
    }

    public void LoadNextLevel()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

        int nextSceneIndex = GetNextSceneIndex(currentSceneIndex);

        SceneManager.LoadScene(nextSceneIndex);
    }

    private int GetNextSceneIndex(int currentSceneIndex)
    {
        const int maxSceneIndex = 5; 

        if (currentSceneIndex < maxSceneIndex)
        {
            return currentSceneIndex + 1;
        }
        else
        {
            Debug.LogError("No next scene available");
            return -1; 
        }
    }
}
