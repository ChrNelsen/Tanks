using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CubeController : MonoBehaviour
{
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float rotateSpeed = 120f;
    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        HandleMovement();
        HandleRotation();
    }

    private void HandleMovement()
    {
        float moveInput = Input.GetAxis("Vertical");
        Vector3 moveDirection = transform.forward * moveInput * moveSpeed * Time.fixedDeltaTime;

        // Check if there’s an obstacle in the movement direction
        bool isBlocked = Physics.CapsuleCast(
            point1: transform.position,
            point2: transform.position + Vector3.up * 1f,
            radius: 0.5f,
            direction: moveDirection.normalized,
            maxDistance: moveDirection.magnitude
        );

        if (!isBlocked)
        {
            rb.MovePosition(rb.position + moveDirection);
        }
    }

    private void HandleRotation()
    {
        float turnInput = Input.GetAxis("Horizontal");
        float rotationAmount = turnInput * rotateSpeed * Time.fixedDeltaTime;
        Quaternion turnOffset = Quaternion.Euler(0f, rotationAmount, 0f);
        rb.MoveRotation(rb.rotation * turnOffset);
    }
}