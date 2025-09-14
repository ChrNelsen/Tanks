using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;


public class Corridor
{
    private GameObject groundPrefab;
    private GameObject wallPrefab;
    private int floorWidth;
    private int floorHeight;
    private int corridorWidth;
    private Vector2 from;
    private Vector2 to;

    private Transform parent;

    public Corridor(GameObject groundPrefab, GameObject wallPrefab, int floorWidth, int floorHeight, int corridorWidth, Transform parent)
    {
        this.groundPrefab = groundPrefab;
        this.wallPrefab = wallPrefab;
        this.floorWidth = floorWidth;
        this.floorHeight = floorHeight;
        this.corridorWidth = corridorWidth;
        this.parent = parent;
    }

    public void Generate(Vector2 from, Vector2 to)
    {
        this.from = from;
        this.to = to;

        float halfWidth = (floorWidth - 1) / 2f + 1;
        float halfHeight = (floorHeight - 1) / 2f + 1;
        float halfCorridor = (corridorWidth - 1) / 2f + 1;

        // Create parent object for this corridor
        GameObject corridorParent = new GameObject("Corridor");
        corridorParent.transform.parent = parent;

        // Horizontal corridor
        if (from.y == to.y)
        {
            float startX = Mathf.Min(from.x, to.x) + halfWidth;
            float endX = Mathf.Max(from.x, to.x) - halfWidth;

            for (float x = startX; x <= endX; x++)
            {
                // Center ground
                Object.Instantiate(groundPrefab, new Vector3(x, -1f, from.y), Quaternion.identity, corridorParent.transform);

                // Corridor width
                for (int w = 1; w <= (corridorWidth - 1) / 2; w++)
                {
                    Object.Instantiate(groundPrefab, new Vector3(x, -1f, from.y + w), Quaternion.identity, corridorParent.transform);
                    Object.Instantiate(groundPrefab, new Vector3(x, -1f, from.y - w), Quaternion.identity, corridorParent.transform);
                }

                // Corridor borders
                Object.Instantiate(wallPrefab, new Vector3(x, 0f, from.y + halfCorridor), Quaternion.identity, corridorParent.transform);
                Object.Instantiate(wallPrefab, new Vector3(x, 0f, from.y - halfCorridor), Quaternion.identity, corridorParent.transform);
            }

            CreateTrigger(corridorParent, new Vector3((startX + endX) / 2f, 0f, from.y), new Vector3(1, 5f, corridorWidth));
        }

        // Vertical corridor (same X)
        else if (from.x == to.x)
        {
            float startY = from.y < to.y ? from.y + halfHeight : to.y + halfHeight;
            float endY = from.y < to.y ? to.y - halfHeight : from.y - halfHeight;

            for (float y = startY; y <= endY; y++)
            {
                // Center ground
                Object.Instantiate(groundPrefab, new Vector3(from.x, -1f, y), Quaternion.identity, corridorParent.transform);

                // Corridor width
                for (int w = 1; w <= (corridorWidth - 1) / 2; w++)
                {
                    Object.Instantiate(groundPrefab, new Vector3(from.x + w, -1f, y), Quaternion.identity, corridorParent.transform);
                    Object.Instantiate(groundPrefab, new Vector3(from.x - w, -1f, y), Quaternion.identity, corridorParent.transform);
                }

                // Corridor borders
                Object.Instantiate(wallPrefab, new Vector3(from.x + halfCorridor, 0f, y), Quaternion.identity, corridorParent.transform);
                Object.Instantiate(wallPrefab, new Vector3(from.x - halfCorridor, 0f, y), Quaternion.identity, corridorParent.transform);
            }

            CreateTrigger(corridorParent, new Vector3(from.x, 0f, (startY + endY) / 2f), new Vector3(corridorWidth, 5f, 1));
        }
    }

    private void CreateTrigger(GameObject corridorParent, Vector3 center, Vector3 size)
    {
        // Create a new empty GameObject to hold the trigger
        GameObject triggerGO = new GameObject("RoomTrigger");

        // Set its parent and position
        triggerGO.transform.parent = corridorParent.transform;
        triggerGO.transform.position = center;

        // Add the BoxCollider and mark it as a trigger
        BoxCollider box = triggerGO.AddComponent<BoxCollider>();
        box.isTrigger = true;
        box.size = size;

        // Add the RoomTrigger script
        triggerGO.AddComponent<RoomTrigger>();
        triggerGO.GetComponent<RoomTrigger>().roomCenterA = from;
        triggerGO.GetComponent<RoomTrigger>().roomCenterB = to;
    }
}



