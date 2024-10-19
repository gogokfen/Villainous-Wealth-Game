using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Gun : WeaponBase
{
    [SerializeField] float maxAttackCooldown = 0.25f;
    private float attackCooldown;
    [SerializeField] GameObject bullet;
    bool use;

    private void Start()
    {
        //playerID = CharacterControl.PlayerTypes.Red;
        //damage = 0;
        //damageType = damageTypes.destructableProjectile; //also used for the lazer gun
    }

    public void Shot(InputAction.CallbackContext context)
    {
        use = context.action.triggered;
    }

    void Update()
    {
        if (attackCooldown > 0)
            attackCooldown -= Time.deltaTime;
        if (use && attackCooldown<=0)
        {
            GameObject tempBullet = Instantiate(bullet,transform.position,transform.rotation);
            tempBullet.GetComponent<WeaponBase>().playerID = playerID;
            tempBullet.GetComponent<WeaponBase>().damage = damage;
            tempBullet.GetComponent<WeaponBase>().damageType = damageType;
            attackCooldown = maxAttackCooldown;
        }
    }
}
