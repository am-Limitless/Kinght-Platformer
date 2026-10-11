using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// On-screen "use" button for touch. An interactable (like a switch) shows it while the player is in range
// and hands over what should happen when it is tapped.
[RequireComponent(typeof(Button))]
public class InteractButton : MonoBehaviour
{
    [Header("Icon")]
    [Tooltip("Shows the same picture as the interactable's sprite, so the button reflects its current state.")]
    [SerializeField] private Image iconImage;

    [Header("Pop-in")]
    [SerializeField, Min(0.05f)] private float popDurationSeconds = 0.2f;
    [SerializeField, Range(0.1f, 1f)] private float popStartScale = 0.5f;

    private Button _button;
    private UnityAction _onPressed;
    private SpriteRenderer _mirroredSprite;

    private void Awake()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(HandleClick);
    }

    private void OnEnable()
    {
        StartCoroutine(PopIn());
    }

    private void OnDestroy()
    {
        if (_button != null)
        {
            _button.onClick.RemoveListener(HandleClick);
        }
    }

    public void Show(UnityAction onPressed, SpriteRenderer spriteToMirror)
    {
        _onPressed = onPressed;
        _mirroredSprite = spriteToMirror;
        MirrorSprite();
        gameObject.SetActive(true);
    }

    // Only the interactable that currently owns the button can hide it, so leaving one switch
    // cannot hide the button for another switch the player has already reached.
    public void Hide(UnityAction onPressed)
    {
        if (_onPressed != onPressed)
        {
            return;
        }

        _onPressed = null;
        _mirroredSprite = null;
        gameObject.SetActive(false);
    }

    // After the Animator has updated the world sprite, so the flip animation shows on the button too.
    private void LateUpdate()
    {
        MirrorSprite();
    }

    private void MirrorSprite()
    {
        if (iconImage != null && _mirroredSprite != null && iconImage.sprite != _mirroredSprite.sprite)
        {
            iconImage.sprite = _mirroredSprite.sprite;
        }
    }

    private void HandleClick()
    {
        _onPressed?.Invoke();
    }

    private IEnumerator PopIn()
    {
        Transform buttonTransform = transform;
        float elapsed = 0f;
        while (elapsed < popDurationSeconds)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / popDurationSeconds);
            buttonTransform.localScale = Vector3.one * Mathf.Lerp(popStartScale, 1f, 1f - (1f - t) * (1f - t));
            yield return null;
        }

        buttonTransform.localScale = Vector3.one;
    }
}
