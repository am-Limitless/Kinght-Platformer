using UnityEngine;

public class Move : MonoBehaviour
{
    [Header("Input Settings")]
    [SerializeField] private InputController _input = null;

    [Header("Movement Settings")]
    [SerializeField, Range(0f, 100f)] private float _maxSpeed = 4f;

    [Header("Ground Feel")]
    [Tooltip("How fast the player speeds up on the ground. Lower = heavier start.")]
    [SerializeField, Range(0f, 100f)] private float _maxAcceleation = 35f;
    [Tooltip("How fast the player slows down when no direction is held. Lower = longer slide.")]
    [SerializeField, Range(0f, 100f)] private float _groundDeceleration = 25f;
    [Tooltip("How fast the player brakes when pushing the opposite way (the skid).")]
    [SerializeField, Range(0f, 100f)] private float _groundTurnAcceleration = 45f;

    [Header("Air Feel")]
    [Tooltip("How fast the player speeds up in the air.")]
    [SerializeField, Range(0f, 100f)] private float _maxAirAcceleration = 20f;
    [Tooltip("How fast the player slows down in the air with no direction held. Low = keeps momentum like Mario.")]
    [SerializeField, Range(0f, 100f)] private float _airDeceleration = 6f;
    [Tooltip("How fast the player changes direction in the air.")]
    [SerializeField, Range(0f, 100f)] private float _airTurnAcceleration = 28f;

    [Header("Audio Settings")]
    [SerializeField] private AudioClip _walkSound;
    [SerializeField] private AudioSource _walkAudioSource;

    private Vector2 _direction;
    private Vector2 _desiredVelocity;
    private Vector2 _velocity;
    private Rigidbody2D _body;
    private Ground _ground;
    private Animator _animator;

    private bool _onGround;
    private bool _isPlayingWalkSound;
    private bool isMovementEnabled = true;

    private const int ANIMATION_STATE_IDLE = 0;
    private const int ANIMATION_STATE_RUNNING = 1;
    private const int ANIMATION_STATE_JUMPING = 2;
    private const float WALK_SOUND_MINIMUM_SPEED = 0.1f;

    private static readonly int PlayerStateParameter = Animator.StringToHash("playerState");

    private void Awake()
    {
        _body = GetComponent<Rigidbody2D>();
        _ground = GetComponent<Ground>();
        _animator = GetComponent<Animator>();
        _walkAudioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        EnableMovement();
    }

    private void Update()
    {
        if (isMovementEnabled)
        {
            _direction.x = _input.RetrieveMoveInput();

            _desiredVelocity = new Vector2(_direction.x, 0f) * _maxSpeed;

            FaceMoveDirection();
            PlayWalkSound();
        }
    }

    private void FixedUpdate()
    {
        if (isMovementEnabled)
        {
            _onGround = _ground.GetOnGround();
            _velocity = _body.linearVelocity;

            float speedChangeRate = SelectSpeedChangeRate();
            _velocity.x = Mathf.MoveTowards(_velocity.x, _desiredVelocity.x, speedChangeRate * Time.fixedDeltaTime);

            _body.linearVelocity = _velocity;

            UpdateAnimator();
        }
    }

    // Only touches the transform when the facing actually flips.
    private void FaceMoveDirection()
    {
        if (_direction.x == 0f)
        {
            return;
        }

        float facing = _direction.x > 0f ? 1f : -1f;
        Vector3 localScale = transform.localScale;
        if (localScale.x != facing)
        {
            localScale.x = facing;
            transform.localScale = localScale;
        }
    }

    private float SelectSpeedChangeRate()
    {
        const float minimumMovingSpeed = 0.01f;

        bool isHoldingDirection = Mathf.Abs(_desiredVelocity.x) > minimumMovingSpeed;
        bool isMoving = Mathf.Abs(_velocity.x) > minimumMovingSpeed;
        bool isReversing = isHoldingDirection && isMoving && Mathf.Sign(_desiredVelocity.x) != Mathf.Sign(_velocity.x);

        if (!isHoldingDirection)
        {
            return _onGround ? _groundDeceleration : _airDeceleration;
        }

        if (isReversing)
        {
            return _onGround ? _groundTurnAcceleration : _airTurnAcceleration;
        }

        return _onGround ? _maxAcceleation : _maxAirAcceleration;
    }

    public void EnableMovement()
    {
        isMovementEnabled = true;
    }

    public void DisableMovement()
    {
        isMovementEnabled = false;
        _body.linearVelocity = Vector2.zero;
        _animator.SetInteger(PlayerStateParameter, ANIMATION_STATE_IDLE);
        _walkAudioSource.Stop();
        _isPlayingWalkSound = false;
    }

    private void PlayWalkSound()
    {
        // Check if the player is moving and on the ground
        if (_onGround && Mathf.Abs(_velocity.x) > WALK_SOUND_MINIMUM_SPEED)
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

        // Read the live value: PlayerTrigger also sets this parameter (death), and we must override it after respawn.
        if (_animator.GetInteger(PlayerStateParameter) != animationState)
        {
            _animator.SetInteger(PlayerStateParameter, animationState);
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

