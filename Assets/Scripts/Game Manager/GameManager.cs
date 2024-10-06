using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject winPannel;

    public WinTrigger winTriggerScript;

    private void Start()
    {
        //DontDestroyOnLoad(winPannel);
    }

    private void Update()
    {
        if (winTriggerScript.winTrigger)
        {
            WinMenu();
        }
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void WinMenu()
    {
        if (winPannel != null)
        {
            winPannel.SetActive(true);
        }
    }

    public void LoadNextLevel()
    {
        Debug.Log("Button Pressed");

        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

        int nextSceneIndex = GetNextSceneIndex(currentSceneIndex);

        if (nextSceneIndex != -1)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
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
