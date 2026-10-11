using UnityEngine;

// Runs after the camera has moved this frame, so the background never lags a frame behind it.
[DefaultExecutionOrder(1000)]
public class BackgroundController : MonoBehaviour
{
    [Header("Camera")]
    public GameObject cam;

    [Header("Parallax")]
    [Tooltip("0 = moves with the world, 1 = stays fixed on screen (infinitely far away).")]
    public float parallaxEffect;
    [Tooltip("Scales the vertical parallax. 1 = same depth feel as horizontal, 0 = no vertical movement.")]
    [SerializeField, Range(0f, 1f)] private float verticalParallaxMultiplier = 1f;

    private float _startPos;
    private float _length;
    private float _startY;
    private float _referenceCameraY;
    private float _pivotOffsetY;
    private float _spriteHeight;
    private bool _verticalReferenceCaptured;

    private Transform _cameraTransform;
    private Camera _camera;
    private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        _startPos = transform.position.x;
        _length = _spriteRenderer.bounds.size.x;

        _startY = transform.position.y;
        _spriteHeight = _spriteRenderer.bounds.size.y;
        _pivotOffsetY = _spriteRenderer.bounds.center.y - transform.position.y;

        if (cam != null)
        {
            _cameraTransform = cam.transform;
            _camera = cam.GetComponent<Camera>();
        }
    }

    private void LateUpdate()
    {
        if (_cameraTransform == null)
        {
            return;
        }

        Vector3 cameraPosition = _cameraTransform.position;

        // Use the camera's position on the first frame as "no vertical offset", so the level looks as authored at the start.
        if (!_verticalReferenceCaptured)
        {
            _referenceCameraY = cameraPosition.y;
            _verticalReferenceCaptured = true;
        }

        float distance = cameraPosition.x * parallaxEffect;
        float movement = cameraPosition.x * (1 - parallaxEffect);

        Vector3 newPosition = transform.position;
        newPosition.x = _startPos + distance;
        newPosition.y = GetParallaxY(cameraPosition.y);
        transform.position = newPosition;

        if (movement > _startPos + _length)
        {
            _startPos += _length;
        }
        else if (movement < _startPos - _length)
        {
            _startPos -= _length;
        }
    }

    private float GetParallaxY(float cameraY)
    {
        float verticalDistance = (cameraY - _referenceCameraY) * parallaxEffect * verticalParallaxMultiplier;
        float spriteCenterY = _startY + _pivotOffsetY + verticalDistance;

        return ClampToCoverCamera(spriteCenterY, cameraY) - _pivotOffsetY;
    }

    // Keeps the sprite covering the whole camera view, so no gap shows above or below it however high the player goes.
    private float ClampToCoverCamera(float spriteCenterY, float cameraY)
    {
        if (_camera == null || !_camera.orthographic)
        {
            return spriteCenterY;
        }

        float halfSpriteHeight = _spriteHeight * 0.5f;
        float lowestCenter = cameraY + _camera.orthographicSize - halfSpriteHeight;
        float highestCenter = cameraY - _camera.orthographicSize + halfSpriteHeight;

        // A sprite shorter than the screen cannot cover it, so leave it alone.
        if (lowestCenter > highestCenter)
        {
            return spriteCenterY;
        }

        return Mathf.Clamp(spriteCenterY, lowestCenter, highestCenter);
    }
}
