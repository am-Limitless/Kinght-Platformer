using UnityEngine;

public class Ground : MonoBehaviour
{
    private bool _onGround;
    private float _friction;
    private float _groundAngle;
    private bool groundDetected;

    //private PhysicsMaterial2D _material;

    [SerializeField] private LayerMask groundLayer;
    [SerializeField, Range(0.1f, 2f)] private float rayLength = 1.5f; 
    [SerializeField, Range(0.1f, 1f)] private float raySpacing = 0.5f; 


    private void FixedUpdate()
    {
        // Perform multiple raycasts to check for ground contact more accurately
        groundDetected = false;
        Vector2 leftRayOrigin = new Vector2(transform.position.x - raySpacing, transform.position.y);
        Vector2 rightRayOrigin = new Vector2(transform.position.x + raySpacing, transform.position.y);

        // Check three positions: left, center, and right
        RaycastHit2D centerHit = Physics2D.Raycast(transform.position, Vector2.down, rayLength, groundLayer);
        RaycastHit2D leftHit = Physics2D.Raycast(leftRayOrigin, Vector2.down, rayLength, groundLayer);
        RaycastHit2D rightHit = Physics2D.Raycast(rightRayOrigin, Vector2.down, rayLength, groundLayer);

        // Visualize the rays for debugging
        Debug.DrawRay(transform.position, Vector2.down * rayLength, Color.red);
        Debug.DrawRay(leftRayOrigin, Vector2.down * rayLength, Color.red);
        Debug.DrawRay(rightRayOrigin, Vector2.down * rayLength, Color.red);

        // Determine if any of the raycasts hit the ground
        if (centerHit.collider != null || leftHit.collider != null || rightHit.collider != null)
        {
            groundDetected = true;
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

    //private void OnCollisionEnter2D(Collision2D collision)
    //{
    //    EvaluateCollision(collision);
    //    RetrieveFriction(collision);
    //}

    //private void OnCollisionStay2D(Collision2D collision)
    //{
    //    EvaluateCollision(collision);
    //    RetrieveFriction(collision);
    //}

    //private void OnCollisionExit2D(Collision2D collision)
    //{
    //    _onGround = false;
    //    _friction = 0;
    //}

    //private void EvaluateCollision(Collision2D collision)
    //{
    //    // Check if the collision has any contacts
    //    if (collision.contactCount > 0 && ((1 << collision.gameObject.layer) & groundLayer) != 0)
    //    {
    //        foreach (ContactPoint2D contact in collision.contacts)
    //        {
    //            _groundAngle = Vector2.Angle(contact.normal, Vector2.up);

    //            // Check if the angle is within the defined threshold to consider it as ground
    //            if (_groundAngle <= maxGroundAngle)
    //            {
    //                _onGround = true;
    //                return;
    //            }

    //        }

    //    }
    //    _onGround = false;
    //}

    //private void RetrieveFriction(Collision2D collision)
    //{
    //    PhysicsMaterial2D material = collision.rigidbody.sharedMaterial;

    //    _friction = 0;

    //    if (material != null)
    //    {
    //        _friction = material.friction;
    //    }
    //}

    //public bool GetOnGround()
    //{
    //    return _onGround;
    //}

    //public float GetFriction()
    //{
    //    return _friction;
    //}
}