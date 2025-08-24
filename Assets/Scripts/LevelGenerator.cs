using UnityEngine;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;

public class LevelGenerator : MonoBehaviour
{
    [Header("Room Settings")]
    [SerializeField] private GameObject blockPrefab;
    [SerializeField] private int floorWidth;
    [SerializeField] private int floorHeight;
    [SerializeField] private int roomBorder = 1;
    [SerializeField] private int totalRooms;
    [SerializeField] private int corridorWidth = 1;

    private List<Vector2> spawnedRooms = new List<Vector2>();
    private Dictionary<RoomType, RoomBase> roomTypes;

    private void Awake()
    {
        roomTypes = new Dictionary<RoomType, RoomBase>
        {
            { RoomType.Normal, new RoomNormal(blockPrefab, transform, floorWidth, floorHeight) }
        };
    }

    private void Start()
    {
        Vector2 startPos = Vector2.zero;
        SpawnRoom(RoomType.Normal, startPos);
        spawnedRooms.Add(startPos);

        for (int i = 1; i < totalRooms; i++)
            SpawnNextRoom();
    }

    private GameObject SpawnRoom(RoomType type, Vector2 position)
    {
        return roomTypes[type].Generate(position);
    }


    private void SpawnNextRoom()
    {
        Vector2 baseRoom = spawnedRooms[Random.Range(0, spawnedRooms.Count)];
        Vector2 newPos = GetAdjacentPosition(baseRoom, (Direction)Random.Range(0, 4));

        if (!spawnedRooms.Contains(newPos))
        {
            SpawnRoom(RoomType.Normal, newPos);
            spawnedRooms.Add(newPos);
            ConnectRooms(baseRoom, newPos);
        }
        else
        {
            SpawnNextRoom();
        }
    }

    private Vector2 GetAdjacentPosition(Vector2 currentPos, Direction dir)
    {
        switch (dir)
        {
            case Direction.Up: return currentPos + new Vector2(0, floorHeight + roomBorder);
            case Direction.Down: return currentPos - new Vector2(0, floorHeight + roomBorder);
            case Direction.Left: return currentPos - new Vector2(floorWidth + roomBorder, 0);
            case Direction.Right: return currentPos + new Vector2(floorWidth + roomBorder, 0);
            default: return currentPos;
        }
    }
    private void ConnectRooms(Vector2 from, Vector2 to)
    {
        float halfWidth = (floorWidth - 1) / 2f + 1;
        float halfHeight = (floorHeight - 1) / 2f + 1;

        // Horizontal corridor (same Y)
        if (from.y == to.y)
        {
            float startX = from.x < to.x ? from.x + halfWidth : to.x + halfWidth;
            float endX = from.x < to.x ? to.x - halfWidth : from.x - halfWidth;

            for (float x = startX; x <= endX; x++)
            {
                Instantiate(blockPrefab, new Vector3(x, -1f, from.y), Quaternion.identity, transform);

                for (int w = 1; w <= (corridorWidth - 1) / 2; w++)
                {
                    Instantiate(blockPrefab, new Vector3(x, -1f, from.y + w), Quaternion.identity, transform);
                    Instantiate(blockPrefab, new Vector3(x, -1f, from.y - w), Quaternion.identity, transform);
                }
            }
        }
        // Vertical corridor (same X)
        else if (from.x == to.x)
        {
            float startY = from.y < to.y ? from.y + halfHeight : to.y + halfHeight;
            float endY = from.y < to.y ? to.y - halfHeight : from.y - halfHeight;

            for (float y = startY; y <= endY; y++)
            {
                Instantiate(blockPrefab, new Vector3(from.x, -1f, y), Quaternion.identity, transform);

                for (int w = 1; w <= (corridorWidth - 1) / 2; w++)
                {
                    Instantiate(blockPrefab, new Vector3(from.x + w, -1f, y), Quaternion.identity, transform);
                    Instantiate(blockPrefab, new Vector3(from.x - w, -1f, y), Quaternion.identity, transform);
                }
            }
        }
    }


    public enum RoomType { Normal, Long, Big, LShaped }
    public enum Direction { Up, Down, Left, Right }
}