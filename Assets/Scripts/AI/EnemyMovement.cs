using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(VehicleBase))]
[RequireComponent(typeof(ObstacleDetector))]
[RequireComponent(typeof(EnemyRotationController))]
public class EnemyMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float rotateSpeed = 120f;

    [Header("Target Settings")]
    [SerializeField] private Transform target;

    private EnemyRotationController rotationController;

    private Rigidbody rb;
    private VehicleBase vehicleBase;
    private ObstacleDetector obstacleDetector;
    private static readonly float[] allowedAngles = { 45f, 90f, 135f, 180f, 225f, 270f, 315f, 360f };

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        vehicleBase = GetComponent<VehicleBase>();
        obstacleDetector = GetComponent<ObstacleDetector>();
        rotationController = GetComponent<EnemyRotationController>();
    }

    private void FixedUpdate()
    {
        // First, handle obstacle-based rotations
        CheckObstacles();

        if (rotationController.IsRotating() != true)
        {
            rotationController.RotateTowardsTarget(target.transform.position, obstacleDetector.FarRightBlocked, obstacleDetector.FarLeftBlocked);
        }

        // Apply rotation step if currently rotating
        rotationController.HandleRotationStep();

        // Move forward every FixedUpdate
        MoveForward();
    }

    private void MoveForward()
    {
        rb.MovePosition(rb.position + transform.forward * vehicleBase.moveSpeed * Time.fixedDeltaTime);
    }

    private void CheckObstacles()
    {
        float currentY = rb.rotation.eulerAngles.y; // get current rotation

        if (obstacleDetector.ForwardBlocked)
        {
            if (obstacleDetector.LeftBlocked && !obstacleDetector.RightBlocked) rotationController.RotateByRelative(45);
            else if (obstacleDetector.RightBlocked && !obstacleDetector.LeftBlocked) rotationController.RotateByRelative(-45);
            else rotationController.RotateByRelative(45);
        }
        else if (obstacleDetector.LeftBlocked)
        {
            // Rotate to the next allowed angle clockwise
            float nearest = FindNearestAllowedAngle(currentY, 1);
            rotationController.RotateTowardsAngle(nearest);
        }
        else if (obstacleDetector.RightBlocked)
        {
            // Rotate to the next allowed angle clockwise
            float nearest = FindNearestAllowedAngle(currentY, -1);
            rotationController.RotateTowardsAngle(nearest);
        }
    }

    private float FindNearestAllowedAngle(float currentY, int direction)
    {
        if (direction > 0) // clockwise
        {
            foreach (float angle in allowedAngles)
                if (angle > currentY) return angle;
            return allowedAngles[0]; // wrap around
        }
        else // counterclockwise
        {
            for (int i = allowedAngles.Length - 1; i >= 0; i--)
                if (allowedAngles[i] < currentY) return allowedAngles[i];
            return allowedAngles[^1]; // wrap around
        }
    }
}