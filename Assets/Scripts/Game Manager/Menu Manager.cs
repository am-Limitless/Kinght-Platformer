using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    private PlayerLifeManager playerLifeManager;

    private void Start()
    {
        playerLifeManager = FindObjectOfType<PlayerLifeManager>();
    }
    public void QuitGame()
    {
        Application.Quit();
    }

    public void LevelOneLoad()
    {
        Time.timeScale = 1.0f;
        if (playerLifeManager != null)
        {
            // Call the ResetPlayerLives() method to reset player lives before starting a new game
            playerLifeManager.ResetPlayerLives();
        }
        else
        {
            Debug.LogWarning("PlayerLifeManager not found in the scene. Lives will not be reset.");
        }
        SceneManager.LoadScene("Level 1");
    }
}
