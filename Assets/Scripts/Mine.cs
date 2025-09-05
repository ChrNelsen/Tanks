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
        exploded = true;

        // Optional: show explosion effect
        if (explosionEffect != null)
        {
            Instantiate(explosionEffect, transform.position, Quaternion.identity);
        }

        // Destroy the mine object
        Destroy(gameObject);
    }
}