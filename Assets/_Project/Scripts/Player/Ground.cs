using UnityEngine;

public class Ground : MonoBehaviour
{
    private bool _onGround;
    private float _friction;

    private const float PROBE_HEIGHT = 0.05f;

    [SerializeField] private LayerMask groundLayer;
    [SerializeField, Range(0.1f, 2f)] private float rayLength = 1.5f;
    [Tooltip("Width of the ground check. Wider = still counts as grounded while standing near an edge. Keep it narrower than the player's collider.")]
    [SerializeField, Range(0.05f, 1.2f)] private float probeWidth = 0.7f;

    private void FixedUpdate()
    {
        Vector2 probeSize = new Vector2(probeWidth, PROBE_HEIGHT);
        RaycastHit2D hit = Physics2D.BoxCast(transform.position, probeSize, 0f, Vector2.down, rayLength, groundLayer);
        _onGround = hit.collider != null;
    }

    public bool GetOnGround()
    {
        return _onGround;
    }
}