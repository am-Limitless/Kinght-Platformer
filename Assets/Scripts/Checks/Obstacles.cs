using System.Collections;
using UnityEngine;

public class Obstacles : MonoBehaviour
{
    private Vector2 startPos;
    private Transform _cachedTransform;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator _animator;

    [SerializeField] private AudioSource _gameOverSoundSource;
    [SerializeField] private AudioClip _gameOverSound;

    private WaitForSeconds _respawnDelay;

    private PlayerLives playerLives;

    private void Start()
    {
        startPos = transform.position;
        _cachedTransform = transform;
        _respawnDelay = new WaitForSeconds(0.8f);

        playerLives = FindObjectOfType<PlayerLives>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Obstacle")) // Avoid unnecessary tag comparisons
        {
            if (_animator.GetInteger("playerState") != 3) // Check if state is already set
            {
                _animator.SetInteger("playerState", 3); // Only set state if not already set
            }

            playerLives.LoseLife();

            //if (playerLives. > 0)
            //{
            //    StartCoroutine(Respawn());
            //}

            PlayGameOverSound();
        }

        if (collision.gameObject.CompareTag("Heart"))
        {
            playerLives.GainLife();
        }
    }

    IEnumerator Respawn()
    {
        rb.simulated = false;
        rb.velocity = Vector2.zero;
        transform.localScale = Vector2.zero;
        yield return _respawnDelay;
        transform.position = startPos;
        transform.localScale = Vector2.one;
        rb.simulated = true;
    }

    private void PlayGameOverSound()
    {
        if (!_gameOverSoundSource.isPlaying)
        {
            _gameOverSoundSource.PlayOneShot(_gameOverSound);
        }
    }
}
