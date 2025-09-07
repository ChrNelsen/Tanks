using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [Header("Weapon Settings")]
    public Projectile projectilePrefab;
    public Transform firePoint;

    [Header("Stats")]
    public float fireRate = 0.5f;

    private float fireCooldown;
    private VehicleBase vehicle; // link to vehicle stats
    private int activeBullets;

    private void Start()
    {
        vehicle = GetComponentInParent<VehicleBase>();
        if (vehicle == null)
            Debug.LogWarning("WeaponController: No VehicleBase found in parent!");
    }

    private void Update()
    {
        if (fireCooldown > 0)
            fireCooldown -= Time.deltaTime;
    }

    public void Fire()
    {
        if (fireCooldown > 0) return;
        if (projectilePrefab == null || firePoint == null) return;

        // enforce max bullets
        if (vehicle != null && activeBullets >= vehicle.maxBullets)
            return;

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

        // If this is a Bullet, set its bounce limit
        Bullet bullet = projectile as Bullet;
        if (bullet != null && vehicle != null)
        {
            bullet.SetMaxBounces(vehicle.bulletBounce);
        }

        // track bullets
        activeBullets++;
        projectile.OnDestroyed += HandleProjectileDestroyed;

        fireCooldown = fireRate;
    }

    private void HandleProjectileDestroyed()
    {
        activeBullets = Mathf.Max(0, activeBullets - 1);
    }
}
