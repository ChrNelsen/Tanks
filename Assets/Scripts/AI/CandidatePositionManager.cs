using UnityEngine;
using System.Collections.Generic;

public class CandidatePositionManager: MonoBehaviour
{
    [Header("Distance Settings")]
    [SerializeField] private float minDistance = 5f;
    [SerializeField] private float maxDistance = 15f;
    [Header("Candidate Position Settings")]
    [SerializeField] private float candidateRadius = 5f;
    [SerializeField] private int numCandidatePositions = 12;
    private List<CandidatePosition> candidatePositions;


    // Generates candidate positions around the enemy within a radius
    public void GenerateCandidates(Transform enemyTransform, LayerMask groundMask, LayerMask wallMask)
    {
        var candidates = new List<CandidatePosition>();

        for (int i = 0; i < numCandidatePositions; i++)
        {
            float angle = i * Mathf.PI * 2f / numCandidatePositions;
            Vector3 offset = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle)) * candidateRadius;
            Vector3 candidatePos = enemyTransform.position + offset;

            if (IsValid(candidatePos, groundMask, wallMask))
                candidates.Add(new CandidatePosition(candidatePos));
        }

        candidatePositions = candidates;
    }

    // Check if position is on ground and not inside a wall
    private bool IsValid(Vector3 pos, LayerMask groundMask, LayerMask wallMask)
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

    // Score candidates
    public void ScoreCandidates(Vector3 playerPos, Vector3 lastKnownPlayerPos, Transform enemyTransform, LayerMask wallMask)
    {
        float shortestDistance = 100;
        CandidatePosition closetCandidate = null;
        if (lastKnownPlayerPos != null)
        {
            foreach (CandidatePosition candidate in candidatePositions)
            {
                if (candidate.GetDistance(lastKnownPlayerPos) < shortestDistance)
                {
                    shortestDistance = candidate.GetDistance(lastKnownPlayerPos);
                    closetCandidate = candidate;
                }
            }
            if (closetCandidate != null)
            {
                closetCandidate.IsCloseToLastKnownPlayerPos = true;
            }
        }

        foreach (CandidatePosition candidate in candidatePositions)
        {
            if (candidate.CheckLOS(playerPos, wallMask)) candidate.Score += 1;
            if (candidate.CheckLOS(enemyTransform.position, wallMask)) candidate.Score += 1;
            if (candidate.IsWithinMinMax(playerPos, minDistance, maxDistance)) candidate.Score += 1;
            if (candidate.IsInFront(enemyTransform, wallMask)) candidate.Score += 1;
            if (candidate.AwayFromEnemies()) candidate.Score += 1;
            if (candidate.IsCloseToLastKnownPlayerPos) candidate.Score += 1;
        }
    }

    // Pick the candidate with the highest score, random if tie
    public CandidatePosition PickBestCandidate()
    {
        int highestScore = 0;
        List<CandidatePosition> highestScoreCandidates = new List<CandidatePosition>();

        foreach (CandidatePosition candidate in candidatePositions)
        {
            if (candidate.Score == highestScore)
            {
                highestScoreCandidates.Add(candidate);
            }
            else if (candidate.Score > highestScore)
            {
                highestScore = candidate.Score;
                highestScoreCandidates.Clear();
                highestScoreCandidates.Add(candidate);
            }
        }
        int randomIndex = Random.Range(0, highestScoreCandidates.Count);
        return highestScoreCandidates[randomIndex];
    }

    public List<CandidatePosition> CandidatePositions()
    {
        return candidatePositions;
    }
}
/*
    // Gizmo debug for this candidate
    public void DrawCandidateGizmos()
    {
        // Candidate sphere
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(Position, 0.3f);

        // LOS to enemy
        Gizmos.color = HasLOSEnemy ? Color.green : Color.red;
        Gizmos.DrawLine(Position, enemyPos.transform.position);

        // LOS to player
        Gizmos.color = HasLOSPlayer ? Color.green : Color.red;
        Gizmos.DrawLine(Position, playerPos);

        // Score label drawn above candidate
#if UNITY_EDITOR
        Vector3 labelPos = Position + Vector3.up * 1.25f; // raise slightly above candidate
        UnityEditor.Handles.Label(labelPos, this.ToString());
#endif
    }
    
    public override string ToString()
    {
        string info = "Score: " + Score + "\n";
        if (HasLOSEnemy) info += "(+1) Has LOS to Enemy\n";
        if (HasLOSPlayer) info += "(+1) Has LOS to Player\n";
        if (IsWIthinMinMax())  info += "(+1) Within min Max (" + minDistance + " " + maxDistance + ")\n";
        if (IsInFront()) info += "(+1) In Front\n";
        if (AwayFromEnemies()) info += "(+1) Away from Enemies\n";
        if (isCloseToLastKnownPlayerPos) info += "(+1) Closest to Last Known Player Pos\n";
        return info;
    }*/