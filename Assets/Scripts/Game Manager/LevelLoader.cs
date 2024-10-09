using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class LevelLoader : MonoBehaviour
{
    public GameObject loadingPannel;
    public Slider slider;

    // Adjusted method signature to accept an integer parameter
    public void LoadLevelInt(int sceneIndex)
    {
        StartCoroutine(LoadSceneAsynInt(sceneIndex));
    }

    public void LoadLevelString(string sceneName)
    {
        StartCoroutine(LoadSceneAsynString(sceneName)); // Load a scene by its name
    }

    public void RestartLevel()
    {
        int currentScene = SceneManager.GetActiveScene().buildIndex;
        StartCoroutine(LoadSceneAsynInt(currentScene));
    }

    public void Quit()
    {
        Application.Quit();
    }

    IEnumerator LoadSceneAsynInt(int sceneIndex)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);

        loadingPannel.SetActive(true);

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);  // Adjusted denominator to properly reflect progress range
            slider.value = progress;

            yield return null;
        }
    }

    IEnumerator LoadSceneAsynString(string sceneName)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);

        loadingPannel.SetActive(true);

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            slider.value = progress;
            yield return null;
        }
    }
}
