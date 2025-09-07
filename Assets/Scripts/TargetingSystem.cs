using UnityEngine;

public class TargetingSystem : MonoBehaviour
{
    [Header("Detection")]
    public float detectionRange = 10f;
    public float detectionAngle = 45f;
    public LayerMask obstacleMask;
    public LayerMask playerMask;

    public Transform CurrentTarget { get; private set; }

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            CurrentTarget = playerObj.transform;
        }
    }

    public bool CanSeeTarget(Transform origin)
    {
        if (CurrentTarget == null) return false;

        Vector3 toTarget = CurrentTarget.position - origin.position;
        if (toTarget.magnitude > detectionRange) return false;
        if (Vector3.Angle(origin.forward, toTarget) > detectionAngle) return false;

        int mask = obstacleMask | playerMask;
        if (Physics.Raycast(origin.position, toTarget.normalized, out RaycastHit hit, toTarget.magnitude, mask))
        {
            if (((1 << hit.collider.gameObject.layer) & obstacleMask) != 0)
                return false;
        }

        return true;
    }
}
