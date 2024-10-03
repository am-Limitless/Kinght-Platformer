using UnityEngine;

public class WinTrigger : MonoBehaviour
{
    public GameObject instuct;

    public bool winTrigger = false;

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            instuct.SetActive(true);
            winTrigger = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            instuct.SetActive(false);
        }
    }
}
