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

    private void Awake()
    {
        _body = GetComponent<Rigidbody2D>();
        _ground = GetComponent<Ground>();
        _animator = GetComponent<Animator>();
        _walkAudioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        _direction.x = _input.RetrieveMoveInput();
        _desiredVelocity = new Vector2(_direction.x, 0f) * Mathf.Max(_maxSpeed - _ground.GetFriction(), 0f);

        // Flip character based on direction
        if (_direction.x > 0)
        {
            // Move right
            transform.localScale = new Vector3(-1f, 1f, 1f); // Face right
        }
        else if (_direction.x < 0)
        {
            // Move left
            transform.localScale = new Vector3(1f, 1f, 1f); // Face left
        }
        PlayWalkSound();
    }

    private void FixedUpdate()
    {
        _onGround = _ground.GetOnGround();
        _velocity = _body.velocity;

        _acceleration = _onGround ? _maxAcceleation : _maxAirAcceleration;
        _maxSpeedChange = _acceleration * Time.deltaTime;
        _velocity.x = Mathf.MoveTowards(_velocity.x, _desiredVelocity.x, _maxSpeedChange);

        _body.velocity = _velocity;

        UpdateAnimator();
    }

    private void PlayWalkSound()
    {
        // Add debug logs to see if the method is being called and what the state is
        Debug.Log("PlayWalkSound called. OnGround: " + _onGround + ", Velocity: " + _velocity.x + ", IsPlaying: " + _walkAudioSource.isPlaying);

        if (_onGround && Mathf.Abs(_velocity.x) > 0.1f && !_walkAudioSource.isPlaying)
        {
            Debug.Log("Starting walk sound.");
            _walkAudioSource.clip = _walkSound;
            _walkAudioSource.loop = true;
            _walkAudioSource.Play();
        }
        else if ((_onGround && Mathf.Abs(_velocity.x) < 0.1f) || !_onGround)
        {
            if (_walkAudioSource.isPlaying)
            {
                Debug.Log("Stopping walk sound.");
                _walkAudioSource.Stop();
            }
        }
    }

    private void UpdateAnimator()
    {
        // Update the animation state based on player's ground status and velocity
        if (_onGround)
        {
            if (_velocity.x != 0)
            {
                _animator.SetInteger("playerState", 1); // Running state
            }
            else
            {
                _animator.SetInteger("playerState", 0); // Idle state
            }
        }
        else
        {
            // If in air, set jump animation
            _animator.SetInteger("playerState", 2); // Jump state
        }
    }
}

