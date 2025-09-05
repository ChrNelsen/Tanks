using UnityEngine;

public class Bullet : Projectile
{
    [SerializeField] int maxBounces = 3;
    public float radius = 0.1f;  // radius of the "bullet"

    private int bounceCount = 0;
    private Vector3 direction;
    public LayerMask collisionMask; // Assign your walls layer here

    public override void Launch(Vector3 dir)
    {
        direction = dir.normalized;
    }

    private void Update()
    {
        float distance = speed * Time.deltaTime;

        // Check if we hit something this frame
        if (Physics.SphereCast(transform.position, radius, direction, out RaycastHit hit, distance, collisionMask))
        {
            // Check if we hit player or another projectile
            if (hit.collider.CompareTag("Player") || hit.collider.CompareTag("Projectile"))
            {
                Destroy(gameObject);
                if(hit.collider.CompareTag("Projectile"))
                {
                    // Destroy the other projectile as well
                    Destroy(hit.collider.gameObject);
                }
                return;
            }
            // Reflect the direction based on surface normal
            direction = Vector3.Reflect(direction, hit.normal);

            // Move to the hit point so we don't overlap
            transform.position += direction * radius;

            // Count this bounce
            bounceCount++;
            if (bounceCount >= maxBounces)
            {
                Destroy(gameObject);
                return;
            }
        }
        else
        {
            // Move normally if no hit
            transform.position += direction * distance;
        }
    }

    // Future expansion for projectile abilities (piercing, freezing, etc.)
    public override void OnHit(GameObject target)
    {
        return;
        /*
        // Run abilities if attached (piercing, freezing, etc.)
        foreach (var ability in GetComponents<IProjectileAbility>())
        {
            ability.OnHit(this, target);
        }
        */
    }
}