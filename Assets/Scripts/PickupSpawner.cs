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

    [SerializeField] bool randomWeapon = false;
    [SerializeField] bool orRandomPowerup = false;
    private int numOfWeapons;
    private int numOfPowerups;
    void Start()
    {
        numOfWeapons = System.Enum.GetValues(typeof(CharacterControl.Weapons)).Length;
        numOfPowerups = System.Enum.GetValues(typeof(PickupManager.PowerupTypes)).Length;

        //can't have both enabled

        if (randomWeapon)
            orRandomPowerup = false;

        if (orRandomPowerup)
            randomWeapon = false;
        
        if (randomWeapon)
        {
            weaponPickupToSpawn = (CharacterControl.Weapons)Random.Range(1, numOfWeapons); //8 is excluded && we don't need fists
        }

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
            if (orRandomPowerup)
            {
                powerupPickupToSpawn = (PickupManager.PowerupTypes)Random.Range(0, numOfPowerups);
            }

            PickupManager.singleton.SpawnPowerUp(powerupPickupToSpawn, transform.position, transform);
            noPickup = false;
            respawnTimer = float.MaxValue;
        }
    }
}
