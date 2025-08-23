using UnityEngine;

public abstract class RoomBase
{
    protected GameObject blockPrefab;
    protected Transform parent;
    public int Width { get; protected set; }
    public int Height { get; protected set; }
    public Vector3 Center { get; protected set; }

    public RoomBase(GameObject BlockPrefab, Transform Parent, int width, int height)
    {
        blockPrefab = BlockPrefab;
        parent = Parent;
        Width = width;
        Height = height;
    }

    // Each room type must implement its own Generate logic
    public abstract GameObject Generate(Vector2 offset);

    // Shared rectangle generation for all rooms
    protected GameObject GenerateRectangle(Vector2 offset, string roomName = "Room")
    {
        GameObject roomParent = new GameObject(roomName);
        roomParent.transform.parent = parent;
        roomParent.transform.position = new Vector3(offset.x, 0, offset.y);

        float xOffset = (Width - 1) / 2f;
        float zOffset = (Height - 1) / 2f;

        // Set Center for this room
        Center = roomParent.transform.position;

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
}