using UnityEngine;

public class CandidatePosition
{
    public Vector3 Position;
    public int Score;
    public bool IsCloseToLastKnownPlayerPos = false;

    // Check line of sight to a target position
    public bool CheckLOS(Vector3 target, LayerMask wallMask)
    {
        Vector3 direction = target - Position;
        float distance = direction.magnitude;
        direction.Normalize();

        // Raycast from candidate to target to check if blocked
        if (Physics.Raycast(Position, direction, distance, wallMask))
            return false;

        return true;
    }

    // Get distance to a point
    public float GetDistance(Vector3 aPoint)
    {
        return Vector3.Distance(Position, aPoint);
    }

    // Check if away from enemies within a certain radius
    public bool AwayFromEnemies(float radius = 5f)
    {
        Collider[] hitColliders = Physics.OverlapSphere(Position, radius);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Enemy"))
            {
                return false;
            }
        }
        return true;
    }

    // Check if candidate is in front of the enemy and not blocked
    public bool IsInFront(Transform enemyPos, LayerMask wallMask)
    {
        Vector3 enemyForwardDir = enemyPos.transform.forward;
        float angle = Vector3.Angle(enemyForwardDir, Position);
        if (angle <= 90f && CheckLOS(enemyPos.transform.position, wallMask))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    // Check if within min and max distance from player
    public bool IsWithinMinMax(Vector3 playerPos, float minDistance, float maxDistance)
    {
        if (GetDistance(playerPos) >= minDistance && GetDistance(playerPos) <= maxDistance)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    
    // Constructor
    public CandidatePosition(Vector3 position)
    {
        Position = position;
        Score = 0;
    }
}