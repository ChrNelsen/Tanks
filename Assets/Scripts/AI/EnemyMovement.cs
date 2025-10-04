using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(VehicleBase))]
[RequireComponent(typeof(ObstacleDetector))]
[RequireComponent(typeof(EnemyRotationController))]
[RequireComponent(typeof(CandidatePositionManager))]
public class EnemyMovement : MonoBehaviour
{
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
    private Vector3 lastKnownPlayerPos, currentTarget;

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
        candidateManager = GetComponent<CandidatePositionManager>();

        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        navigator = new EnemyNavigator(rb, vehicleBase, obstacleDetector, rotationController);
    }

    private void Start()
    {
        UpdateCandidatePositions();
    }

    private void FixedUpdate()
    {
        if (currentTarget != Vector3.zero)
        {
            navigator.HandleNavigation(currentTarget, canMove);

            // Check if reached target
            if (Vector3.Distance(transform.position, currentTarget) <= reachThreshold)
            {
                UpdateCandidatePositions();
                candidateRefreshTimer = 0f;
            }
        }

        // Periodic refresh
        candidateRefreshTimer += Time.fixedDeltaTime;
        if (candidateRefreshTimer >= candidateRefreshInterval)
        {
            UpdateCandidatePositions();
            candidateRefreshTimer = 0f;
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
    }

    private void UpdateCandidatePositions()
    {
        candidateManager.GenerateCandidates(transform, groundMask, wallMask);
        candidateManager.ScoreCandidates(player.position, lastKnownPlayerPos, transform, wallMask);
        currentTarget = candidateManager.PickBestCandidate().Position;
    }

    private bool HasLineOfSight(Vector3 from, Vector3 to)
    {
        return Physics.Raycast(from, (to - from).normalized, out RaycastHit hit) && hit.transform.CompareTag("Player");
    }

    #endregion

    #region Public Control

    public void StopMovement() => canMove = false;
    public void StartMovement() => canMove = true;
    public bool IsMoving() => canMove;

    #endregion

    #region Gizmos

    /*
    private void OnDrawGizmos()
    {
        foreach (CandidatePosition c in candidateManager.CandidatePositions())
        {
            c.DrawGizmos();
        }

        if (player != null)
        {
            Gizmos.color = HasLineOfSight(transform.position, player.position) ? Color.green : Color.red;
            Gizmos.DrawLine(transform.position, player.position);
        }
        if (currentTarget != Vector3.zero)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(currentTarget, 0.5f);
            Gizmos.DrawLine(transform.position, currentTarget);
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
    */
    #endregion
}
