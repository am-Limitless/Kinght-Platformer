using UnityEngine;

public class SwitchController : MonoBehaviour
{
    public DoorController door;
    public GameObject KeyInstruct;
    private bool playerInRange = false;
    public AudioSource doorSoundSource;
    public Animator animator;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the player enters the switch's trigger area
        if (collision.CompareTag("Player"))
        {
            playerInRange = true;
            KeyInstruct.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // Check if the player leaves the switch's trigger area
        if (collision.CompareTag("Player"))
        {
            playerInRange = false;
            KeyInstruct.SetActive(false);
        }
    }

    private void Update()
    {
        // Check if the player is in range and presses the interact key (e.g., 'E')
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            // Toggle the door's state
            if (door.isOpen)
            {
                door.CloseDoor();
            }
            else
            {
                door.OpenDoor();
                doorSoundSource.Play();
                animator.SetTrigger("switch ON");
            }
        }
    }
}
