using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSelectMenu : MonoBehaviour
{
    [Serializable]
    private class LevelButtonView
    {
        public Button button;
        public GameObject lockIcon;
        public TMP_Text numberLabel;
    }

    [Header("Levels (first entry is level 1)")]
    [SerializeField] private LevelButtonView[] levelButtons;
    [SerializeField] private Button backButton;

    [Header("Scenes and lives")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";
    [Tooltip("Lives the player starts with when a level is picked from this menu.")]
    [SerializeField, Min(1)] private int startingLives = 3;

    private void Start()
    {
        Time.timeScale = 1f;
        RefreshLevelButtons();

        if (backButton != null)
        {
            backButton.onClick.AddListener(BackToMainMenu);
        }
    }

    // On Android the Back button is reported as Escape.
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            BackToMainMenu();
        }
    }

    private void RefreshLevelButtons()
    {
        int highestUnlockedLevel = LevelProgress.HighestUnlockedLevel;

        for (int i = 0; i < levelButtons.Length; i++)
        {
            int levelNumber = LevelProgress.FirstLevel + i;
            bool isUnlocked = levelNumber <= highestUnlockedLevel;
            LevelButtonView view = levelButtons[i];

            view.button.interactable = isUnlocked;
            view.button.onClick.AddListener(() => LoadLevel(levelNumber));

            if (view.lockIcon != null)
            {
                view.lockIcon.SetActive(!isUnlocked);
            }

            if (view.numberLabel != null)
            {
                view.numberLabel.text = levelNumber.ToString();
                view.numberLabel.gameObject.SetActive(isUnlocked);
            }
        }
    }

    private void LoadLevel(int levelNumber)
    {
        if (!LevelProgress.IsUnlocked(levelNumber))
        {
            return;
        }

        PlayerPrefs.SetInt(PlayerLifeManager.CurrentLivesKey, startingLives);
        SceneManager.LoadScene(LevelProgress.GetSceneName(levelNumber));
    }

    private void BackToMainMenu()
    {
        SceneManager.LoadScene(mainMenuSceneName);
    }
}
