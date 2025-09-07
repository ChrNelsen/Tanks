using UnityEngine;

public class Enemy : VehicleBase
{
    private Transform player;

    protected override void Start()
    {
        // Run the VehicleBase Start() so health and shared stats are initialized
        base.Start();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
    }
}