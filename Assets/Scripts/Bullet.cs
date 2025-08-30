using UnityEngine;

public class Bullet : Projectile
{
    [SerializeField] int maxBounces = 3;

    private int bounceCount = 0;
    private Vector3 direction;

    public override void Launch(Vector3 dir)
    {
        dir.y = 0f; // Flatten direction to stay on fixed Y plane
        direction = dir.normalized;
    }

    private void Update()
    {
        // Move bullet at constant speed
        Vector3 movement = direction * speed * Time.deltaTime;
        transform.position += movement;

        // Keep Y fixed
        Vector3 pos = transform.position;
        pos.y = fixedY;
        transform.position = pos;
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Destroy both projectiles if they collide
        if (collision.gameObject.tag == "Projectile")
        {
            Destroy(collision.gameObject);
            Destroy(gameObject);
            return;
        }

        Vector3 normal = collision.GetContact(0).normal;

        // Snap normal to clsest axis direction
        if (Mathf.Abs(normal.x) > Mathf.Abs(normal.z))
            normal = new Vector3(Mathf.Sign(normal.x), 0f, 0f);
        else
            normal = new Vector3(0f, 0f, Mathf.Sign(normal.z));

        // Reflect bullet
        direction = Vector3.Reflect(direction, normal).normalized;

        bounceCount++;
        if (bounceCount > maxBounces)
        {
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
        } */
    }
}