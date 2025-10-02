using UnityEngine;

public class CandidatePosition
{
    public Vector3 Position;
    public bool HasLOSEnemy;   // LOS to the enemy that generated this position
    public bool HasLOSPlayer;  // LOS to the player
    public int Score;

    private Vector3 playerPos, enemyPos;

    private bool CheckLOS(Vector3 target, LayerMask wallMask)
    {
        Vector3 direction = target - Position;
        float distance = direction.magnitude;
        direction.Normalize();

        // Raycast from candidate to target to check if blocked
        if (Physics.Raycast(Position, direction, distance, wallMask))
            return false;

        return true;
    }

    // Constructor
    public CandidatePosition(Vector3 position, Vector3 enemyPos, Vector3 playerPos, LayerMask wallMask)
    {
        this.enemyPos = enemyPos;
        this.playerPos = playerPos;
        Position = position;
        Score = 0;
        HasLOSEnemy = CheckLOS(enemyPos, wallMask);
        HasLOSPlayer = CheckLOS(playerPos, wallMask);
    }

    // Gizmo debug for this candidate
    public void DrawGizmos()
    {
        // Candidate sphere
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(Position, 0.3f);

        // LOS to enemy
        Gizmos.color = HasLOSEnemy ? Color.green : Color.red;
        Gizmos.DrawLine(Position, enemyPos);

        // LOS to player
        Gizmos.color = HasLOSPlayer ? Color.green : Color.red;
        Gizmos.DrawLine(Position, playerPos);

        // Score label drawn above candidate
        #if UNITY_EDITOR
        Vector3 labelPos = Position + Vector3.up * 1f; // raise slightly above
        UnityEditor.Handles.Label(labelPos, Score.ToString());
        #endif
    }
}