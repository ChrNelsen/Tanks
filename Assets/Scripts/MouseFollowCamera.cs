using UnityEngine;
using System.Collections;

public class MouseFollowCamera : MonoBehaviour
{
    public Transform player;
    public float yOffset = 15f;           // Height above player
    public float zOffset = -10f;          // Distance behind player
    public float followSpeed = 5f;
    public float mouseInfluence = 0.5f;   // How much the mouse moves the camera
    public Vector2 roomMin;
    public Vector2 roomMax;
    public float borderPadding = 2f;
    public float tiltAngle = 45f;         // Fixed top-down tilt

    private Quaternion fixedRotation;
    private bool isMovingToRoom;
    private Vector2 roomCenter = new Vector2(0, 0);

    // Cached clamp values
    private float minX, maxX, minZ, maxZ;

    void Start()
    {
        // Set rotation once and keep it fixed
        fixedRotation = Quaternion.Euler(tiltAngle, 0, 0);
        transform.rotation = fixedRotation;
        UpdateClampValues(); // Precompute clamp limits
    }

    void LateUpdate()
    {
        if (!player) return;
        if (isMovingToRoom) return;

        // Base camera position behind player
        Vector3 basePos = player.position + new Vector3(0, yOffset, zOffset);

        // Get mouse world position at player's height
        Vector3 mouseScreen = Input.mousePosition;
        mouseScreen.z = Camera.main.WorldToScreenPoint(player.position).z;
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(mouseScreen);

        // Mouse influence
        Vector3 offset = (mouseWorld - player.position) * mouseInfluence;
        offset.y = 0; // Don't change height

        // Apply offset
        Vector3 desiredPos = basePos + new Vector3(offset.x, 0, offset.z);

        // Clamp to the room as long as camera is not shifting to a new one
        if (!isMovingToRoom)
        {
            desiredPos.x = Mathf.Clamp(desiredPos.x, minX, maxX);
            desiredPos.z = Mathf.Clamp(desiredPos.z, minZ, maxZ);
        }

        // Smoothly move camera
        transform.position = Vector3.Lerp(transform.position, desiredPos, followSpeed * Time.deltaTime);

        // Keep rotation fixed
        transform.rotation = fixedRotation;
    }

    private void UpdateClampValues()
    {
        minX = roomMin.x + roomCenter.x + borderPadding;
        maxX = roomMax.x + roomCenter.x - borderPadding;
        minZ = roomMin.y + roomCenter.y + borderPadding;
        maxZ = roomMax.y + roomCenter.y - borderPadding;
    }

    public void MoveToRoom(Vector2 center)
    {
        // Update our currenct roomCenter
        roomCenter = center;
        Vector3 targetPosition = new Vector3(center.x, player.position.y + yOffset, center.y + zOffset);
        StopAllCoroutines();
        StartCoroutine(MoveCameraCoroutine(targetPosition));
    }

    private IEnumerator MoveCameraCoroutine(Vector3 targetPos)
    {
        isMovingToRoom = true; // Disable clamping
        UpdateClampValues();

        float duration = 5f;
        Vector3 startPos = transform.position;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0, 1, elapsed / duration);
            transform.position = Vector3.Lerp(startPos, targetPos, t);
            yield return null;
        }

        transform.position = targetPos;
        isMovingToRoom = false; // Re-enable clamping
    }
}
