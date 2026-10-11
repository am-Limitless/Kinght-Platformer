using System.Collections.Generic;
using UnityEngine;

public class PlatformController : MonoBehaviour
{
    [Header("Platform Settings")]
    public Transform posA, posB;
    public int Speed;

    Vector2 targetPos;
    private float _threshold = 0.1f;
    private readonly List<Rigidbody2D> _riders = new List<Rigidbody2D>();
    // The player has several colliders, so a body stays a rider until its last collider leaves.
    private readonly Dictionary<Rigidbody2D, int> _riderContactCounts = new Dictionary<Rigidbody2D, int>();

    private void Start()
    {
        targetPos = posB.position;
    }

    private void FixedUpdate()
    {
        Vector2 previousPos = transform.position;

        float distanceToA = (previousPos - (Vector2)posA.position).sqrMagnitude;
        float distanceToB = (previousPos - (Vector2)posB.position).sqrMagnitude;

        if (distanceToA < _threshold * _threshold)
        {
            targetPos = posB.position;
        }

        if (distanceToB < _threshold * _threshold)
        {
            targetPos = posA.position;
        }

        Vector2 newPos = Vector2.MoveTowards(previousPos, targetPos, Speed * Time.fixedDeltaTime);
        transform.position = newPos;

        // Setting position (not MovePosition) keeps the rider's own velocity, so walking and jumping still work.
        Vector2 platformStep = newPos - previousPos;
        for (int i = 0; i < _riders.Count; i++)
        {
            _riders[i].position += platformStep;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player") || collision.attachedRigidbody == null)
        {
            return;
        }

        Rigidbody2D riderBody = collision.attachedRigidbody;
        _riderContactCounts.TryGetValue(riderBody, out int contactCount);
        _riderContactCounts[riderBody] = contactCount + 1;

        if (contactCount == 0)
        {
            _riders.Add(riderBody);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player") || collision.attachedRigidbody == null)
        {
            return;
        }

        Rigidbody2D riderBody = collision.attachedRigidbody;
        if (!_riderContactCounts.TryGetValue(riderBody, out int contactCount))
        {
            return;
        }

        if (contactCount <= 1)
        {
            _riderContactCounts.Remove(riderBody);
            _riders.Remove(riderBody);
        }
        else
        {
            _riderContactCounts[riderBody] = contactCount - 1;
        }
    }

    private void OnDisable()
    {
        _riders.Clear();
        _riderContactCounts.Clear();
    }
}
