using UnityEngine;
using UnityEngine.UIElements;

public class EnemyNavigator
{
    private readonly Rigidbody rb;
    private readonly VehicleBase vehicleBase;
    private readonly ObstacleDetector obstacleDetector;
    private readonly EnemyRotationController rotationController;

    private static readonly float[] allowedAngles = { 45f, 90f, 135f, 180f, 225f, 270f, 315f, 360f };

    public void HandleNavigation(Vector3 targetPosition, bool canMove)
    {
        // First, check obstacles and rotate if necessary
        CheckObstacles(targetPosition);

        // Apply rotation step if currently rotating
        rotationController.HandleRotationStep();

        // Only move if allowed
        if (canMove)
            MoveForward();
    }

    private void MoveForward()
    {
        rb.MovePosition(rb.position + rb.transform.forward * vehicleBase.moveSpeed * Time.fixedDeltaTime);
    }

    private void CheckObstacles(Vector3 targetPosition)
    {
        float currentY = rb.rotation.eulerAngles.y;

        if (obstacleDetector.ForwardBlocked)
        {
            if (obstacleDetector.LeftBlocked && !obstacleDetector.RightBlocked)
                rotationController.RotateByRelative(45);
            else if (obstacleDetector.RightBlocked && !obstacleDetector.LeftBlocked)
                rotationController.RotateByRelative(-45);
            else
            {
                if (!obstacleDetector.FarLeftBlocked)
                {
                    rotationController.RotateByRelative(-45);
                }
                else if (!obstacleDetector.FarRightBlocked)
                {
                    rotationController.RotateByRelative(45);
                }
                else
                {
                    rotationController.RotateByRelative(90);
                }
            }
        }
        else if (obstacleDetector.LeftBlocked)
        {
            float nearest = FindNearestAllowedAngle(currentY, 1);
            rotationController.RotateTowardsAngle(nearest);
        }
        else if (obstacleDetector.RightBlocked)
        {
            float nearest = FindNearestAllowedAngle(currentY, -1);
            rotationController.RotateTowardsAngle(nearest);
        }
        else if (!rotationController.IsRotating())
        {
            // Rotate toward target if nothing is blocking
            rotationController.RotateTowardsTarget(targetPosition, obstacleDetector.FarRightBlocked, obstacleDetector.FarLeftBlocked);
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

    // Constructor
    public EnemyNavigator(Rigidbody rb, VehicleBase vehicleBase, ObstacleDetector obstacleDetector, EnemyRotationController rotationController)
    {
        this.rb = rb;
        this.vehicleBase = vehicleBase;
        this.obstacleDetector = obstacleDetector;
        this.rotationController = rotationController;
    }
}
