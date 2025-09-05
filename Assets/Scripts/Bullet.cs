using UnityEngine;

public class Bullet : Projectile
{
    [SerializeField] int maxBounces = 3;
    [SerializeField] float radius = 0.1f;   // for SphereCast
    [SerializeField] LayerMask collisionMask; // assign only walls here

    private int bounceCount = 0;
    private Vector3 direction;

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

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Bullet hit: " + other.name);
        // ✅ Handle gameplay hits separately from walls
        if (other.CompareTag("Player"))
        {
            // Example: damage player here
            Destroy(gameObject);
        }
        else if (other.CompareTag("Projectile"))
        {
            Destroy(other.gameObject);
            Destroy(gameObject);
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