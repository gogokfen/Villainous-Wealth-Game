using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WeaponBase : MonoBehaviour
{
    [HideInInspector]
    public CharacterControl.PlayerTypes playerID;
    public enum damageTypes
    {
        melee,
        destructableProjectile,
        indestructableProjectile,
        bounceOffProjectile
    }
    public damageTypes damageType;

    public int damage;



    //public int damageType;
    /* 
     * 0 melee (fists, swords)
     * 1 destructable projectile (bullets)
     * 2 indestructable projectile(lazers,boomerangs)


    */
}
