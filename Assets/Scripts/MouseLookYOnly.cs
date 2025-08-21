using UnityEngine;

public class MouseLookYOnly : MonoBehaviour
{
    public LayerMask groundMask;  // Assign this in the inspector to include only the ground
    public float rotationSpeed = 10f;

    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        // Cast ray only against ground layer
        if (Physics.Raycast(ray, out RaycastHit hit, 100f, groundMask))
        {
            Vector3 targetDirection = hit.point - transform.position;
            targetDirection.y = 0f; // Eliminate vertical rotation

            if (targetDirection != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
            }
        }
    }
}
