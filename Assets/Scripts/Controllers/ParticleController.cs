using UnityEngine;

public class ParticleController : MonoBehaviour
{
    [Header("Particle Systems")]
    [SerializeField] ParticleSystem movementParticle;
    [SerializeField] ParticleSystem fallPaticle;

    [Header("Movement Parameters")]
    [Range(0, 10)]
    [SerializeField] int occurAfterVelocity;

    [Range(0, 0.2f)]
    [SerializeField] float dustFormationPeriod;

    [Header("Player References")]
    [SerializeField] Rigidbody2D playerRb;

    float counter;
    bool isGround;

    private void Update()
    {
        counter += Time.deltaTime;

        if (isGround && Mathf.Abs(playerRb.velocity.x) > occurAfterVelocity)
        {
            if (counter > dustFormationPeriod)
            {

                movementParticle.Play();
                counter = 0;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground"))
        {
            fallPaticle.Play();
            isGround = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground"))
        {
            isGround = false;
        }
    }
}
