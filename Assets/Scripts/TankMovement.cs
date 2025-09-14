using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class TankMovement : MonoBehaviour
{
    [SerializeField] private float rotateSpeed = 120f;
    private Rigidbody rb;
    private VehicleBase vehicle;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        vehicle = GetComponent<VehicleBase>();
    }

    private void FixedUpdate()
    {
        HandleMovement();
        HandleRotation();
    }

    private void HandleMovement()
    {
        float moveInput = Input.GetAxis("Vertical");
        Vector3 moveDirection = transform.forward * moveInput * vehicle.moveSpeed * Time.fixedDeltaTime;

        bool isBlocked = Physics.CapsuleCast(
            transform.position,
            transform.position + Vector3.up * 1f,
            0.5f,
            moveDirection.normalized,
            moveDirection.magnitude,
            Physics.DefaultRaycastLayers,
            QueryTriggerInteraction.Ignore   // <-- ignore trigger colliders
        );

        if (!isBlocked)
            rb.MovePosition(rb.position + moveDirection);
    }

    private void HandleRotation()
    {
        float turnInput = Input.GetAxis("Horizontal");
        float rotationAmount = turnInput * rotateSpeed * Time.fixedDeltaTime;
        Quaternion turnOffset = Quaternion.Euler(0f, rotationAmount, 0f);
        rb.MoveRotation(rb.rotation * turnOffset);
    }
}
