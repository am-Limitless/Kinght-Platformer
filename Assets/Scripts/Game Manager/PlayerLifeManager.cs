using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerLifeManager : MonoBehaviour
{
    public int initialLives = 3;
    public static int currentLives;
    public TextMeshProUGUI livesText;

    private Rigidbody2D playerRb;
    //[SerializeField] private GameObject player;

    private Vector2 startPos;
    private static PlayerLifeManager Instance;
    private WaitForSeconds _respawnDelay;

    private float _collisionCooldown = 0.5f;
    private bool _isOnCooldown = false;

    public LevelLoader levelLoader;
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

        playerRb = GetComponent<Rigidbody2D>();
        startPos = transform.position;

        _respawnDelay = new WaitForSeconds(0.8f);

        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            currentLives = initialLives;
        }
        else
        {
            currentLives = PlayerPrefs.GetInt("CurrentLives", initialLives);
        }

        if (livesText == null)
        {
            livesText = GameObject.Find("LivesText").GetComponent<TextMeshProUGUI>();
        }
        if (levelLoader == null)
        {
            levelLoader = FindObjectOfType<LevelLoader>(); // Find the LevelLoader script in the scene
        }


        UpdateLivesUI();

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public void SetStartPos(Vector2 startPos)
    {
        this.startPos = startPos;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {

        if (livesText == null)
        {
            livesText = GameObject.Find("LivesText").GetComponent<TextMeshProUGUI>();
        }
        UpdateLivesUI();
        playerRb = GetComponent<Rigidbody2D>();
        startPos = transform.position;

        RespawnPlayerAtStartPosition();

        StartCoroutine(Respawn());

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

        PlayerPrefs.SetInt("CurrentLives", currentLives);

        if (currentLives <= 0)
        {
            GameOver();
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

        PlayerPrefs.SetInt("CurrentLives", currentLives);

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

    public IEnumerator Respawn()
    {
        Debug.Log("Respawn coroutine started");
        playerRb.velocity = Vector2.zero;
        transform.position = startPos;
        transform.localScale = Vector2.zero;

        yield return _respawnDelay;

        RespawnPlayerAtStartPosition();
    }

    public void RespawnPlayerAtStartPosition()
    {
        transform.position = startPos;
        playerRb.simulated = true;
        transform.localScale = Vector2.one;
    }

    public void TransferLivesToNextScene()
    {
        PlayerPrefs.SetInt("CurrentLives", currentLives);

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

}
