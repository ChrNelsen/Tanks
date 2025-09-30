using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(VehicleBase))]
[RequireComponent(typeof(ObstacleDetector))]
[RequireComponent(typeof(EnemyRotationController))]
public class EnemyMovement : MonoBehaviour
{
    [Header("Distance Settings")]
    [SerializeField] private float minDistance = 5f;
    [SerializeField] private float maxDistance = 15f;

    [Header("Candidate Position Settings")]
    [SerializeField] private float candidateRadius = 5f;
    [SerializeField] private int numCandidatePositions = 12;
    [SerializeField] private float refreshInterval = 3f;

    [Header("Environment")]
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private LayerMask wallMask;

    private List<CandidatePosition> candidatePositions = new();
    private float timer = 5f;


    private CandidatePositionManager candidateManager;
    private Rigidbody rb;
    private VehicleBase vehicleBase;
    private ObstacleDetector obstacleDetector;
    private EnemyRotationController rotationController;
    private EnemyNavigator navigator;
    private Transform player;
    private CandidatePosition losCandidate;

    private Vector3 CurrentTarget { get; set; }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        vehicleBase = GetComponent<VehicleBase>();
        obstacleDetector = GetComponent<ObstacleDetector>();
        rotationController = GetComponent<EnemyRotationController>();
        candidateManager = new CandidatePositionManager(transform, candidateRadius, groundMask, wallMask, minDistance, maxDistance);

        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        navigator = new EnemyNavigator(rb, vehicleBase, obstacleDetector, rotationController);
    }

    private void Start()
    {
        UpdateCandidatePositions();
    }

    private void FixedUpdate()
    {
        navigator.HandleNavigation(CurrentTarget);
        SeekShot();
    }

    private void Update()
    {
        if (player == null) return;

        // Increase timer by the time elapsed since last frame
        timer += Time.deltaTime;

        // Check LOS and handle candidate refresh
        SeekShot();
    }

private void SeekShot()
{
    bool hasLOS = HasLineOfSight(transform.position, player.position);

    if (hasLOS)
    {
        CurrentTarget = player.position;
        /*
            timer = 0f;

            // Only drop a LOS candidate if we don't already have one

            if (losCandidate == null)
            {
                losCandidate = new CandidatePosition(transform.position);
                losCandidate.HasLOS = true;

                float distance = Vector3.Distance(transform.position, player.position);
                float idealDistance = (minDistance + maxDistance) / 2f;
                float distanceScore = Mathf.Max(0f, 1f - Mathf.Abs(distance - idealDistance) / idealDistance) * 100f;
                losCandidate.Score = distanceScore + 100f; // big LOS bonus

                CurrentTarget = losCandidate.Position;
            }
            */
            return;
    }

    // If we lost LOS, clear it and refresh after interval
    if (timer >= refreshInterval)
    {
        UpdateCandidatePositions();
        timer = 0f;
        losCandidate = null; // remove LOS candidate so we can set a new one next time
    }
}




    private void UpdateCandidatePositions()
    {
        candidatePositions = candidateManager.GenerateCandidates(numCandidatePositions, player.position);
        ScoreCandidates(candidatePositions);
        PickBestTarget();
    }

    private bool HasLineOfSight(Vector3 from, Vector3 to)
    {
        bool tf = Physics.Raycast(from, (to - from).normalized, out RaycastHit hit) && hit.transform.CompareTag("Player");
        Debug.Log(tf);
        return tf;

    }

    private void PickBestTarget()
    {
        if (candidatePositions == null || candidatePositions.Count == 0) return;

        CandidatePosition best = null;
        float highestScore = float.MinValue;

        foreach (var candidate in candidatePositions)
        {
            if (candidate.Score > highestScore)
            {
                highestScore = candidate.Score;
                best = candidate;
            }
        }

        if (best != null) CurrentTarget = best.Position;
    }

    private void ScoreCandidates(List<CandidatePosition> candidates)
    {
        float idealDistance = (minDistance + maxDistance) / 2f;

        foreach (var candidate in candidates)
        {
            Vector3 from = candidate.Position; // no offset needed for scoring
            Vector3 to = player.position;

            // Check LOS for this candidate
            candidate.HasLOS = HasLineOfSight(from, to);

            float distance = Vector3.Distance(candidate.Position, player.position);

            // Discard candidates too close or too far
            if (distance < minDistance || distance > maxDistance)
            {
                candidate.Score = float.MinValue;
                continue;
            }

            // Distance score (normalized 0-100)
            float distanceScore = Mathf.Max(0f, 1f - Mathf.Abs(distance - idealDistance) / idealDistance) * 100f;

            // LOS score: heavily favor positions with LOS
            float losScore = candidate.HasLOS ? 200f : 0f; // <- boost LOS weight

            // Final score
            candidate.Score = distanceScore + losScore;
        }
    }


    #region Gizmos
    private void OnDrawGizmos()
    {
        DrawCandidatePositions();
        DrawLineOfSight();
        DrawCurrentTarget();
    }

    private void DrawCandidatePositions()
    {
        if (candidatePositions == null) return;

        Gizmos.color = Color.green;
        foreach (var candidate in candidatePositions)
        {
            Gizmos.DrawSphere(candidate.Position, 0.5f);
            Gizmos.DrawLine(transform.position, candidate.Position);
        }

        Gizmos.DrawWireSphere(transform.position, candidateRadius);
    }

    private void DrawLineOfSight()
    {
        if (player == null) return;

        Vector3 from = transform.position;
        Vector3 to = player.position;
        bool canSee = HasLineOfSight(from, to);

        Gizmos.color = canSee ? Color.green : Color.red;
        Gizmos.DrawLine(from, to);

        if (Physics.Raycast(from, (to - from).normalized, out RaycastHit hit))
            Gizmos.DrawSphere(hit.point, 0.2f);
    }

    private void DrawCurrentTarget()
    {
        if (CurrentTarget == Vector3.zero) return;

        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(CurrentTarget, 0.5f);
        Gizmos.DrawLine(transform.position, CurrentTarget);
    }

    private void DrawLineOfSightCandidate()
    {
        if (losCandidate == null) return;

        Gizmos.color = Color.yellow; // special color for LOS candidate
        Gizmos.DrawSphere(losCandidate.Position, 0.6f);
        Gizmos.DrawLine(transform.position, losCandidate.Position);
    }
    #endregion
}
