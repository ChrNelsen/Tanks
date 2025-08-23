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
    int halfWidth = (floorWidth - 1) / 2;
    int halfHeight = (floorHeight - 1) / 2;

    Vector2 start = from;
    Vector2 end = to;

    // Move start/end to room edges
    if (to.x > from.x) { start.x += halfWidth + 1; end.x -= halfWidth + 1; }
    if (to.x < from.x) { start.x -= halfWidth + 1; end.x += halfWidth + 1; }
    if (to.y > from.y) { start.y += halfHeight + 1; end.y -= halfHeight + 1; }
    if (to.y < from.y) { start.y -= halfHeight + 1; end.y += halfHeight + 1; }

    // Corner point where the L-shape turns
    Vector2 corner = new Vector2(end.x, start.y);

    // Horizontal: stop BEFORE the corner
    int xStep = start.x < corner.x ? 1 : -1;
    for (int x = (int)start.x; x != (int)corner.x; x += xStep)
        Instantiate(blockPrefab, new Vector3(x, -1f, start.y), Quaternion.identity, transform);

    // Vertical: starts AT the corner, goes to end
    int yStep = corner.y < end.y ? 1 : -1;
    for (int y = (int)corner.y; y != (int)end.y + yStep; y += yStep)
        Instantiate(blockPrefab, new Vector3(end.x, -1f, y), Quaternion.identity, transform);
}



    public enum RoomType { Normal, Long, Big, LShaped }
    public enum Direction { Up, Down, Left, Right }
}