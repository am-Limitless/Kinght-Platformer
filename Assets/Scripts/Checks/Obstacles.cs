using System.Collections;
using UnityEngine;

public class Obstacles : MonoBehaviour
{
    private Vector2 startPos;
    private Rigidbody2D rb;

    [SerializeField] private AudioClip _gameOverSound;
    [SerializeField] private AudioSource _gameOverSoundSource;

    private void Start()
    {
        startPos = transform.position;
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Obstacle"))
        {
            Die();
            PlayGameOverSound();
        }
    }

    private void Die()
    {
        StartCoroutine(Respawn(0.5f));
    }

    IEnumerator Respawn(float duration)
    {
        rb.simulated = false;
        rb.velocity = new Vector2(0, 0);
        transform.localScale = new Vector2(0, 0);
        yield return new WaitForSeconds(duration);
        transform.position = startPos;
        transform.localScale = new Vector2(1, 1);
        rb.simulated = true;
    }

    private void PlayGameOverSound()
    {
        _gameOverSoundSource.PlayOneShot(_gameOverSound);
    }

}
