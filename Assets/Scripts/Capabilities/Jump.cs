using UnityEngine;


public class Jump : MonoBehaviour
{
    [Header("Jump Settings")]
    [SerializeField] private InputController _input = null;
    [SerializeField, Range(0f, 10f)] private float _jumpHeight = 3f;
    [SerializeField, Range(0, 5)] private int _maxAirJumps = 1;
    private int _jumpsLeft;

    [Header("Gravity Settings")]
    [SerializeField, Range(0f, 5f)] private float _downwardMovementMultiplier = 3f;
    [SerializeField, Range(0f, 5f)] private float _upwardsMovementMultiplier = 1f;

    [Header("Timing Settings")]
    [SerializeField, Range(0f, 0.3f)] private float _coyoteTime = 0.2f;
    [SerializeField, Range(0f, 0.3f)] private float _jumpBufferTime = 0.2f;

    [Header("Audio Settings")]
    [SerializeField] private AudioClip _jumpSound;
    [SerializeField] private AudioSource _jumpAudioSource;

    private Rigidbody2D _body;
    private Ground _ground;
    private Vector2 _velocity;

    private int _jumpPhase;
    private float _defaultGravityScale;
    private float _jumpSpeed;
    private float _coyoteCounter;
    private float _jumpBufferCounter;

    private bool _desiredJump;
    private bool _onGround;
    private bool _isJumping;

    private void Start()
    {
        ResetJumpState();
    }

    private void Awake()
    {
        _body = GetComponent<Rigidbody2D>();
        _ground = GetComponent<Ground>();
        _defaultGravityScale = _body.gravityScale;
        _jumpsLeft = _maxAirJumps + 1;
    }

    private void Update()
    {
        _desiredJump |= _input.RetriveJumpInput();
    }

    private void FixedUpdate()
    {
        _onGround = _ground.GetOnGround();

        _velocity = _body.velocity;

        ResetOnGround();

        HandleJumpInput();

        AdjustGravityScale();

        _body.velocity = _velocity;
    }

    private void ResetOnGround()
    {
        if (_onGround)
        {
            ResetJumpState();
        }
        else
        {
            _coyoteCounter -= Time.deltaTime;
        }
    }

    private void HandleJumpInput()
    {
        // Manage jump buffering and coyote time for jump actions
        if (_desiredJump)
        {
            _desiredJump = false;
            _jumpBufferCounter = _jumpBufferTime;
        }
        else if (_jumpBufferCounter > 0)
        {
            _jumpBufferCounter -= Time.deltaTime;
        }

        // Execute jump if conditions are met
        if (_jumpBufferCounter > 0 && (_coyoteCounter > 0f || _jumpPhase < _maxAirJumps))
        {
            ExecuteJump();
        }
    }

    private void ExecuteJump()
    {
        if (_jumpsLeft < 0)
        {
            return;
        }
        else
        {
            _jumpPhase++;
            _jumpsLeft--;
            _jumpBufferCounter = 0;
            _coyoteCounter = 0;

            _jumpSpeed = Mathf.Sqrt(-2f * Physics2D.gravity.y * _jumpHeight);
            _velocity.y = _jumpSpeed;

            _isJumping = true;
            PlayJumpSound();
        }

    }

    private void PlayJumpSound()
    {
        if (_jumpSound != null && _jumpAudioSource != null)
        {
            _jumpAudioSource.PlayOneShot(_jumpSound);
        }
    }


    private void AdjustGravityScale()
    {

        // Adjust gravity scale based on the player's vertical velocity and jump hold input
        if (!_input.RetriveJumpHoldInput() && _body.velocity.y > 0)
        {
            _body.gravityScale = _upwardsMovementMultiplier; // Apply upwards multiplier when jump is held
        }
        else if (_body.velocity.y < 0)
        {
            _body.gravityScale = _downwardMovementMultiplier; // Reset to default gravity scale when not jumping or falling
        }
        else
        {
            _body.gravityScale = _defaultGravityScale;
        }
    }

    private void ResetJumpState()
    {
        _jumpsLeft = _maxAirJumps + 1;
        _jumpPhase = 0;
        _coyoteCounter = _coyoteTime;
        _isJumping = false;
    }
}