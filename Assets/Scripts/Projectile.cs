using UnityEngine;

public abstract class Projectile : MonoBehaviour
{
    public float fixedY;
    public float speed;
    void Start()
    {
        fixedY = transform.position.y;
    }
    public abstract void Launch(Vector3 dir);
    public abstract void OnHit(GameObject target);

}