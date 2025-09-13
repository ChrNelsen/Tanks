using UnityEngine;

public class Mine : MonoBehaviour
{
    [Header("Explosion Settings")]
    public float explodeAfterSeconds = 5f; // Time until auto-explosion
    public float explosionRadius = 5f;     // Radius of explosion
    public GameObject explosionEffect;     // Optional: Particle effect prefab

    private bool exploded = false;

    void Start()
    {
        // Schedule automatic explosion
        Invoke(nameof(Explode), explodeAfterSeconds);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !exploded)
        {
            Explode();
        }
    }

    void Explode()
    {
        if (exploded) return;
        exploded = true;

        // Optional: show explosion effect
        if (explosionEffect != null)
        {
            Instantiate(explosionEffect, transform.position, Quaternion.identity);
        }

        // Spawn "explosion sphere" and detect breakables
        Collider[] hits = Physics.OverlapSphere(transform.position, explosionRadius);

        foreach (Collider hit in hits)
        {
            if (hit.CompareTag("Breakable"))
            {
                Destroy(hit.gameObject);
            }
        }

        // Destroy the mine object
        Destroy(gameObject);
    }

    // Optional: Draw explosion radius in Scene view
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
