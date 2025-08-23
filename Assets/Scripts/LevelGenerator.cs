using UnityEngine;
using System.Collections.Generic;

public class LevelGenerator : MonoBehaviour
{
    [Header("Room Settings")]
    [SerializeField] private GameObject blockPrefab;
    [SerializeField] private int floorWidth;
    [SerializeField] private int floorHeight;
    [SerializeField] private int roomBorder = 1;
    [SerializeField] private int totalRooms;

    private List<Vector2> spawnedRooms = new List<Vector2>();
    private Dictionary<RoomType, RoomBase> roomTypes;

    private void Awake()
    {
        roomTypes = new Dictionary<RoomType, RoomBase>
        {
            { RoomType.Normal, new RoomNormal(blockPrefab, transform, floorWidth, floorHeight) }
            // { RoomType.LShaped, new LShapedRoom(blockPrefab, transform) },
            // { RoomType.Long, new LongRoom(blockPrefab, transform) }, // You can add LongRoom and BigRoom similarly
            // { RoomType.Big, new BigRoom(blockPrefab, transform) }
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

    public enum RoomType { Normal, Long, Big, LShaped }
    public enum Direction { Up, Down, Left, Right }
}