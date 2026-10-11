using UnityEngine;

public class WorldTrigger : MonoBehaviour
{
    public GameObject instruct;

    [Tooltip("Optional. Flashes the touch jump zone when the player enters this trigger.")]
    [SerializeField] private JumpZoneFlash jumpZoneFlash;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            instruct.SetActive(true);

            if (jumpZoneFlash != null && jumpZoneFlash.isActiveAndEnabled)
            {
                jumpZoneFlash.Flash();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            instruct.SetActive(false);
        }
    }
}
