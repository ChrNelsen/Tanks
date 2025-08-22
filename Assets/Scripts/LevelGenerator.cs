using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class LevelGenerator : MonoBehaviour
{
    [SerializeField] private GameObject blockPrefab;
    [SerializeField] private int width = 22;
    [SerializeField] private int height = 16;
    private Dictionary<RoomType, Vector2Int[]> roomShapes;

    private void Start()
    {
        // Define room shapes in grid coords
        roomShapes = new Dictionary<RoomType, Vector2Int[]>
        {
            { RoomType.Normal, new Vector2Int[] { new Vector2Int(0, 0) } },
        };

        float xOffset = (width - 1) / 2f;
        float zOffset = (height - 1) / 2f;
        //GenerateRoom(new Vector2Int(0, 0), RoomType.Big, xOffset, zOffset);
        GenerateRoom(new Vector2Int(0, 0), RoomType.LShaped, xOffset, zOffset);

    }
    private void GenerateRoom(Vector2Int startPos, RoomType type, float xOffset, float zOffset)
    {
        if (type == RoomType.Normal)
        {

            for (int x = 0; x < width; x++)
            {
                for (int z = 0; z < height; z++)
                {
                    Vector3 position = new Vector3(x - xOffset, -1, z - zOffset);

                    // Raise border blocks to 0 height, ground blocks stay at -1
                    float yPos = IsBorder(x, z) ? 0f : position.y;
                    Instantiate(blockPrefab, new Vector3(position.x, yPos, position.z), Quaternion.identity);
                }
            }
        }

        else if (type == RoomType.Long)
        {
            // Long room: double width, same height
            int longWidth = width * 2;

            for (int x = 0; x < longWidth; x++)
            {
                for (int z = 0; z < height; z++)
                {
                    Vector3 position = new Vector3(x - (longWidth - 1) / 2f, -1, z - zOffset);
                    float yPos = (x == 0 || x == longWidth - 1 || z == 0 || z == height - 1) ? 0f : position.y;
                    Instantiate(blockPrefab, new Vector3(position.x, yPos, position.z), Quaternion.identity);
                }
            }
        }

        else if (type == RoomType.Big)
        {
            // Big room: double width and double height
            int bigWidth = width * 2;
            int bigHeight = height * 2;

            for (int x = 0; x < bigWidth; x++)
            {
                for (int z = 0; z < bigHeight; z++)
                {
                    Vector3 position = new Vector3(x - (bigWidth - 1) / 2f, -1, z - (bigHeight - 1) / 2f);
                    float yPos = (x == 0 || x == bigWidth - 1 || z == 0 || z == bigHeight - 1) ? 0f : position.y;
                    Instantiate(blockPrefab, new Vector3(position.x, yPos, position.z), Quaternion.identity);
                }
            }
        }

else if (type == RoomType.LShaped)
{
    // L-shaped room: composed of 3 normal rooms (width x height each)
    int normalWidth = width;
    int normalHeight = height;

    // Loop through total L room bounds
    for (int x = 0; x < normalWidth * 2; x++)         // width = 2 normals
    {
        for (int z = 0; z < normalHeight * 2; z++)    // height = 2 normals
        {
            bool skip = (x >= normalWidth && z >= normalHeight); // top-right quarter is empty

            Vector3 position = new Vector3(x - (normalWidth * 2 - 1) / 2f, -1, z - (normalHeight * 2 - 1) / 2f);

            // Outer walls + inner walls along the cutout corner
            bool isBorder = (x == 0 || x == normalWidth * 2 - 1 || z == 0 || z == normalHeight * 2 - 1) || 
                            (skip && (x == normalWidth || z == normalHeight));

            float yPos = isBorder ? 0f : position.y;

            if (!skip || isBorder) // keep walls in cutout
            {
                Instantiate(blockPrefab, new Vector3(position.x, yPos, position.z), Quaternion.identity);
            }
        }
    }
}
    }

    private bool IsBorder(int x, int z)
    {
        return x == 0 || x == width - 1 || z == 0 || z == height - 1;
    }

    public enum RoomType
    {
        Normal,
        Long,
        Big,
        LShaped
    }
}