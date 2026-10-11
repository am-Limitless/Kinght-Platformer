using UnityEngine;

public class DoorController : MonoBehaviour
{
    public bool isOpen = false; // Tracks if the door is open
    public Vector3 openPosition; // The position of the door when open
    public Vector3 closedPosition; // The position of the door when closed
    private Vector3 targetPosition; // The target position of the door

    private void Start()
    {
        // Initialize the door's position
        closedPosition = transform.position;
        openPosition = new Vector3(transform.position.x, transform.position.y + 2, transform.position.z);
        targetPosition = closedPosition;
    }

    private void Update()
    {
        // Smoothly move the door to the target position
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * 2);
    }

    // Method to open the door
    public void OpenDoor()
    {
        isOpen = true;
        targetPosition = openPosition;
    }

    // Method to close the door
    public void CloseDoor()
    {
        isOpen = false;
        targetPosition = closedPosition;
    }
}
