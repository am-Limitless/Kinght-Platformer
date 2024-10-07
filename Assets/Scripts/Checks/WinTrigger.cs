using UnityEngine;

public class WinTrigger : MonoBehaviour
{
    public GameObject instruct;

    public bool winTrigger = false;

    public AudioSource WinMusicSource;
    public AudioClip WinMusic;
    private bool hasWon = false;

    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !hasWon)
        {
            if (instruct != null)
            {
                instruct.SetActive(true);
            }
            winTrigger = true;
            animator.SetTrigger("playerToched");
            WinMusicSource.PlayOneShot(WinMusic);
            hasWon = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (instruct != null)
            {
                instruct.SetActive(false);
            }
        }
    }

    private void OnDestroy()
    {
        instruct = null;   // Prevent accessing instruct after it is destroyed
    }

    private void OnDisable()
    {
        instruct = null;  // Prevent accessing instruct after this script is disabled
    }
}
