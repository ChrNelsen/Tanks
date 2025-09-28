using UnityEngine;

public class TurretAI : MonoBehaviour
{
    private enum TurretState { Scanning, Tracking }
    private TurretState currentState = TurretState.Scanning;

    [Header("Scanning")]
    public float scanAngle = 60f;
    public float scanSpeed = 30f;
    private float startAngle;
    private int scanDirection = 1;

    private TargetingSystem targeting;
    private WeaponController weapon;

    void Start()
    {
        // Use local Y rotation relative to parent
        startAngle = transform.localEulerAngles.y;
        targeting = GetComponent<TargetingSystem>();
        weapon = GetComponent<WeaponController>();
    }

    void Update()
    {
        switch (currentState)
        {
            case TurretState.Scanning:
                Scan();
                if (targeting.CanSeeTarget(transform))
                    currentState = TurretState.Tracking;
                break;

            case TurretState.Tracking:
                if (!targeting.CanSeeTarget(transform))
                    currentState = TurretState.Scanning;
                else
                    TrackAndShoot();
                break;
        }
    }

    void Scan()
    {
        float currentAngle = transform.localEulerAngles.y;
        float delta = Mathf.DeltaAngle(currentAngle, startAngle);

        if (Mathf.Abs(delta) <= scanAngle)
        {
            ScanBackAndForth();
        }
        else
        {
            ReturnToScanningPosition();
        }
    }

    void ScanBackAndForth()
    {
        float angle = transform.localEulerAngles.y + scanDirection * scanSpeed * Time.deltaTime;
        if (Mathf.Abs(Mathf.DeltaAngle(angle, startAngle)) > scanAngle)
            scanDirection *= -1;

        transform.localRotation = Quaternion.Euler(0, angle, 0);
    }

    void ReturnToScanningPosition()
    {
        float currentAngle = transform.localEulerAngles.y;
        float step = scanSpeed * Time.deltaTime;
        float newAngle = Mathf.MoveTowardsAngle(currentAngle, startAngle, step);
        transform.localRotation = Quaternion.Euler(0, newAngle, 0);

        if (Mathf.Abs(Mathf.DeltaAngle(newAngle, startAngle)) < 0.1f)
            scanDirection = 1;
    }

    void TrackAndShoot()
    {
        Transform target = targeting.CurrentTarget;
        if (target == null) return;

        Vector3 dir = target.position - transform.position;
        dir.y = 0f;

        if (dir.sqrMagnitude > 0.001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(dir, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, scanSpeed * Time.deltaTime);
        }

        weapon.Fire();
    }
}

