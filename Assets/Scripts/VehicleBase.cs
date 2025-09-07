using UnityEngine;

public class VehicleBase : MonoBehaviour
{
    [Header("Vehicle Stats")]
    public int maxHealth = 5;
    public float moveSpeed = 5f;
    public int maxBullets = 3;
    public int bulletBounce = 1;
    public GameObject explosionEffect;

    protected int currentHealth;

    protected virtual void Start()
    {
        currentHealth = maxHealth;
    }

    public virtual void TakeDamage(int amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            DestroyVehicle();
        }
    }

    protected virtual void DestroyVehicle()
    {
        PlayExplosion();
        Destroy(gameObject);
    }

    private void PlayExplosion()
    {
        if (explosionEffect != null)
        {
            Instantiate(explosionEffect, transform.position, Quaternion.identity);
        }
    }
}