using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(VehicleBase))]
[RequireComponent(typeof(ObstacleDetector))]
[RequireComponent(typeof(EnemyRotationController))]
public class EnemyMovement : MonoBehaviour
{
    [Header("Target Settings")]
    [SerializeField] private Transform target;

    private Rigidbody rb;
    private VehicleBase vehicleBase;
    private ObstacleDetector obstacleDetector;
    private EnemyRotationController rotationController;

    private EnemyNavigator navigator;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        vehicleBase = GetComponent<VehicleBase>();
        obstacleDetector = GetComponent<ObstacleDetector>();
        rotationController = GetComponent<EnemyRotationController>();

        navigator = new EnemyNavigator(rb, vehicleBase, obstacleDetector, rotationController);
    }

    private void FixedUpdate()
    {
        navigator.HandleNavigation(target.position);
    }
}
