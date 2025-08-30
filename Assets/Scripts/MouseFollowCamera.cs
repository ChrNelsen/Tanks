using UnityEngine;

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

    void Start()
    {
        // Set rotation once and keep it fixed
        fixedRotation = Quaternion.Euler(tiltAngle, 0, 0);
        transform.rotation = fixedRotation;
    }

    void LateUpdate()
    {
        if (!player) return;

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

        // Clamp to room bounds
        float minX = roomMin.x + borderPadding;
        float maxX = roomMax.x - borderPadding;
        float minZ = roomMin.y + borderPadding;
        float maxZ = roomMax.y - borderPadding;

        desiredPos.x = Mathf.Clamp(desiredPos.x, minX, maxX);
        desiredPos.z = Mathf.Clamp(desiredPos.z, minZ, maxZ);

        // Smoothly move camera
        transform.position = Vector3.Lerp(transform.position, desiredPos, followSpeed * Time.deltaTime);

        // Keep rotation fixed
        transform.rotation = fixedRotation;
    }
}
