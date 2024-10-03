using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject WinPannel;

    public WinTrigger winTriggerScript;


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
        WinPannel.SetActive(true);
    }

    public void NextLevel()
    {
        Scene currentScene = SceneManager.GetActiveScene();

        int nextSceneIndex = currentScene.buildIndex + 1;

        SceneManager.LoadScene(nextSceneIndex);
    }
}
