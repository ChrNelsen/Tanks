using UnityEngine;
using System.Collections.Generic;

public class CandidatePositionManager
{
    private LayerMask groundMask;
    private LayerMask wallMask;
    private Transform enemyTransform;
    private float candidateRadius;
    private float minDistance;
    private float maxDistance;

    public List<CandidatePosition> GenerateCandidates(int count, Vector3 playerPosition)
    {
        var candidates = new List<CandidatePosition>();

        for (int i = 0; i < count; i++)
        {
            float angle = i * Mathf.PI * 2f / count;
            Vector3 offset = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle)) * candidateRadius;
            Vector3 candidatePos = enemyTransform.position + offset;

            if (IsValid(candidatePos, playerPosition))
                candidates.Add(new CandidatePosition(candidatePos));
        }

        return candidates;
    }

    private bool IsValid(Vector3 pos, Vector3 playerPos)
    {
        // Ground check
        Vector3 start = pos + Vector3.up * 2f;
        if (!Physics.Raycast(start, Vector3.down, out RaycastHit hit, 5f, groundMask))
            return false;

        // Wall check
        if (Physics.OverlapSphere(pos, 0.5f, wallMask).Length > 0)
            return false;

        return true;
    }

    // Constructor
    public CandidatePositionManager(Transform enemyTransform, float radius, LayerMask groundMask, LayerMask wallMask, float minDistance = 5f, float maxDistance = 15f)
    {
        this.enemyTransform = enemyTransform;
        this.candidateRadius = radius;
        this.groundMask = groundMask;
        this.wallMask = wallMask;
        this.minDistance = minDistance;
        this.maxDistance = maxDistance;
    }
}


