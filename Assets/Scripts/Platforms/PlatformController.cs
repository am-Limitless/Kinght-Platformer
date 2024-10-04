using UnityEngine;

public class PlatformController : MonoBehaviour
{
    [Header("Platform Settings")]
    public Transform posA, posB;
    public int Speed;

    Vector2 targetPos;
    private float _threshold = 0.1f;

    private void Start()
    {
        targetPos = posB.position;
    }

    void Update()
    {
        float distanceToA = ((Vector2)transform.position - (Vector2)posA.position).sqrMagnitude;
        float distanceToB = ((Vector2)transform.position - (Vector2)posB.position).sqrMagnitude;

        if (distanceToA < _threshold * _threshold)
        {
            targetPos = posB.position;
        }

        if (distanceToB < _threshold * _threshold)
        {
            targetPos = posA.position;
        }

        transform.position = Vector2.MoveTowards(transform.position, targetPos, Speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.transform.parent = this.transform;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.transform.parent = null;
        }
    }

}
