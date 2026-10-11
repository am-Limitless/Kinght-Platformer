using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinPanel : MonoBehaviour
{
    [Header("Animated content")]
    [Tooltip("Scaled from small to full size when the panel opens.")]
    [SerializeField] private RectTransform content;
    [SerializeField, Min(0.05f)] private float popDurationSeconds = 0.45f;
    [Tooltip("Values above 1 make the pop overshoot a little before settling.")]
    [SerializeField, Range(1f, 2f)] private float popOvershoot = 1.15f;

    [Header("Optional texts")]
    [SerializeField] private TMP_Text levelLabel;
    [SerializeField] private TMP_Text livesLabel;

    [Header("Celebration")]
    [SerializeField] private ConfettiCelebration confettiPrefab;

    private const float PopStartScale = 0.4f;

    private void OnEnable()
    {
        HideTouchControls();
        RefreshTexts();
        PlayConfetti();

        if (content != null)
        {
            StartCoroutine(PopIn());
        }
    }

    // The joystick, jump zone and pause button are not needed once the level is won.
    private static void HideTouchControls()
    {
        TouchInputBinder touchControls = FindFirstObjectByType<TouchInputBinder>();
        if (touchControls != null)
        {
            touchControls.gameObject.SetActive(false);
        }
    }

    private void RefreshTexts()
    {
        if (levelLabel != null)
        {
            levelLabel.text = SceneManager.GetActiveScene().name.ToUpperInvariant();
        }

        if (livesLabel != null)
        {
            livesLabel.text = "x" + Mathf.Max(0, PlayerLifeManager.currentLives);
        }
    }

    private void PlayConfetti()
    {
        if (confettiPrefab == null || Camera.main == null)
        {
            return;
        }

        Instantiate(confettiPrefab).Play(Camera.main);
    }

    private IEnumerator PopIn()
    {
        content.localScale = Vector3.one * PopStartScale;

        float elapsed = 0f;
        while (elapsed < popDurationSeconds)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / popDurationSeconds);
            content.localScale = Vector3.one * Mathf.LerpUnclamped(PopStartScale, 1f, EaseOutBack(t, popOvershoot));
            yield return null;
        }

        content.localScale = Vector3.one;
    }

    private static float EaseOutBack(float t, float overshoot)
    {
        float c1 = (overshoot - 1f) * 17f;
        float c3 = c1 + 1f;
        float shifted = t - 1f;
        return 1f + c3 * shifted * shifted * shifted + c1 * shifted * shifted;
    }
}
