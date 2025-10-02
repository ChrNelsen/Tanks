using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class TankMoveToTargetSimple : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotateSpeed = 120f;
    [SerializeField] private Transform target;
    [SerializeField] private float rayDistance = 2f;
    [SerializeField] private LayerMask obstacleMask;

    private Rigidbody rb;
    private bool avoiding = false;
    private float remainingTurn = 0f;
    private bool cooldown = false; // new flag for "ignore target rotation"

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }

    private void FixedUpdate()
    {
        if (target == null) return;

        if (avoiding)
        {
            RotateStep();
        }
        else if (IsPathBlocked())
        {
            StartAvoidance();
        }
        else
        {
            // Only rotate toward target if cooldown is off
            if (!cooldown)
                RotateTowardsTarget();

            MoveForward();
        }
    }

    private void MoveForward()
    {
        Vector3 move = transform.forward * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + move);
    }

    private void RotateTowardsTarget()
    {
        Vector3 directionToTarget = (target.position - transform.position).normalized;
        directionToTarget.y = 0f;

        if (directionToTarget.sqrMagnitude < 0.001f) return;

        Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
        rb.MoveRotation(Quaternion.RotateTowards(rb.rotation, targetRotation, rotateSpeed * Time.fixedDeltaTime));
    }

    private void StartAvoidance()
    {
        avoiding = true;
        remainingTurn = Random.Range(45f, 90f);

        // Randomly choose left or right
        if (Random.value > 0.5f)
            remainingTurn = -remainingTurn;
    }

    private void RotateStep()
    {
        float step = Mathf.Sign(remainingTurn) * rotateSpeed * Time.fixedDeltaTime;

        if (Mathf.Abs(step) > Mathf.Abs(remainingTurn))
            step = remainingTurn;

        Quaternion turn = Quaternion.Euler(0f, step, 0f);
        rb.MoveRotation(rb.rotation * turn);

        remainingTurn -= step;

        // Done rotating
        if (Mathf.Abs(remainingTurn) <= 0.01f)
        {
            if (IsPathBlocked())
            {
                // Still blocked → queue another avoidance turn
                StartAvoidance();
            }
            else
            {
                avoiding = false;
                StartCoroutine(TargetCooldown()); // wait 1–3s before chasing target again
            }
        }
    }

    private IEnumerator TargetCooldown()
    {
        cooldown = true;
        float waitTime = Random.Range(3f, 5f);
        yield return new WaitForSeconds(waitTime);
        cooldown = false;
    }

    private bool IsPathBlocked()
    {
        return Physics.Raycast(transform.position + Vector3.up * 0.5f, transform.forward, rayDistance, obstacleMask);
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;

        Vector3 origin = transform.position + Vector3.up * 0.5f;
        Vector3 direction = transform.forward * rayDistance;

        // Green if clear, Red if blocked
        Gizmos.color = Physics.Raycast(origin, transform.forward, rayDistance, obstacleMask)
            ? Color.red
            : Color.green;

        Gizmos.DrawLine(origin, origin + direction);
        Gizmos.DrawSphere(origin + direction, 0.1f);
    }
}
