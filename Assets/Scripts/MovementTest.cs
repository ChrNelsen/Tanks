using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(VehicleBase))]
public class MovementTest : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float rotateSpeed = 120f;
    [SerializeField] private float rayDistance = 1.5f;
    [SerializeField] private LayerMask obstacleMask;

    [Header("Target Settings")]
    [SerializeField] private Transform target;

    private Rigidbody rb;
    private VehicleBase vehicleBase;

    // Rotation state
    private bool rotating;
    private float remainingRotation;
    private int rotationDirection; // 1 = clockwise, -1 = counterclockwise

    private bool blockedOnLeft;
    private bool blockedOnRight;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        vehicleBase = GetComponent<VehicleBase>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }

    private void FixedUpdate()
    {
        CheckObstacles();

        if (rotating)
            HandleRotationStep();
        else if (target != null)
            RotateTowardsTarget();

        MoveForward();
    }

    private void MoveForward()
    {
        rb.MovePosition(rb.position + transform.forward * vehicleBase.moveSpeed * Time.fixedDeltaTime);
    }

    private void HandleRotationStep()
    {
        float step = Mathf.Min(rotateSpeed * Time.fixedDeltaTime, remainingRotation) * rotationDirection;
        rb.MoveRotation(rb.rotation * Quaternion.Euler(0f, step, 0f));
        remainingRotation -= Mathf.Abs(step);

        if (remainingRotation <= 0.01f)
            rotating = false;
    }

    private void RotateTowardsTarget()
    {
        int dir = GetRotationDirectionToTarget();
        if ((dir == 1 && blockedOnRight) || (dir == -1 && blockedOnLeft) || dir == 0) return;

        Vector3 direction = (target.position - transform.position).normalized;
        direction.y = 0f;

        float angleToTarget = Vector3.SignedAngle(transform.forward, direction, Vector3.up);
        float step = Mathf.Clamp(angleToTarget, -rotateSpeed * Time.fixedDeltaTime, rotateSpeed * Time.fixedDeltaTime);

        rb.MoveRotation(rb.rotation * Quaternion.Euler(0f, step, 0f));
    }

    private void CheckObstacles()
    {
        bool hitForward = RayHit(transform.forward, rayDistance);
        bool hitLeft = RayHit(Quaternion.Euler(0, -20f, 0) * transform.forward, rayDistance);
        bool hitRight = RayHit(Quaternion.Euler(0, 20f, 0) * transform.forward, rayDistance);

        blockedOnLeft = RayHit(Quaternion.Euler(0, -60f, 0) * transform.forward, 2f);
        blockedOnRight = RayHit(Quaternion.Euler(0, 60f, 0) * transform.forward, 2f);

        if (hitForward)
        {
            if (hitLeft && !hitRight) RotateToNearestAngle(1);
            else if (hitRight && !hitLeft) RotateToNearestAngle(-1);
            else RotateToNearestAngle(1);
        }
        else if (hitLeft) RotateToNearestAngle(1);
        else if (hitRight) RotateToNearestAngle(-1);
    }

    private void RotateToNearestAngle(int direction)
    {
        float[] allowedAngles = { 45f, 90f, 135f, 180f, 225f, 270f, 315f, 360f };
        float currentY = rb.rotation.eulerAngles.y;
        float targetY = currentY;

        if (direction > 0) // Clockwise
        {
            foreach (float angle in allowedAngles)
            {
                if (angle > currentY) { targetY = angle; break; }
            }
            if (targetY <= currentY) targetY = allowedAngles[0];
        }
        else // Counterclockwise
        {
            for (int i = allowedAngles.Length - 1; i >= 0; i--)
            {
                if (allowedAngles[i] < currentY) { targetY = allowedAngles[i]; break; }
            }
            if (targetY >= currentY) targetY = allowedAngles[^1];
        }

        float diff = Mathf.DeltaAngle(currentY, targetY);
        remainingRotation = Mathf.Abs(diff);
        rotationDirection = diff >= 0 ? 1 : -1;
        rotating = true;
    }

    private int GetRotationDirectionToTarget()
    {
        if (target == null) return 0;

        Vector3 dir = (target.position - transform.position).normalized;
        dir.y = 0f;

        if (dir.sqrMagnitude < 0.0001f) return 0;

        float angle = Vector3.SignedAngle(transform.forward, dir, Vector3.up);
        if (Mathf.Abs(angle) < 1f) return 0;

        return angle > 0 ? 1 : -1;
    }

    private bool RayHit(Vector3 direction, float distance) =>
        Physics.Raycast(transform.position, direction, distance, obstacleMask);

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;

        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.forward * rayDistance);

        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, Quaternion.Euler(0, -20f, 0) * transform.forward * rayDistance);
        Gizmos.DrawRay(transform.position, Quaternion.Euler(0, 20f, 0) * transform.forward * rayDistance);

        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, Quaternion.Euler(0, -60f, 0) * transform.forward * 2f);
        Gizmos.DrawRay(transform.position, Quaternion.Euler(0, 60f, 0) * transform.forward * 2f);
    }
}
