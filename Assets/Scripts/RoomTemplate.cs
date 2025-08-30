using UnityEngine;

[CreateAssetMenu(fileName = "RoomTemplate", menuName = "Level/Room Template")]
public class RoomTemplate : ScriptableObject
{
    public GameObject prefab;        // The room prefab
    public int difficulty = 1;       // Optional
    public RoomType type;            // Enemy, Treasure, Boss, etc.

    public enum RoomType
    {
        Enemy,
        Treasure,
        Boss,
        Secret
    }
}