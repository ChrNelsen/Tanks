using UnityEngine;

public class PlayerTurret : MonoBehaviour
{
    [Header("References")]
    public Transform firePoint;
    public WeaponController weaponController; // modular weapon system

    private void Update()
    {
        AimWithMouse();

        // Fire bullet when left mouse button is clicked
        if (Input.GetMouseButtonDown(0))
        {
            weaponController.TryShoot();
        }
    }

    private void AimWithMouse()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hitInfo))
        {
            Vector3 lookPos = hitInfo.point - transform.position;
            lookPos.y = 0; // keep turret flat
            if (lookPos.sqrMagnitude > 0.01f)
            {
                Quaternion rotation = Quaternion.LookRotation(lookPos);
                transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * 10f);
            }
        }
    }
}
