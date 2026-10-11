using UnityEngine;

public class SwitchController : MonoBehaviour
{
    public DoorController door;
    private bool playerInRange = false;
    public AudioSource doorSoundSource;
    public Animator animator;

    private InteractButton interactButton;
    private SpriteRenderer switchSprite;

    private void Start()
    {
        // Looked up once; the button lives in the shared Touch Controls prefab and may start inactive.
        interactButton = FindFirstObjectByType<InteractButton>(FindObjectsInactive.Include);
        switchSprite = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the player enters the switch's trigger area
        if (collision.CompareTag("Player"))
        {
            playerInRange = true;

            if (interactButton != null)
            {
                interactButton.Show(ToggleSwitch, switchSprite);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // Check if the player leaves the switch's trigger area
        if (collision.CompareTag("Player"))
        {
            playerInRange = false;
            HideInteractButton();
        }
    }

    private void OnDisable()
    {
        HideInteractButton();
    }

    private void Update()
    {
        // Check if the player is in range and presses the interact key (e.g., 'E')
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            ToggleSwitch();
        }
    }

    private void HideInteractButton()
    {
        if (interactButton != null)
        {
            interactButton.Hide(ToggleSwitch);
        }
    }

    // Shared by the keyboard key and the on-screen button.
    private void ToggleSwitch()
    {
        if (door.isOpen)
        {
            door.CloseDoor();
            doorSoundSource.Play();
            animator.SetTrigger("switch OFF");
        }
        else
        {
            door.OpenDoor();
            doorSoundSource.Play();
            animator.SetTrigger("switch ON");
        }
    }
}
