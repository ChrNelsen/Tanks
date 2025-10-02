using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(VehicleBase))]
[RequireComponent(typeof(ObstacleDetector))]
[RequireComponent(typeof(EnemyRotationController))]
public class EnemyMovement : MonoBehaviour
{
    [Header("Distance Settings")]
    [SerializeField] private float minDistance = 5f, maxDistance = 15f;
    [Header("Candidate Settings")]
    [SerializeField] private float candidateRadius = 5f;
    [SerializeField] private int numCandidatePositions = 12;
    [Header("Environment")]
    [SerializeField] private LayerMask groundMask, wallMask;
    [Header("Peek Settings")]
    [SerializeField] private float peekChance = 0.2f, peekDurationMin = 0.5f, peekDurationMax = 1.5f;
    [Header("Search Settings")]
    [SerializeField] private float searchDuration = 10f;
    [SerializeField] private float reachThreshold = 0.5f; // How close to target before picking a new one

    [SerializeField] private float candidateRefreshInterval = 2f;
    private float candidateRefreshTimer;

    private Rigidbody rb;
    private VehicleBase vehicleBase;
    private ObstacleDetector obstacleDetector;
    private EnemyRotationController rotationController;
    private EnemyNavigator navigator;
    private CandidatePositionManager candidateManager;
    private Transform player;

    private bool isPeeking, canMove = true;
    private float peekTimer, peekDuration, lostSightTimer;
    private Vector3 lastKnownPlayerPos, CurrentTarget;
    private List<CandidatePosition> candidatePositions = new();

    private bool IsSearching => !HasLineOfSight(transform.position, player.position)
                                && lostSightTimer < searchDuration
                                && lastKnownPlayerPos != Vector3.zero;

    #region Unity Methods

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

    private void Start() => UpdateCandidatePositions();

    private void FixedUpdate()
    {
        if (CurrentTarget != Vector3.zero)
        {
            navigator.HandleNavigation(CurrentTarget, canMove);

            // Check if reached target
            if (Vector3.Distance(transform.position, CurrentTarget) <= reachThreshold)
            {
                // If we were searching the last known player pos, allow refreshing again
                if (IsSearching && CurrentTarget == lastKnownPlayerPos)
                {
                    UpdateCandidatePositions();
                    candidateRefreshTimer = 0f; // reset timer when search target is reached
                }
                else
                {
                    UpdateCandidatePositions();
                    candidateRefreshTimer = 0f;
                }
            }
        }

        // Periodic refresh
        candidateRefreshTimer += Time.fixedDeltaTime;
        if (candidateRefreshTimer >= candidateRefreshInterval)
        {
            // Don’t refresh if currently searching and haven’t reached last known player pos
            if (!(IsSearching && CurrentTarget == lastKnownPlayerPos))
            {
                UpdateCandidatePositions();
                candidateRefreshTimer = 0f;
            }
        }

        SeekPlayer();
    }



    private void Update()
    {
        if (player == null) return;
        HandlePeeking();
    }

    #endregion

    #region Peeking

    private void HandlePeeking()
    {
        bool seesPlayer = HasLineOfSight(transform.position, player.position);
        if (!seesPlayer && !IsSearching)
        {
            if (!isPeeking && Random.value < peekChance * Time.deltaTime) StartPeek();
            if (isPeeking && (peekTimer += Time.deltaTime) >= peekDuration) EndPeek();
        }
        else EndPeek();
    }

    private void StartPeek() { isPeeking = true; peekDuration = Random.Range(peekDurationMin, peekDurationMax); peekTimer = 0f; canMove = false; }
    private void EndPeek() { isPeeking = false; canMove = true; }

    #endregion

    #region LOS & Candidate Logic

    private void SeekPlayer()
    {
        if (player == null) return;
        bool hasLOS = HasLineOfSight(transform.position, player.position);

        if (hasLOS)
        {
            lostSightTimer = 0f;
            lastKnownPlayerPos = player.position;
        }
        else
        {
            lostSightTimer += Time.fixedDeltaTime;
            if (IsSearching) CurrentTarget = lastKnownPlayerPos;
        }
    }

    private void UpdateCandidatePositions()
    {
        candidatePositions = candidateManager.GenerateCandidates(numCandidatePositions, player.position);
        ScoreCandidates(candidatePositions);
        PickBestCandidate();
    }

    private bool HasLineOfSight(Vector3 from, Vector3 to)
    {
        return Physics.Raycast(from, (to - from).normalized, out RaycastHit hit) && hit.transform.CompareTag("Player");
    }

    private void PickBestCandidate()
    {
        CandidatePosition best = null;
        float maxScore = float.MinValue;
        foreach (var c in candidatePositions)
            if (c.Score > maxScore) { maxScore = c.Score; best = c; }
        if (best != null) CurrentTarget = best.Position;
    }

    private void ScoreCandidates(List<CandidatePosition> candidates)
    {
        float idealDistance = (minDistance + maxDistance) / 2f;

        foreach (var c in candidates)
        {
            float distanceToPlayer = Vector3.Distance(c.Position, player.position);

            // Out of range → minimum score (1)
            if (distanceToPlayer < minDistance || distanceToPlayer > maxDistance)
            {
                c.Score = 1;
                continue;
            }

            // Distance score: closer to ideal = higher
            // Scale from 0 → 80
            int distanceScore = Mathf.RoundToInt(
                Mathf.Max(0f, 1f - Mathf.Abs(distanceToPlayer - idealDistance) / idealDistance) * 80f
            );

            // LOS bonus: 0 or +20
            int losScore = c.HasLOSPlayer ? 20 : 0;

            // Final score, clamped to 1–100
            c.Score = Mathf.Clamp(distanceScore + losScore, 1, 100);
        }
    }


    #endregion

    #region Public Control

    public void StopMovement() => canMove = false;
    public void StartMovement() => canMove = true;
    public bool IsMoving() => canMove;

    #endregion

    #region Gizmos

    private void OnDrawGizmos()
    {
        if (candidatePositions != null)
        {
            foreach (var c in candidatePositions)
                c.DrawGizmos();
        }
        if (player != null)
        {
            Gizmos.color = HasLineOfSight(transform.position, player.position) ? Color.green : Color.red;
            Gizmos.DrawLine(transform.position, player.position);
        }
        if (CurrentTarget != Vector3.zero)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(CurrentTarget, 0.5f);
            Gizmos.DrawLine(transform.position, CurrentTarget);
        }
    }

    private void DrawCandidates(List<CandidatePosition> candidates, Color color)
    {
        if (candidates == null) return;
        Gizmos.color = color;
        foreach (var c in candidates)
        {
            Gizmos.DrawSphere(c.Position, 0.5f);
            Gizmos.DrawLine(transform.position, c.Position);
        }
        if (color == Color.green) Gizmos.DrawWireSphere(transform.position, candidateRadius);
    }

    #endregion
}
