using Unity.Cinemachine;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerLifeManager : MonoBehaviour
{
    public const string CurrentLivesKey = "CurrentLives";

    // Static so HUD elements in any scene can listen without looking up the persistent player.
    public static event System.Action LifeGained;

    private const int AnimationStateDeath = 3;
    private const int AnimationStateIdle = 0;

    [Tooltip("How long the death sprite stays on screen before the player goes back to the start (or Game Over).")]
    [SerializeField, Min(0f)] private float deathDisplaySeconds = 1f;

    public int initialLives = 3;
    public static int currentLives;
    public TextMeshProUGUI livesText;

    private Rigidbody2D playerRb;
    //[SerializeField] private GameObject player;

    private Vector2 startPos;
    private static PlayerLifeManager Instance;
    private WaitForSeconds _deathDisplayDelay;
    private Animator _animator;
    private bool _isShowingDeath;

    private float _collisionCooldown = 0.5f;
    private bool _isOnCooldown = false;

    public LevelLoader levelLoader;

    private Move move;

    public Transform startPosition;

    private CinemachineCamera _cinemachineCamera;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        InitializePlayer();
    }

    private void InitializePlayer()
    {
        move = GetComponent<Move>();

        playerRb = GetComponent<Rigidbody2D>();
        //startPos = transform.position;
        _deathDisplayDelay = new WaitForSeconds(deathDisplaySeconds);
        _animator = GetComponent<Animator>();

        // Initialize lives correctly
        if (SceneManager.GetActiveScene().buildIndex == 1) // First level
        {
            currentLives = initialLives;
        }
        else
        {
            // Load current lives for subsequent levels
            currentLives = PlayerPrefs.GetInt(CurrentLivesKey, initialLives);
        }

        if (livesText == null)
        {
            livesText = GameObject.Find("LivesText").GetComponent<TextMeshProUGUI>();
        }
        if (levelLoader == null)
        {
            levelLoader = FindFirstObjectByType<LevelLoader>();
        }

        UpdateLivesUI();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public void SetStartPos(Vector2 newPos)
    {
        startPos = newPos;
        RespawnPlayerAtStartPosition();
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MainMenu")
        {
            Destroy(gameObject); // Remove the player object when going to the main menu
            return;
        }

        _isShowingDeath = false;
        SetStartPosition();

        if (livesText == null)
        {
            livesText = GameObject.Find("LivesText").GetComponent<TextMeshProUGUI>();
        }
        UpdateLivesUI();

        RespawnPlayerAtStartPosition();

        FindAndAssignCinemachineCamera();

        move.EnableMovement();

    }

    private void FindAndAssignCinemachineCamera()
    {
        // Find the CinemachineVirtualCamera in the scene
        _cinemachineCamera = FindFirstObjectByType<CinemachineCamera>();
        if (_cinemachineCamera != null)
        {
            // Set the player's transform as the follow target
            _cinemachineCamera.Follow = transform;
        }
        else
        {
            Debug.LogWarning("Cinemachine Camera not found in the scene.");
        }
    }


    private void SetStartPosition()
    {
        // Find and set the start position from the "StartPosition" GameObject in the scene
        GameObject startObject = GameObject.Find("StartPosition");
        if (startObject != null)
        {
            startPosition = startObject.transform;
            startPos = startPosition.position;  // Update the start position
        }
        else
        {
            Debug.LogWarning("StartPosition GameObject not found. Using the current position.");
            startPos = transform.position;  // Fall back to current position
        }
    }

    public void LoseLife()
    {
        if (_isOnCooldown)
        {
            return;
        }

        currentLives--;
        UpdateLivesUI();
        _isOnCooldown = true;

        PlayerPrefs.SetInt(CurrentLivesKey, currentLives);

        if (currentLives <= 0)
        {
            StartCoroutine(GameOverAfterDeath());
        }
        else
        {
            StartCoroutine(Respawn());
        }

        StartCoroutine(CooldownCoroutine());
    }

    public void GainLife()
    {
        if (_isOnCooldown)
        {
            return;
        }
        currentLives++;
        UpdateLivesUI();
        _isOnCooldown = true;

        PlayerPrefs.SetInt(CurrentLivesKey, currentLives);
        LifeGained?.Invoke();

        StartCoroutine(CooldownCoroutine());
    }

    private IEnumerator CooldownCoroutine()
    {
        yield return new WaitForSeconds(_collisionCooldown);
        _isOnCooldown = false;
    }

    private void UpdateLivesUI()
    {
        if (livesText != null)
        {
            livesText.text = "X" + currentLives;
        }
    }

    private void GameOver()
    {
        if (levelLoader != null)
        {
            levelLoader.LoadLevelString("GameOver"); // Use LevelLoader to display loading UI
        }
        else
        {
            SceneManager.LoadScene("GameOver");
        }
    }

    // Freezes the player where it died so the death sprite is visible, and stops input and physics.
    private void FreezeForDeath()
    {
        move.DisableMovement();
        playerRb.linearVelocity = Vector2.zero;
        playerRb.simulated = false;
        _animator.SetInteger("playerState", AnimationStateDeath); // DisableMovement resets this to idle
    }

    private IEnumerator GameOverAfterDeath()
    {
        _isShowingDeath = true;
        FreezeForDeath();
        yield return _deathDisplayDelay;
        GameOver();
    }

    public IEnumerator Respawn()
    {
        if (_isShowingDeath)
        {
            yield break;
        }

        _isShowingDeath = true;
        FreezeForDeath();

        yield return _deathDisplayDelay;

        SetStartPosition();

        RespawnPlayerAtStartPosition();

        _animator.SetInteger("playerState", AnimationStateIdle);
        move.EnableMovement();
        _isShowingDeath = false;
    }



    public void RespawnPlayerAtStartPosition()
    {
        transform.position = startPos;
        playerRb.simulated = true;
        transform.localScale = Vector2.one;
    }

    public void TransferLivesToNextScene()
    {
        PlayerPrefs.SetInt(CurrentLivesKey, currentLives);

        if (levelLoader != null)
        {
            // Use LevelLoader to transition to the next scene using the scene index
            levelLoader.LoadLevelInt(SceneManager.GetActiveScene().buildIndex + 1);
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }

        UpdateLivesUI();
    }

    public void ResetPlayerLives()
    {
        currentLives = initialLives;
        PlayerPrefs.SetInt(CurrentLivesKey, currentLives);
        UpdateLivesUI();
    }

}
