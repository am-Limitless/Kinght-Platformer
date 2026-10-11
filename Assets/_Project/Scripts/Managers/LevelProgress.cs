using UnityEngine;
using UnityEngine.SceneManagement;

public static class LevelProgress
{
    public const int FirstLevel = 1;
    public const int LastLevel = 15;

    private const string HighestUnlockedKey = "HighestUnlockedLevel";
    private const string LevelScenePrefix = "Level ";

    public static int HighestUnlockedLevel =>
        Mathf.Clamp(PlayerPrefs.GetInt(HighestUnlockedKey, FirstLevel), FirstLevel, LastLevel);

    public static bool IsUnlocked(int levelNumber)
    {
        return levelNumber <= HighestUnlockedLevel;
    }

    public static string GetSceneName(int levelNumber)
    {
        return LevelScenePrefix + levelNumber;
    }

    public static void CompleteCurrentLevel()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        if (TryGetLevelNumber(sceneName, out int levelNumber))
        {
            CompleteLevel(levelNumber);
        }
    }

    public static void CompleteLevel(int levelNumber)
    {
        int nextLevel = Mathf.Min(levelNumber + 1, LastLevel);
        if (nextLevel <= HighestUnlockedLevel)
        {
            return;
        }

        PlayerPrefs.SetInt(HighestUnlockedKey, nextLevel);
        PlayerPrefs.Save();
    }

    private static bool TryGetLevelNumber(string sceneName, out int levelNumber)
    {
        levelNumber = 0;
        if (!sceneName.StartsWith(LevelScenePrefix))
        {
            return false;
        }

        return int.TryParse(sceneName.Substring(LevelScenePrefix.Length), out levelNumber);
    }
}
