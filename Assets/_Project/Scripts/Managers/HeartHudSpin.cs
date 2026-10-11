using System.Collections;
using UnityEngine;

// Spins the HUD heart once, like the heart pickup in the level, whenever the player gains a life.
public class HeartHudSpin : MonoBehaviour
{
    [Tooltip("The heart picture that spins. Must not be a parent of the lives text, or the text would spin too.")]
    [SerializeField] private RectTransform spinTarget;
    [SerializeField, Min(0.1f)] private float spinDurationSeconds = 1f;
    [Tooltip("Degrees around the vertical axis. The level heart pickup turns a full 360.")]
    [SerializeField] private float spinDegrees = 360f;

    private Coroutine spinRoutine;

    private void OnEnable()
    {
        PlayerLifeManager.LifeGained += Spin;
    }

    private void OnDisable()
    {
        PlayerLifeManager.LifeGained -= Spin;
        spinRoutine = null;
        if (spinTarget != null)
        {
            spinTarget.localRotation = Quaternion.identity;
        }
    }

    private void Spin()
    {
        if (spinTarget == null)
        {
            return;
        }

        if (spinRoutine != null)
        {
            StopCoroutine(spinRoutine);
        }

        spinRoutine = StartCoroutine(SpinRoutine());
    }

    private IEnumerator SpinRoutine()
    {
        float elapsed = 0f;
        while (elapsed < spinDurationSeconds)
        {
            elapsed += Time.unscaledDeltaTime;
            float angle = spinDegrees * Mathf.Clamp01(elapsed / spinDurationSeconds);
            spinTarget.localRotation = Quaternion.Euler(0f, angle, 0f);
            yield return null;
        }

        spinTarget.localRotation = Quaternion.identity;
        spinRoutine = null;
    }
}
