using UnityEngine;

public class WorldTrigger : MonoBehaviour
{
    public GameObject instuct1;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            instuct1.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            instuct1.SetActive(false);
        }
    }
}
