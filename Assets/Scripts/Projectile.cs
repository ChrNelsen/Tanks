using UnityEngine;

public abstract class Projectile : MonoBehaviour
{
    public float fixedY;
    public float speed;
    public GameObject explosionEffect;
    public System.Action OnDestroyed; // <-- add this

    void Start()
    {
        fixedY = transform.position.y;
    }

    public abstract void Launch(Vector3 dir);
    public abstract void OnHit(GameObject target);

    public virtual void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Enemy"))
        {
            VehicleBase vehicle = other.GetComponent<VehicleBase>();
            if (vehicle != null)
            {
                vehicle.TakeDamage(1);
                Destroy(gameObject);
            }

            PlayExplosion();
            Destroy(gameObject);
        }
        else if (other.CompareTag("Projectile"))
        {
            PlayExplosion();
            Destroy(other.gameObject);
            Destroy(gameObject);
        }
    }

    private void PlayExplosion()
    {
        if (explosionEffect != null)
        {
            Instantiate(explosionEffect, transform.position, Quaternion.identity);
        }
    }

    private void OnDestroy()
    {
        PlayExplosion();
        OnDestroyed?.Invoke();
    }
}