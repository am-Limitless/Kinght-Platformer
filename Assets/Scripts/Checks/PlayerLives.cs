using System.Collections;
using TMPro; // Using TextMeshPro for UI text
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerLives : MonoBehaviour
{
    public int initialLives = 3;
    public static int currentLives;
    public TextMeshProUGUI livesText;
    public AudioSource gameOverSoundSource;
    public AudioClip gameOverSound;

    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator _animator;

    private Vector2 startPos;
    private static PlayerLives instances;
    private WaitForSeconds _respawnDelay;

    private float _collisionCooldown = 0.5f;
    private bool _isOnCooldown = false;

    private void Awake()
    {
        // Singleton pattern to ensure only one instance of PlayerLives exists
        if (instances == null)
        {
            instances = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        startPos = transform.position;
        _respawnDelay = new WaitForSeconds(0.8f);

        // Set initial lives if starting at the first level
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            currentLives = initialLives;
        }

        // Update UI with initial life count
        UpdateLivesUI();

        // Subscribe to scene loaded event to update UI when a new scene is loaded
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        // Unsubscribe from the scene loaded event to prevent memory leaks
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Update lives UI when a new scene is loaded
        UpdateLivesUI();
    }

    // Method to decrease lives when the player hits an obstacle
    public void LoseLife()
    {
        if (_isOnCooldown)
        {
            return;
        }

        currentLives--;
        UpdateLivesUI();
        _isOnCooldown = true;

        if (currentLives <= 0)
        {
            GameOver();
        }
        else
        {
            // Respawn player at the start position
            StartCoroutine(Respawn());
        }

        StartCoroutine(CooldownCoroutine());
    }

    // Method to increase lives when collecting a heart
    public void GainLife()
    {
        if (_isOnCooldown)
        {
            return;
        }
        currentLives++;
        UpdateLivesUI();
        _isOnCooldown = true;

        StartCoroutine(CooldownCoroutine());
    }

    private IEnumerator CooldownCoroutine()
    {
        yield return new WaitForSeconds(_collisionCooldown);
        _isOnCooldown = false;
    }


    // Update the UI text displaying lives
    private void UpdateLivesUI()
    {
        if (livesText != null)
        {
            livesText.text = "X " + currentLives;
        }
    }

    // Handle game over state (restart the game or load the GameOver scene)
    private void GameOver()
    {
        Debug.Log("Game Over!");
        SceneManager.LoadScene("GameOver");
    }

    // Play the game over sound when player loses all lives
    private void PlayGameOverSound()
    {
        if (!gameOverSoundSource.isPlaying)
        {
            gameOverSoundSource.PlayOneShot(gameOverSound);
        }
    }

    // Respawn the player at the initial position after a delay
    IEnumerator Respawn()
    {
        // Disable player physics and scale down
        rb.simulated = false;
        rb.velocity = Vector2.zero;
        transform.localScale = Vector2.zero;

        // Wait for the respawn delay
        yield return _respawnDelay;

        // Reset player position, scale, and re-enable physics
        transform.position = startPos;
        transform.localScale = Vector2.one;
        rb.simulated = true;
    }

    // Detect collisions with obstacles and hearts
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if player collided with an obstacle
        if (collision.CompareTag("Obstacle"))
        {
            if (_animator.GetInteger("playerState") != 3) // Check if state is already set
            {
                _animator.SetInteger("playerState", 3); // Only set state if not already set
            }

            // Lose a life and play sound
            LoseLife();
            PlayGameOverSound();
        }

        // Check if player collided with a heart
        if (collision.CompareTag("Heart"))
        {
            GainLife();
            Destroy(collision.gameObject); // Remove the heart from the scene
        }
    }
}
