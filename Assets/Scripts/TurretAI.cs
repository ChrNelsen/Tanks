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
        startAngle = transform.eulerAngles.y;
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
        float angle = transform.eulerAngles.y + scanDirection * scanSpeed * Time.deltaTime;
        if (Mathf.Abs(Mathf.DeltaAngle(angle, startAngle)) > scanAngle)
            scanDirection *= -1;

        transform.rotation = Quaternion.Euler(0, angle, 0);
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

