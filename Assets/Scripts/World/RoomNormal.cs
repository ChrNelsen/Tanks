using UnityEngine;

public class RoomNormal : RoomBase
{
    public RoomNormal(GameObject blockPrefab, Transform parent, int width, int height)
        : base(blockPrefab, parent, width, height) { }
    public override GameObject Generate(Vector2 offset)
    {
        return GenerateRectangle(offset, "NormalRoom: " + GridPosition);
    }
}