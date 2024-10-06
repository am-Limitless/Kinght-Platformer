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
    [SerializeField] private GameObject player;

    private Vector2 StartPos;
    public static PlayerLifeManager Instance { get; private set; }
    private WaitForSeconds _respawnDelay;

    private float _collisionCooldown = 0.5f;
    private bool _isOnCooldown = false;

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
        playerRb = player.GetComponent<Rigidbody2D>();
        StartPos = player.transform.position;

        _respawnDelay = new WaitForSeconds(0.8f);

        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            currentLives = initialLives;
        }
        else
        {
            currentLives = PlayerPrefs.GetInt("CurrentLives", initialLives);
        }

        UpdateLivesUI();

        if (livesText == null)
        {
            livesText = GameObject.Find("LivesText").GetComponent<TextMeshProUGUI>();
        }

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public void SetStartPos(Vector2 startPos)
    {
        this.StartPos = startPos;
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

        player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            playerRb = player.GetComponent<Rigidbody2D>();
            StartPos = player.transform.position;

            RespawnPlayerAtStartPosition();

            StartCoroutine(Respawn());
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
        Debug.Log("Game Over!");
        SceneManager.LoadScene("GameOver");
    }

    public IEnumerator Respawn()
    {
        Debug.Log("Respawn coroutine started");
        playerRb.velocity = Vector2.zero;
        player.transform.position = StartPos;
        player.transform.localScale = Vector2.zero;

        yield return _respawnDelay;

        RespawnPlayerAtStartPosition();
    }

    public void RespawnPlayerAtStartPosition()
    {
        if (player != null)
        {
            player.transform.position = StartPos;
            playerRb.simulated = true;
            player.transform.localScale = Vector2.one;

        }
    }

    public void TransferLivesToNextScene()
    {
        PlayerPrefs.SetInt("CurrentLives", currentLives);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        UpdateLivesUI();
    }

}
