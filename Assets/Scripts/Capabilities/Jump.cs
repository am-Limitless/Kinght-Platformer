using UnityEngine;


public class Jump : MonoBehaviour
{
    [Header("Jump Settings")]
    [SerializeField] private InputController _input = null;
    [SerializeField, Range(0f, 10f)] private float _jumpHeight = 3f;
    [SerializeField, Range(0, 5)] private int _maxAirJumps = 1;
    [SerializeField] private int _jumpsLeft;

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
        if (_desiredJump)
        {
            Debug.Log("Jump button pressed.");
        }
    }

    private void FixedUpdate()
    {
        _onGround = _ground.GetOnGround();

        _velocity = _body.velocity;

        if (_onGround && _body.velocity.y == 0)
        {
            ResetJumpState();
        }
        else
        {
            _coyoteCounter -= Time.deltaTime;
        }

        if (_desiredJump)
        {
            _desiredJump = false;
            _jumpBufferCounter += _jumpBufferTime;
        }
        else if (!_desiredJump && _jumpBufferCounter > 0)
        {
            _jumpBufferCounter -= Time.deltaTime;
        }

        if (_jumpBufferCounter > 0 && (_coyoteCounter > 0f || _jumpPhase < _maxAirJumps))
        {
            JumpAction();
        }

        AdjustGravityScale();

        _body.velocity = _velocity;
    }

    private void JumpAction()
    {
        if (_coyoteCounter > 0f || _jumpPhase < _maxAirJumps)
        {
            _jumpPhase++;
            _jumpsLeft--;
            _jumpBufferCounter = 0;
            _coyoteCounter = 0;
            _jumpSpeed = Mathf.Sqrt(-2f * Physics2D.gravity.y * _jumpHeight);
            _isJumping = true;
            _velocity.y = _jumpSpeed;
            PlayJumpSound();
        }
    }

    private void PlayJumpSound()
    {
        _jumpAudioSource.PlayOneShot(_jumpSound);
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
        _jumpsLeft = _maxAirJumps + 1; // Reset total jumps (initial jump + air jumps)
        _jumpPhase = 0; // Reset jump phase
        _coyoteCounter = _coyoteTime; // Reset coyote time
        _isJumping = false; // Not in jump state
        Debug.Log("Grounded! Jump state reset. Jumps Left: " + _jumpsLeft);
    }
}