using UnityEngine;

public class CandidatePosition
{
    public Vector3 Position;
    public bool HasLOS;
    public float Score;

    public CandidatePosition(Vector3 position)
    {
        Position = position;
        HasLOS = false;
        Score = 0f;
    }
}