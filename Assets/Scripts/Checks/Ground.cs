using UnityEngine;

public class Ground : MonoBehaviour
{
    private bool _onGround;
    private float _friction;

    [SerializeField] private LayerMask groundLayer;
    [SerializeField, Range(0.1f, 2f)] private float rayLength = 1.5f;

    private void FixedUpdate()
    {
        Debug.DrawRay(transform.position, Vector2.down * rayLength, Color.red);

        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, rayLength, groundLayer);
        if (hit.collider != null)
        {
            _onGround = true;
        }
        else
        {
            _onGround = false;
        }
    }

    public bool GetOnGround()
    {
        return _onGround;
    }
}