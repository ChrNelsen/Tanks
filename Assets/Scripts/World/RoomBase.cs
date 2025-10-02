using UnityEngine;
using System.Collections.Generic;

public abstract class RoomBase
{
    protected GameObject blockPrefab;   // Prefab used to generate the floor/wall blocks
    protected Transform parent;         // Parent transform to keep hierarchy clean

    // Room size in blocks
    public int Width { get; protected set; }
    public int Height { get; protected set; }

    public Vector3 Center { get; protected set; }           // Center of room in unity space used for room placement
    public Vector2Int GridPosition { get; private set; }    // Logical grid position Used for adjacency checks, graph traversal, etc.
    public List<RoomBase> Neighbors { get; private set; }   // connected rooms (neighbors in the procedural graph)

    public RoomBase(GameObject BlockPrefab, Transform Parent, int width, int height)
    {
        blockPrefab = BlockPrefab;
        parent = Parent;
        Width = width;
        Height = height;
        Neighbors = new List<RoomBase>();
    }

    public void Connect(RoomBase other)
    {
        if (!Neighbors.Contains(other))
        {
            Neighbors.Add(other);
            other.Neighbors.Add(this); // ensure both rooms know about the connection
        }
    }

    // Each room type must implement its own Generate logic
    public abstract GameObject Generate(Vector2 offset);

    // Shared rectangle generation for all rooms.  Instantiateces room in unity and sets room center
    protected GameObject GenerateRectangle(Vector2 offset, string roomName = "Room")
    {
        GameObject roomParent = new GameObject(roomName);
        roomParent.transform.parent = parent;
        roomParent.transform.position = new Vector3(offset.x, 0, offset.y);

        float xOffset = (Width - 1) / 2f;
        float zOffset = (Height - 1) / 2f;

        // Set Center for this room
        Center = roomParent.transform.position;

        // Instantiate the blocks to fill the room
        for (int x = 0; x < Width; x++)
        {
            for (int z = 0; z < Height; z++)
            {
                Vector3 pos = new Vector3(offset.x + x - xOffset, -1f, offset.y + z - zOffset);
                Object.Instantiate(blockPrefab, pos, Quaternion.identity, roomParent.transform);
            }
        }

        return roomParent;
    }

    public bool HasNeighbor(Direction dir)
    {
        foreach (RoomBase neighbor in Neighbors)
        {
            Vector2Int delta = neighbor.GridPosition - this.GridPosition; // relative position

            switch (dir)
            {
                case Direction.Up:
                    if (delta == new Vector2(0, 1)) return true;
                    break;
                case Direction.Down:
                    if (delta == new Vector2(0, -1)) return true;
                    break;
                case Direction.Left:
                    if (delta == new Vector2(-1, 0)) return true;
                    break;
                case Direction.Right:
                    if (delta == new Vector2(1, 0)) return true;
                    break;
            }
        }

        return false; // no neighbor in that direction
    }
    public void SetGridPostion(Vector2Int vector2)
    {
        GridPosition = vector2;
    }
    
}