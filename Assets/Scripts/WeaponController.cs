using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [Header("Weapon Settings")]
    public Projectile bulletPrefab;
    public Transform firePoint;
    public float fireRate = 1f;

    private float fireTimer = 0f;

    

    public void TryShoot()
    {
        fireTimer += Time.deltaTime;
        if (fireTimer >= fireRate)
        {
            Shoot();
            fireTimer = 0f;
        }
    }

    private void Shoot()
    {
        if (bulletPrefab == null || firePoint == null) return;
        Projectile projectile = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        projectile.Launch(firePoint.forward);
    }
}
