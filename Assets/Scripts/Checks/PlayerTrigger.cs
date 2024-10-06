using UnityEngine;

public class PlayerTrigger : MonoBehaviour
{

    [SerializeField] private Animator _animator;
    [SerializeField] private PlayerLifeManager lifeManager;

    public bool playerDead = false;
    public AudioSource gameOverSoundSource;
    public AudioClip gameOverSound;

    private void Awake()
    {
        if (lifeManager == null)
        {
            lifeManager = FindAnyObjectByType<PlayerLifeManager>();
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Obstacle"))
        {
            if (_animator.GetInteger("playerState") != 3) // Check if state is already set
            {
                _animator.SetInteger("playerState", 3); // Only set state if not already set
            }
            playerDead = true;
            PlayGameOverSound();
            lifeManager.LoseLife();
            StartCoroutine(lifeManager.Respawn());
        }
        if (collision.CompareTag("Heart"))
        {
            Destroy(collision.gameObject); // Remove the heart from the scene
            lifeManager.GainLife();
        }
    }

    private void PlayGameOverSound()
    {
        if (!gameOverSoundSource.isPlaying)
        {
            gameOverSoundSource.PlayOneShot(gameOverSound);
        }
    }
}
