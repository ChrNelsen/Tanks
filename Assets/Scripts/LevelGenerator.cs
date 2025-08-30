using UnityEngine;
using System.Collections.Generic;

public class LevelGenerator : MonoBehaviour
{
    [Header("Room Settings")]
    [SerializeField] private GameObject wallPrefab;         // Wall block prefab
    [SerializeField] private GameObject groundPrefab;       // Floor block prefab
    [SerializeField] private int floorWidth;                // Width of each room: X
    [SerializeField] private int floorHeight;               // Height of each room: Z
    [SerializeField] private int roomBorder = 1;            // Gap between rooms
    [SerializeField] private int totalRooms;                // Total number of rooms
    [SerializeField] private int corridorWidth = 1;         // Width of corridors
    [SerializeField] private RoomPopulator roomPopulator;   // Room Populator Script

    // Graph of all spawned rooms (key = grid position)
    private Dictionary<Vector2Int, RoomBase> roomGraph = new Dictionary<Vector2Int, RoomBase>();

    // Available room types
    private Dictionary<RoomType, RoomBase> roomTypes;

    private void Awake()
    {
        // Register room types
        roomTypes = new Dictionary<RoomType, RoomBase>
        {
            { RoomType.Normal, new RoomNormal(groundPrefab, transform, floorWidth, floorHeight) }
        };
    }

    private void Start()
    {
        // Spawn starting room at origin
        GetOrCreateRoom(Vector2Int.zero, RoomType.Normal);

        // Spawn rest of rooms
        for (int i = 1; i < totalRooms; i++)
        {
            SpawnNextRoom();
        }
        ConnectAllRooms();
        AddBordersToAllRooms();


    }

    private void SpawnNextRoom()
    {
        Vector2Int baseRoomPos = GetRandomRoomPosition();
        Vector2Int newPos = GetAdjacentPosition(baseRoomPos, (Direction)Random.Range(0, 4));

        // Only spawn if position is unoccupied
        if (!roomGraph.ContainsKey(newPos))
        {
            GetOrCreateRoom(newPos, RoomType.Normal);
        }
        else
        {
            SpawnNextRoom(); // Retry if that position is taken
        }
    }

    private RoomBase GetOrCreateRoom(Vector2Int position, RoomType type)
    {
        if (!roomGraph.TryGetValue(position, out RoomBase room))
        {
            room = (RoomBase)System.Activator.CreateInstance(roomTypes[type].GetType(), groundPrefab, transform, floorWidth, floorHeight);
            room.Generate(GridToWorld(position));
            room.SetGridPostion(position);
            roomGraph[position] = room;

            // Populate Room with Objects
            roomPopulator.PopulateRoom(room);
        }
        return room;
    }

    private Vector2Int GetRandomRoomPosition()
    {
        var keys = new List<Vector2Int>(roomGraph.Keys);
        return keys[Random.Range(0, keys.Count)];
    }

    private Vector2Int GetAdjacentPosition(Vector2Int currentPos, Direction dir)
    {
        switch (dir)
        {
            case Direction.Up: return currentPos + new Vector2Int(0, 1);
            case Direction.Down: return currentPos - new Vector2Int(0, 1);
            case Direction.Left: return currentPos - new Vector2Int(1, 0);
            case Direction.Right: return currentPos + new Vector2Int(1, 0);
            default: return currentPos;
        }
    }

    private void ConnectRooms(Vector2 from, Vector2 to)
    {
        float halfWidth = (floorWidth - 1) / 2f + 1;
        float halfHeight = (floorHeight - 1) / 2f + 1; // 15 -> 8
        float halfCorridor = (corridorWidth - 1) / 2f + 1;  //5 -> 3

        // Horizontal corridor (same Y)
        if (from.y == to.y)
        {
            float startX = Mathf.Min(from.x, to.x) + halfWidth;
            float endX = Mathf.Max(from.x, to.x) - halfWidth;

            for (float x = startX; x <= endX; x++)
            {
                // Center ground
                Instantiate(groundPrefab, new Vector3(x, -1f, from.y), Quaternion.identity, transform);

                // Corridor width
                for (int w = 1; w <= (corridorWidth - 1) / 2; w++)
                {
                    Instantiate(groundPrefab, new Vector3(x, -1f, from.y + w), Quaternion.identity, transform);
                    Instantiate(groundPrefab, new Vector3(x, -1f, from.y - w), Quaternion.identity, transform);
                }

                // Corridor borders
                Instantiate(wallPrefab, new Vector3(x, 0f, from.y + halfCorridor), Quaternion.identity, transform);
                Instantiate(wallPrefab, new Vector3(x, 0f, from.y - halfCorridor), Quaternion.identity, transform);
            }
        }

        // Vertical corridor (same X)
        else if (from.x == to.x)
        {
            float startY = from.y < to.y ? from.y + halfHeight : to.y + halfHeight;
            float endY = from.y < to.y ? to.y - halfHeight : from.y - halfHeight;

            for (float y = startY; y <= endY; y++)
            {
                // Center ground
                Instantiate(groundPrefab, new Vector3(from.x, -1f, y), Quaternion.identity, transform);

                // Corridor width
                for (int w = 1; w <= (corridorWidth - 1) / 2; w++)
                {
                    Instantiate(groundPrefab, new Vector3(from.x + w, -1f, y), Quaternion.identity, transform);
                    Instantiate(groundPrefab, new Vector3(from.x - w, -1f, y), Quaternion.identity, transform);
                }

                // Corridor borders
                Instantiate(wallPrefab, new Vector3(from.x + halfCorridor, 0f, y), Quaternion.identity, transform);
                Instantiate(wallPrefab, new Vector3(from.x - halfCorridor, 0f, y), Quaternion.identity, transform);
            }
        }
    }

    private void ConnectAllRooms()
    {
        // Loop through all rooms in the graph
        foreach (var kvp in roomGraph)
        {
            Vector2Int gridPos = kvp.Key;      // logical position
            RoomBase room = kvp.Value;

            // Check all four directions
            Vector2Int[] neighborOffsets = new Vector2Int[]
            {
                new Vector2Int(0, 1),   // Up
                new Vector2Int(0, -1),  // Down
                new Vector2Int(1, 0),   // Right
                new Vector2Int(-1, 0)   // Left
            };

            // Offsets for neighbors in logical space
            foreach (Vector2Int offset in neighborOffsets)
            {
                Vector2Int neighborPos = gridPos + offset;

                // Only connect if neighbor exists and they aren't already connected
                if (roomGraph.TryGetValue(neighborPos, out RoomBase neighbor) && !room.Neighbors.Contains(neighbor))
                {
                    // Update the graph connection
                    room.Connect(neighbor);

                    // Convert logical → world for corridor drawing
                    Vector2 fromWorld = GridToWorld(gridPos);
                    Vector2 toWorld = GridToWorld(neighborPos);

                    // Draw the corridor physically
                    ConnectRooms(fromWorld, toWorld);
                }
            }
        }
    }

    private void AddBordersToAllRooms()
    {
        float halfWidth = (floorWidth - 1) / 2f + 1;
        float halfHeight = (floorHeight - 1) / 2f + 1; // 15 -> 8
        float halfCorridor = (corridorWidth - 1) / 2f + 1;  //5 -> 3

        foreach (RoomBase room in roomGraph.Values)
        {
            Vector2 roomWorldPos = GridToWorld(room.GridPosition);

            if (!room.HasNeighbor(Direction.Up))
            {
                for (int i = 1; i <= floorWidth; i++)
                {
                    Instantiate(wallPrefab, new Vector3(roomWorldPos.x - halfWidth + i, 0f, roomWorldPos.y + halfHeight), Quaternion.identity, transform);
                }
            }
            else if (room.HasNeighbor(Direction.Up))
            {
                for (int i = 1; i <= halfWidth - halfCorridor - 1; i++)
                {
                    Instantiate(wallPrefab, new Vector3(roomWorldPos.x - halfWidth + i, 0f, roomWorldPos.y + halfHeight), Quaternion.identity, transform);
                    Instantiate(wallPrefab, new Vector3(roomWorldPos.x + halfWidth - i, 0f, roomWorldPos.y + halfHeight), Quaternion.identity, transform);
                }
            }
            if (!room.HasNeighbor(Direction.Down))
            {
                for (int i = 1; i <= floorWidth; i++)
                {
                    Instantiate(wallPrefab, new Vector3(roomWorldPos.x - halfWidth + i, 0f, roomWorldPos.y - halfHeight), Quaternion.identity, transform);
                }
            }
            else if (room.HasNeighbor(Direction.Down))
            {
                for (int i = 1; i <= halfWidth - halfCorridor - 1; i++)
                {
                    Instantiate(wallPrefab, new Vector3(roomWorldPos.x - halfWidth + i, 0f, roomWorldPos.y - halfHeight), Quaternion.identity, transform);
                    Instantiate(wallPrefab, new Vector3(roomWorldPos.x + halfWidth - i, 0f, roomWorldPos.y - halfHeight), Quaternion.identity, transform);
                }
            }
            if (!room.HasNeighbor(Direction.Left))
            {
                for (int i = 1; i <= floorHeight; i++)
                {
                    Instantiate(wallPrefab, new Vector3(roomWorldPos.x - halfWidth, 0f, roomWorldPos.y - halfHeight + i), Quaternion.identity, transform);
                }
            }
            else if (room.HasNeighbor(Direction.Left))
            {
                for (int i = 1; i <= halfHeight - halfCorridor - 1; i++)
                {
                    Instantiate(wallPrefab, new Vector3(roomWorldPos.x - halfWidth, 0f, roomWorldPos.y - halfHeight + i), Quaternion.identity, transform);
                    Instantiate(wallPrefab, new Vector3(roomWorldPos.x - halfWidth, 0f, roomWorldPos.y + halfHeight - i), Quaternion.identity, transform);
                }
            }
            if (!room.HasNeighbor(Direction.Right))
            {
                for (int i = 1; i <= floorHeight; i++)
                {
                    Instantiate(wallPrefab, new Vector3(roomWorldPos.x + halfWidth, 0f, roomWorldPos.y - halfHeight + i), Quaternion.identity, transform);
                }
            }
            else if (room.HasNeighbor(Direction.Right))
            {
                for (int i = 1; i <= halfHeight - halfCorridor - 1; i++)
                {
                    Instantiate(wallPrefab, new Vector3(roomWorldPos.x + halfWidth, 0f, roomWorldPos.y - halfHeight + i), Quaternion.identity, transform);
                    Instantiate(wallPrefab, new Vector3(roomWorldPos.x + halfWidth, 0f, roomWorldPos.y + halfHeight - i), Quaternion.identity, transform);
                }
            }
        }
    }

    private Vector2 GridToWorld(Vector2Int gridPos)
    {
        float x = gridPos.x * (floorWidth + roomBorder);
        float y = gridPos.y * (floorHeight + roomBorder);
        return new Vector2(x, y);  // Unity world coordinates
    }

    public enum RoomType { Normal, Long, Big, LShaped }
}