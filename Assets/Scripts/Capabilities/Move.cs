using UnityEngine;

public class Move : MonoBehaviour
{
    [Header("Input Settings")]
    [SerializeField] private InputController _input = null;

    [Header("Movement Settings")]
    [SerializeField, Range(0f, 100f)] private float _maxSpeed = 4f;
    [SerializeField, Range(0f, 100f)] private float _maxAcceleation = 35f;
    [SerializeField, Range(0f, 100f)] private float _maxAirAcceleration = 20f;

    [Header("Audio Settings")]
    [SerializeField] private AudioClip _walkSound;
    [SerializeField] private AudioSource _walkAudioSource;

    private Vector2 _direction;
    private Vector2 _desiredVelocity;
    private Vector2 _velocity;
    private Rigidbody2D _body;
    private Ground _ground;
    private Animator _animator;

    private float _maxSpeedChange;
    private float _acceleration;
    private bool _onGround;
    private bool _isPlayingWalkSound;
    private bool isMovementEnabled = true;

    private const int ANIMATION_STATE_IDLE = 0;
    private const int ANIMATION_STATE_RUNNING = 1;
    private const int ANIMATION_STATE_JUMPING = 2;

    private void Awake()
    {
        _body = GetComponent<Rigidbody2D>();
        _ground = GetComponent<Ground>();
        _animator = GetComponent<Animator>();
        _walkAudioSource = GetComponent<AudioSource>();

        _maxSpeedChange = _maxAcceleation * Time.deltaTime;
    }

    private void Update()
    {
        if (isMovementEnabled)
        {
            _direction.x = _input.RetrieveMoveInput();

            _desiredVelocity = new Vector2(_direction.x, 0f) * _maxSpeed;

            Vector3 localScale = transform.localScale;

            // Flip character based on direction
            if (_direction.x > 0)
            {
                // Move right
                localScale.x = 1f; // Face right
            }
            else if (_direction.x < 0)
            {
                // Move left
                localScale.x = -1f; // Face left
            }

            transform.localScale = localScale;
            PlayWalkSound();
        }
    }

    private void FixedUpdate()
    {
        if (isMovementEnabled)
        {
            _onGround = _ground.GetOnGround();
            _velocity = _body.velocity;

            if (_onGround)
            {
                _acceleration = _maxAcceleation;
            }
            else
            {
                _acceleration = _maxAirAcceleration;
                _maxSpeedChange = _acceleration * Time.deltaTime;
            }

            _velocity.x = Mathf.MoveTowards(_velocity.x, _desiredVelocity.x, _maxSpeedChange);

            _body.velocity = _velocity;

            UpdateAnimator();
        }
    }

    public void EnableMovement()
    {
        isMovementEnabled = true;
    }

    public void DisableMovement()
    {
        isMovementEnabled = false;
        _body.velocity = Vector2.zero;
        _animator.SetInteger("playerState", ANIMATION_STATE_IDLE);
        _walkAudioSource.Stop();
    }

    private void PlayWalkSound()
    {
        // Check if the player is moving and on the ground
        if (_onGround && Mathf.Abs(_velocity.x) > 0.1f)
        {
            if (!_isPlayingWalkSound)
            {
                _walkAudioSource.clip = _walkSound;
                _walkAudioSource.loop = true;
                _walkAudioSource.Play();
                _isPlayingWalkSound = true;
            }
        }
        else
        {
            if (_isPlayingWalkSound)
            {
                _walkAudioSource.Stop();
                _isPlayingWalkSound = false;
            }
        }
    }



    private void UpdateAnimator()
    {
        int animationState = _onGround ? (_velocity.x != 0 ? ANIMATION_STATE_RUNNING : ANIMATION_STATE_IDLE) : ANIMATION_STATE_JUMPING;

        if (_animator.GetInteger("playerState") != animationState)
        {
            _animator.SetInteger("playerState", animationState);
        }

        if (animationState == ANIMATION_STATE_IDLE)
        {
            if (_isPlayingWalkSound)
            {
                _walkAudioSource.Stop();
                _isPlayingWalkSound = false;
            }
        }
    }
}

