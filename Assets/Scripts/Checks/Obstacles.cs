using System.Collections;
using UnityEngine;

public class Obstacles : MonoBehaviour
{
    private Vector2 startPos;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator _animator;

    [SerializeField] private AudioSource _gameOverSoundSource;
    [SerializeField] private AudioClip _gameOverSound;

    private void Start()
    {
        startPos = transform.position;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Obstacle"))
        {
            _animator.SetInteger("playerState", 3);
            StartCoroutine(Respawn(0.8f));
            PlayGameOverSound();
        }
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
