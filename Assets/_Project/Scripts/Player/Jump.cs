using UnityEngine;


public class Jump : MonoBehaviour
{
    [Header("Jump Settings")]
    [SerializeField] private InputController _input = null;
    [SerializeField, Range(0f, 10f)] private float _jumpHeight = 3f;
    [SerializeField, Range(0, 5)] private int _maxAirJumps = 1;
    [Tooltip("An air jump waits until the player is rising slower than this share of the jump speed, so it is never wasted right after take-off.")]
    [SerializeField, Range(0.1f, 1f)] private float _airJumpMaxRiseRatio = 0.7f;
    private int _jumpsLeft;

    [Header("Gravity Settings")]
    [Tooltip("Gravity while falling. Higher = heavier fall.")]
    [SerializeField, Range(0f, 5f)] private float _downwardMovementMultiplier = 3f;
    [Tooltip("Gravity while rising with the jump button released. Higher = smaller hop on a quick tap. Holding the button keeps the full jump height.")]
    [SerializeField, Range(0f, 5f)] private float _upwardsMovementMultiplier = 1f;
    [Tooltip("Fastest the player can fall, in units per second.")]
    [SerializeField, Min(1f)] private float _maxFallSpeed = 24f;

    [Header("Timing Settings")]
    [SerializeField, Range(0f, 0.3f)] private float _coyoteTime = 0.2f;
    [SerializeField, Range(0f, 0.3f)] private float _jumpBufferTime = 0.2f;

    [Header("Audio Settings")]
    [SerializeField] private AudioClip _jumpSound;
    [SerializeField] private AudioSource _jumpAudioSource;

    private Rigidbody2D _body;
    private Ground _ground;
    private Vector2 _velocity;

    // A body that is still moving up this fast has only just left the ground, so it does not count as landed.
    private const float LANDED_MAX_UPWARD_SPEED = 0.5f;

    private float _defaultGravityScale;
    private float _coyoteCounter;
    private float _jumpBufferCounter;

    private bool _desiredJump;
    private bool _onGround;
#pragma warning disable CS0414 // Suppress the warning for unused private fields
    private bool _isJumping;
#pragma warning restore CS0414


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

        _velocity = _body.linearVelocity;

        UpdateJumpAvailability();

        HandleJumpInput();

        AdjustGravityScale();

        _velocity.y = Mathf.Max(_velocity.y, -_maxFallSpeed);

        _body.linearVelocity = _velocity;
    }

    private void UpdateJumpAvailability()
    {
        bool hasLanded = _onGround && _velocity.y <= LANDED_MAX_UPWARD_SPEED;
        if (hasLanded)
        {
            ResetJumpState();
            return;
        }

        _coyoteCounter -= Time.fixedDeltaTime;
        if (_coyoteCounter <= 0f)
        {
            // Once the ground jump is gone only air jumps remain, however the player left the ground.
            _jumpsLeft = Mathf.Min(_jumpsLeft, _maxAirJumps);
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
            _jumpBufferCounter -= Time.fixedDeltaTime;
        }

        if (_jumpBufferCounter > 0 && CanJumpNow())
        {
            ExecuteJump();
        }
    }

    private bool CanJumpNow()
    {
        if (_jumpsLeft <= 0)
        {
            return false;
        }

        bool isGroundJump = _jumpsLeft > _maxAirJumps;
        if (isGroundJump)
        {
            return true;
        }

        // Pressing again right after take-off would add almost no height, so wait (the press stays buffered) instead of wasting the air jump.
        return _velocity.y <= CalculateJumpSpeed() * _airJumpMaxRiseRatio;
    }

    private void ExecuteJump()
    {
        _jumpsLeft--;
        _jumpBufferCounter = 0f;
        _coyoteCounter = 0f;

        _velocity.y = CalculateJumpSpeed();

        _isJumping = true;
        PlayJumpSound();
    }

    private float CalculateJumpSpeed()
    {
        return Mathf.Sqrt(-2f * Physics2D.gravity.y * _jumpHeight);
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

        // Uses the speed after this step's jump, otherwise a jump's first step can get fall gravity and lose height.
        // Releasing the button while rising cuts the jump short (variable jump height).
        if (!_input.RetriveJumpHoldInput() && _velocity.y > 0)
        {
            _body.gravityScale = _upwardsMovementMultiplier;
        }
        else if (_velocity.y < 0)
        {
            _body.gravityScale = _downwardMovementMultiplier;
        }
        else
        {
            _body.gravityScale = _defaultGravityScale;
        }
    }

    private void ResetJumpState()
    {
        _jumpsLeft = _maxAirJumps + 1;
        _coyoteCounter = _coyoteTime;
        _isJumping = false;
    }
}