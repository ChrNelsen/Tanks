using Unity.VisualScripting;
using UnityEngine;

public class EnemyRotationController : MonoBehaviour
{
    private float rotateSpeed = 120f;
    private Rigidbody rb;
    private bool rotating;
    private float remainingRotation;
    private int rotationDirection; // 1 = clockwise, -1 = counterclockwise
    private int desiredTurnDir;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Rotates toward a given target angle (world Y rotation) smoothly.
    public void RotateTowardsAngle(float targetY)
    {
        float currentY = rb.rotation.eulerAngles.y;
        float diff = Mathf.DeltaAngle(currentY, targetY); // shortest signed angle

        if (Mathf.Abs(diff) < 0.01f) return;

        // Update disired Turn Direction while doing this
        desiredTurnDir = diff > 0 ? 1 : -1; // +1 right, -1 left
        remainingRotation = Mathf.Abs(diff);
        rotationDirection = diff > 0 ? 1 : -1;
        rotating = true;
    }

    /// Rotates a bit each FixedUpdate until done.
    public void HandleRotationStep()
    {
        if (!rotating) return;

        float step = Mathf.Min(rotateSpeed * Time.fixedDeltaTime, remainingRotation) * rotationDirection;
        rb.MoveRotation(rb.rotation * Quaternion.Euler(0f, step, 0f));
        remainingRotation -= Mathf.Abs(step);

        if (remainingRotation <= 0.01f)
            rotating = false;
    }

    public void RotateTowardsTarget(Vector3 target, bool blockedOnRight, bool blockedOnLeft)
    {
        // Direction to target
        Vector3 direction = (target - transform.position).normalized;
        direction.y = 0f;

        if (direction.sqrMagnitude < 0.0001f) return;

        // Angle to target
        float angleToTarget = Vector3.SignedAngle(transform.forward, direction, Vector3.up);
        int dir = Mathf.Abs(angleToTarget) < 1f ? 0 : (angleToTarget > 0 ? 1 : -1);

        // Don't rotate if blocked
        if ((dir == 1 && blockedOnRight) || (dir == -1 && blockedOnLeft) || dir == 0) return;

        // Clamp rotation step
        float step = Mathf.Clamp(angleToTarget, -rotateSpeed * Time.fixedDeltaTime, rotateSpeed * Time.fixedDeltaTime);
        rb.MoveRotation(rb.rotation * Quaternion.Euler(0f, step, 0f));
    }


    // Rotate by a fixed amount (e.g., 90 degrees) or to nearest allowed angle.
    public void RotateByRelative(float degrees)
    {
        float targetY = rb.rotation.eulerAngles.y + degrees;
        RotateTowardsAngle(targetY);
    }

    public bool IsRotating()
    {
        if (rotating)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public int GetDesiredTurnDir()
    {
        return desiredTurnDir;
    }
}
