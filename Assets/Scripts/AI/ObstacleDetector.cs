using UnityEngine;

public class ObstacleDetector : MonoBehaviour
{
    [SerializeField] float middleProbesRayDistance = 1.5f;
    [SerializeField] float outSideProbesRayDistance = 2.0f;
    [SerializeField] float innerProbeAngle = 20f;
    [SerializeField] float outsideProbeAngle = 60f;
    [SerializeField] private LayerMask obstacleMask;

    public bool FarLeftBlocked { get; private set; }
    public bool LeftBlocked { get; private set; }
    public bool ForwardBlocked { get; private set; }
    public bool RightBlocked { get; private set; }
    public bool FarRightBlocked { get; private set; }

    private void FixedUpdate()
    {
        UpdateDetection();
    }

    public void UpdateDetection()
    {
        ForwardBlocked = RayHit(transform.forward, middleProbesRayDistance);
        LeftBlocked = RayHit(Quaternion.Euler(0, -innerProbeAngle, 0) * transform.forward, middleProbesRayDistance);
        RightBlocked = RayHit(Quaternion.Euler(0, innerProbeAngle, 0) * transform.forward, middleProbesRayDistance);
        FarLeftBlocked = RayHit(Quaternion.Euler(0, -outsideProbeAngle, 0) * transform.forward, outSideProbesRayDistance);
        FarRightBlocked = RayHit(Quaternion.Euler(0, outsideProbeAngle, 0) * transform.forward, outSideProbesRayDistance);
    }

    private bool RayHit(Vector3 direction, float distance) =>
        Physics.Raycast(transform.position, direction, distance, obstacleMask);

    public void OnDrawGizmos()
    {

        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.forward * middleProbesRayDistance);

        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, Quaternion.Euler(0, -innerProbeAngle, 0) * transform.forward * middleProbesRayDistance);
        Gizmos.DrawRay(transform.position, Quaternion.Euler(0, innerProbeAngle, 0) * transform.forward * middleProbesRayDistance);

        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, Quaternion.Euler(0, outsideProbeAngle, 0) * transform.forward * outSideProbesRayDistance);
        Gizmos.DrawRay(transform.position, Quaternion.Euler(0, -outsideProbeAngle, 0) * transform.forward * outSideProbesRayDistance);
    }
}

