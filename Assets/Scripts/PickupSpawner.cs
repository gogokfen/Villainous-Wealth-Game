using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupSpawner : MonoBehaviour
{
    public CharacterControl.Weapons weaponPickupToSpawn;
    public PickupManager.PowerupTypes powerupPickupToSpawn;

    private float respawnTimer;
    private bool noPickup = false;
    [SerializeField] float respawnCooldown;

    void Start()
    {
            respawnTimer = float.MaxValue;
        if (weaponPickupToSpawn!= CharacterControl.Weapons.Fist)
            PickupManager.singleton.SpawnPowerUp(weaponPickupToSpawn, transform.position, transform);
        else
            PickupManager.singleton.SpawnPowerUp(powerupPickupToSpawn, transform.position, transform);
    }

    void Update()
    {
        if (transform.childCount == 0 && weaponPickupToSpawn == CharacterControl.Weapons.Fist && !noPickup) // in case the powerup was already picked up and needs to respawn,weapons do not respawn
        {
            noPickup = true;
            respawnTimer = Time.time + respawnCooldown;
        }

        if (Time.time>respawnTimer)
        {
            PickupManager.singleton.SpawnPowerUp(powerupPickupToSpawn, transform.position, transform);
            noPickup = false;
            respawnTimer = float.MaxValue;
        }
    }
}
