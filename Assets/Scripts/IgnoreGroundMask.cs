using UnityEngine;

public class IgnoreGroundMask : MonoBehaviour
{

    // "Player" layer index and "Ground" layer index
    int playerLayer = LayerMask.NameToLayer("Player");
    int groundLayer = LayerMask.NameToLayer("Ground");
    
    void Start()
    {
        Physics.IgnoreLayerCollision(playerLayer, groundLayer, true);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
