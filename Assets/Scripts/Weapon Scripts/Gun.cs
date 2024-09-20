using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : WeaponBase
{
    [SerializeField] float maxAttackCooldown = 0.25f;
    private float attackCooldown;
    [SerializeField] GameObject bullet;

    private void Start()
    {
        //playerID = CharacterControl.PlayerTypes.Red;
        //damage = 0;
        //damageType = damageTypes.destructableProjectile;
    }

    void Update()
    {
        if (attackCooldown > 0)
            attackCooldown -= Time.deltaTime;
        if (Input.GetMouseButton(0) && attackCooldown<=0)
        {
            GameObject tempBullet = Instantiate(bullet,transform.position,transform.rotation);
            tempBullet.GetComponent<WeaponBase>().playerID = playerID;
            tempBullet.GetComponent<WeaponBase>().damage = damage;
            tempBullet.GetComponent<WeaponBase>().damageType = damageType;
            attackCooldown = maxAttackCooldown;
        }
    }
}
