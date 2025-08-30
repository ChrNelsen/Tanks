using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.GameCenter;

public class RoomPopulator : MonoBehaviour
{
    [SerializeField] private List<RoomTemplate> roomTemplates;

    public GameObject PopulateRoom(RoomBase room)
    {
        if (roomTemplates.Count == 0) return null;

        // Pick a random template
        RoomTemplate template = roomTemplates[Random.Range(0, roomTemplates.Count)];

        // Instantiate the prefab at the given position
        GameObject roomInstance = Instantiate(template.prefab, room.Center, Quaternion.identity);

        // 0 = no rotation, 1 = 180 degrees
        int rotations = Random.Range(0, 2); // returns 0 or 1
        roomInstance.transform.Rotate(Vector3.up, rotations * 180f);

        return roomInstance;
    }
}
