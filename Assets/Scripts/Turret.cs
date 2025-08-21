using UnityEngine;

public class Turret : MonoBehaviour
{

    public Transform firePoint;
    public Projectile projectilePrefab;

    void Update()
    {
        // Fire bullet when left mouse button is clicked
        if (Input.GetMouseButtonDown(0)) // 0 = left click
        {
            FireBullet();
        }
    }
    private void FireBullet()
    {
        Projectile projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity, null);
        projectile.Launch(firePoint.forward);
    }
}
