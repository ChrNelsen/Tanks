using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [Header("Weapon Settings")]
    public Projectile projectilePrefab;
    public Transform firePoint;
    public float fireRate = 1f;

    private float fireCooldown;

    private void Update()
    {
        if (fireCooldown > 0)
            fireCooldown -= Time.deltaTime;
    }

    public void Fire()
    {
        if (fireCooldown > 0) return;
        if (projectilePrefab == null || firePoint == null) return;

        // spawn muzzle flash
        /*
        if (muzzleFlashEffect != null)
        {
            Instantiate(muzzleFlashEffect, firePoint.position, firePoint.rotation, firePoint);
        }
        */

        // spawn projectile
        Projectile projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        projectile.Launch(firePoint.forward);

        fireCooldown = fireRate;
    }
}
