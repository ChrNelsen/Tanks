using UnityEngine;

public class SmarterTurretAI : MonoBehaviour
{
    private enum TurretState { Scanning, Tracking }
    private TurretState currentState = TurretState.Scanning;

    [Header("Scanning")]
    public float scanAngle = 60f;
    public float scanSpeed = 30f;
    private float startAngle;
    private int scanDirection = 1;

    [Header("Targeting")]
    public LayerMask obstacleMask;
    public LayerMask enemyMask;
    public Transform player;
    private WeaponController weapon;

    void Start()
    {
        startAngle = transform.localEulerAngles.y;
        weapon = GetComponent<WeaponController>();

        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
                
        }
    }

    void Update()
    {
        switch (currentState)
        {
            case TurretState.Scanning:
                Scan();
                if (HasLOS())
                    currentState = TurretState.Tracking;
                break;

            case TurretState.Tracking:
                if (!HasLOS())
                    currentState = TurretState.Scanning;
                else
                    TrackAndFire();
                break;
        }
    }

    #region Scanning
    void Scan()
    {
        float currentAngle = transform.localEulerAngles.y;
        float delta = Mathf.DeltaAngle(currentAngle, startAngle);

        // Back-and-forth scanning
        float angle = currentAngle + scanDirection * scanSpeed * Time.deltaTime;
        if (Mathf.Abs(Mathf.DeltaAngle(angle, startAngle)) > scanAngle)
            scanDirection *= -1;

        transform.localRotation = Quaternion.Euler(0, angle, 0);
    }
    #endregion

    #region Targeting
    bool HasLOS()
    {
        if (player == null) return false;

        Vector3 dir = player.position - transform.position;
        dir.y = 0f;
        float distance = dir.magnitude;

        // Wall check
        if (Physics.Raycast(transform.position, dir.normalized, out RaycastHit wallHit, distance, obstacleMask))
        {
            Debug.DrawLine(transform.position, wallHit.point, Color.red); // hit wall
            return false;
        }

        // Enemy check using SphereCast
        float sphereRadius = 0.5f;
        if (Physics.SphereCast(transform.position, sphereRadius, dir.normalized, out RaycastHit enemyHit, distance, enemyMask))
        {
            Debug.DrawLine(transform.position, enemyHit.point, Color.magenta); // hit enemy
            Debug.DrawRay(enemyHit.point, Vector3.up * 2f, Color.magenta); // show hit enemy in world
            return false;
        }

        // Clear LOS
        Debug.DrawLine(transform.position, player.position, Color.green);
        return true;
    }


    void TrackAndFire()
    {
        Vector3 dir = player.position - transform.position;
        dir.y = 0f;
        if (dir.sqrMagnitude < 0.001f) return;

        // Rotate smoothly toward player
        Quaternion targetRot = Quaternion.LookRotation(dir, Vector3.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, scanSpeed * Time.deltaTime);

        // Fire if roughly aligned
        if (Vector3.Angle(transform.forward, dir) < 10f)
            weapon.Fire();
    }
    #endregion

    private void OnDrawGizmos()
    {
        if (player != null)
            Debug.DrawLine(transform.position, player.position, HasLOS() ? Color.green : Color.red);
    }

}
