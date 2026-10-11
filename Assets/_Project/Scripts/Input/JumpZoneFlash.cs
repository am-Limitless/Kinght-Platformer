using System.Collections;
using UnityEngine;
using UnityEngine.UI;

// Pulses the jump zone image to show the player where to tap. Visual only: it never touches input.
[RequireComponent(typeof(Image))]
public class JumpZoneFlash : MonoBehaviour
{
    [Header("Flash")]
    [SerializeField] private Color flashColor = new Color(1f, 1f, 1f, 0.3f);
    [SerializeField, Range(1, 5)] private int flashCount = 2;
    [SerializeField, Min(0.05f)] private float fadeInSeconds = 0.2f;
    [SerializeField, Min(0.05f)] private float fadeOutSeconds = 0.3f;

    private Image zoneImage;
    private Color restingColor;
    private Coroutine flashRoutine;

    private void Awake()
    {
        zoneImage = GetComponent<Image>();
        restingColor = zoneImage.color;
    }

    public void Flash()
    {
        if (flashRoutine != null)
        {
            StopCoroutine(flashRoutine);
        }

        flashRoutine = StartCoroutine(FlashRoutine());
    }

    private void OnDisable()
    {
        flashRoutine = null;
        if (zoneImage != null)
        {
            zoneImage.color = restingColor;
        }
    }

    private IEnumerator FlashRoutine()
    {
        for (int i = 0; i < flashCount; i++)
        {
            yield return Fade(restingColor, flashColor, fadeInSeconds);
            yield return Fade(flashColor, restingColor, fadeOutSeconds);
        }

        flashRoutine = null;
    }

    private IEnumerator Fade(Color from, Color to, float seconds)
    {
        float elapsed = 0f;
        while (elapsed < seconds)
        {
            elapsed += Time.unscaledDeltaTime;
            zoneImage.color = Color.Lerp(from, to, elapsed / seconds);
            yield return null;
        }

        zoneImage.color = to;
    }
}
