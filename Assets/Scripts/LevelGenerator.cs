using UnityEngine;
using System.Collections.Generic;

public class LevelGenerator : MonoBehaviour
{
    [SerializeField] private GameObject blockPrefab;
    [SerializeField] private int floorWidth = 3;
    [SerializeField] private int floorHeight = 6;
    [SerializeField] private int roomBorder = 1;
    [SerializeField] private int totalRooms = 5;

    private List<Vector2> spawnedRooms = new List<Vector2>();
    private Room roomBuilder;

    private void Start()
    {
        roomBuilder = new Room(blockPrefab, transform);

        Vector2 startPos = Vector2.zero;
        roomBuilder.GenerateRoom(RoomType.Normal, startPos, floorWidth, floorHeight);
        spawnedRooms.Add(startPos);

        for (int i = 1; i < totalRooms; i++)
            SpawnNextRoom();
    }

    private void SpawnNextRoom()
    {
        Vector2 baseRoom = spawnedRooms[Random.Range(0, spawnedRooms.Count)];
        Vector2 newPos = GetAdjacentPosition(baseRoom, (Direction)Random.Range(0, 4));

        if (!spawnedRooms.Contains(newPos))
        {
            roomBuilder.GenerateRoom(RoomType.Normal, newPos, floorWidth, floorHeight);
            spawnedRooms.Add(newPos);
        }
        else
        {
            SpawnNextRoom(); // Retry if position is taken
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