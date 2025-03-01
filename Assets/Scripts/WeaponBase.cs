using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WeaponBase : MonoBehaviour
{
    [HideInInspector] public CharacterControl.PlayerTypes playerID;
    public enum damageTypes
    {
        melee,
        destructableProjectile,
        indestructableProjectile,
        bounceOffProjectile,
        grenade,
        zone
    }
    public damageTypes damageType;

    public int damage;
}
