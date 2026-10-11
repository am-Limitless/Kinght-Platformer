using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject winPannel;

    public GameObject GameUI;

    public WinTrigger winTriggerScript;

    [SerializeField] private float DelayTimer;

    private Move moveScript;
    private bool winSequenceStarted;

    private void Start()
    {
        moveScript = FindFirstObjectByType<Move>();
    }

    private void Update()
    {
        if (winTriggerScript.winTrigger && !winSequenceStarted)
        {
            winSequenceStarted = true;
            StartCoroutine(WinMenuTimer());
            moveScript.DisableMovement();
        }
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void LoadNextLevel()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

        int nextSceneIndex = GetNextSceneIndex(currentSceneIndex);

        if (nextSceneIndex != -1)
        {
            SceneManager.LoadScene(nextSceneIndex);
            moveScript.EnableMovement();
        }
    }

    private int GetNextSceneIndex(int currentSceneIndex)
    {
        const int maxSceneIndex = 20;

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

    IEnumerator WinMenuTimer()
    {
        Debug.Log("Starting WinMenuTimer Coroutine.");
        yield return new WaitForSeconds(DelayTimer);

        if (winPannel != null)
        {
            GameUI.SetActive(false);
            winPannel.SetActive(true);
            Debug.Log("Win Panel activated.");
        }
    }
}
