using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class PauseButton : MonoBehaviour
{
    private Button _button;
    private PauseMenu _pauseMenu;

    private void Awake()
    {
        _button = GetComponent<Button>();
    }

    private void OnEnable()
    {
        _button.onClick.AddListener(OnPauseButtonClicked);
    }

    private void OnDisable()
    {
        _button.onClick.RemoveListener(OnPauseButtonClicked);
    }

    // The button lives in a shared prefab, so it finds the scene's PauseMenu the first time it is tapped.
    private void OnPauseButtonClicked()
    {
        if (_pauseMenu == null)
        {
            _pauseMenu = FindFirstObjectByType<PauseMenu>();
        }

        if (_pauseMenu == null)
        {
            Debug.LogWarning("PauseButton: no PauseMenu found in this scene.", this);
            return;
        }

        _pauseMenu.TogglePause();
    }
}
