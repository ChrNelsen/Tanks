using UnityEngine;

public class EnemyRotationController : MonoBehaviour
{
    [SerializeField] private float rotateSpeed = 120f;

    private Rigidbody rb;
    private bool rotating;
    private float remainingRotation;
    private int rotationDirection; // 1 = clockwise, -1 = counterclockwise
    private int desiredTurnDir; // +1 = right, -1 = left

    private void Awake() => rb = GetComponent<Rigidbody>();

    #region Rotation Methods

    // Rotate smoothly toward a world Y angle
    public void RotateTowardsAngle(float targetY)
    {
        float currentY = rb.rotation.eulerAngles.y;
        float diff = Mathf.DeltaAngle(currentY, targetY);

        if (Mathf.Abs(diff) < 0.01f) return;

        StartRotation(diff);
    }

    // Rotate toward a target position, respecting obstacles
    public void RotateTowardsTarget(Vector3 target, bool blockedRight, bool blockedLeft)
    {
        Vector3 direction = target - transform.position;
        direction.y = 0f;
        if (direction.sqrMagnitude < 0.0001f) return;

        float angle = Vector3.SignedAngle(transform.forward, direction.normalized, Vector3.up);
        int turnDir = angle > 0 ? 1 : -1;

        // Stop if blocked or already facing target
        if ((turnDir == 1 && blockedRight) || (turnDir == -1 && blockedLeft) || Mathf.Abs(angle) < 1f) return;

        // Smoothly rotate using the same step logic
        StartRotation(angle);
    }

    // Rotate by a fixed amount relative to current rotation
    public void RotateByRelative(float degrees) => RotateTowardsAngle(rb.rotation.eulerAngles.y + degrees);

    // Step rotation each FixedUpdate if currently rotating
    public void HandleRotationStep()
    {
        if (!rotating) return;

        float step = Mathf.Min(rotateSpeed * Time.fixedDeltaTime, remainingRotation) * rotationDirection;
        rb.MoveRotation(rb.rotation * Quaternion.Euler(0f, step, 0f));
        remainingRotation -= Mathf.Abs(step);

        if (remainingRotation <= 0.01f)
            rotating = false;
    }

    #endregion

    #region Helpers

    private void StartRotation(float angle)
    {
        rotationDirection = angle > 0 ? 1 : -1;
        desiredTurnDir = rotationDirection;
        remainingRotation = Mathf.Abs(angle);
        rotating = true;
    }

    public bool IsRotating() => rotating;

    public int GetDesiredTurnDir() => desiredTurnDir;

    #endregion
}
