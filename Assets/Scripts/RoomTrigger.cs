using NUnit.Framework.Internal;
using UnityEngine;

public class RoomTrigger : MonoBehaviour
{
    private MouseFollowCamera cameraController;

    public Vector2 roomCenterA;
    public Vector2 roomCenterB;

    private void Start()
    {
        // Find the camera using the MainCamera tag
        Camera mainCam = Camera.main; // This automatically finds the camera tagged as MainCamera
        if (mainCam != null)
        {
            cameraController = mainCam.GetComponent<MouseFollowCamera>();
        }

        if (cameraController == null)
            Debug.LogWarning("MouseFollowCamera not found on MainCamera!");
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && cameraController != null)
        {
            Vector2 playerPos = new Vector2(other.transform.position.x, other.transform.position.z);

            // Decide which room the player is moving into
            float distA = Vector2.Distance(playerPos, roomCenterA);
            float distB = Vector2.Distance(playerPos, roomCenterB);

            if (distA < distB)
            {
                cameraController.MoveToRoom(roomCenterA);
            }
            else
            {
                cameraController.MoveToRoom(roomCenterB);
            }
        }
    }
}