using UnityEngine;

public class WorldTrigger : MonoBehaviour
{
    public GameObject instruct;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            instruct.SetActive(true);
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
