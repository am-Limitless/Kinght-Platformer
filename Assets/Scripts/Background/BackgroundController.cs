using UnityEngine;

public class BackgroundController : MonoBehaviour
{
    private float _startPos;
    private float _length;
    private SpriteRenderer _spriteRenderer;

    public GameObject cam;
    public float parallaxEffect;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        _startPos = transform.position.x;
        _length = _spriteRenderer.bounds.size.x;
    }

    private void LateUpdate()
    {
        float cameraPositionX = cam.transform.position.x;

        float distance = cameraPositionX * parallaxEffect;
        float movement = cameraPositionX * (1 - parallaxEffect);

        //transform.position = new Vector3(startPos + distance, transform.position.y, transform.position.z);
        Vector3 newPosition = transform.position;
        newPosition.x = _startPos + distance;
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
}