using UnityEngine;

public class Room
{
    private GameObject blockPrefab;
    private Transform parent;

    public Room(GameObject blockPrefab, Transform parent)
    {
        this.blockPrefab = blockPrefab;
        this.parent = parent;
    }

    public void GenerateRoom(LevelGenerator.RoomType type, Vector2 offset, int floorWidth, int floorHeight)
    {
        switch (type)
        {
            case LevelGenerator.RoomType.Normal:
                GenerateRectangle(floorWidth, floorHeight, offset);
                break;
            case LevelGenerator.RoomType.Long:
                GenerateRectangle(floorWidth * 2, floorHeight, offset);
                break;
            case LevelGenerator.RoomType.Big:
                GenerateRectangle(floorWidth * 2, floorHeight * 2, offset);
                break;
            case LevelGenerator.RoomType.LShaped:
                GenerateLShaped(floorWidth, floorHeight, offset);
                break;
        }
    }

    private void GenerateRectangle(int width, int height, Vector2 offset)
    {
        GameObject roomParent = new GameObject("Room", typeof(Transform));
        roomParent.transform.parent = parent;
        roomParent.transform.position = new Vector3(offset.x, 0, offset.y);

        float xOffset = (width - 1) / 2f;
        float zOffset = (height - 1) / 2f;

        for (int x = 0; x < width; x++)
            for (int z = 0; z < height; z++)
            {
                Vector3 pos = new Vector3(offset.x + x - xOffset, -1f, offset.y + z - zOffset);
                Object.Instantiate(blockPrefab, pos, Quaternion.identity, roomParent.transform);
            }
    }

    private void GenerateLShaped(int floorWidth, int floorHeight, Vector2 offset)
    {
        GameObject roomParent = new GameObject("Room", typeof(Transform));
        roomParent.transform.parent = parent;
        roomParent.transform.position = new Vector3(offset.x, 0, offset.y);

        // Horizontal part (bottom)
        for (int x = 0; x < floorWidth * 2; x++)
            for (int z = 0; z < floorHeight; z++)
                Object.Instantiate(blockPrefab, new Vector3(offset.x + x, -1f, offset.y + z), Quaternion.identity, roomParent.transform);

        // Vertical part (left), skipping overlap
        for (int x = 0; x < floorWidth; x++)
            for (int z = floorHeight; z < floorHeight * 2; z++)
                Object.Instantiate(blockPrefab, new Vector3(offset.x + x, -1f, offset.y + z), Quaternion.identity, roomParent.transform);
    }
}
