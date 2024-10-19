using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Grenade : WeaponBase
{
    [SerializeField] float maxAttackCooldown = 4f;
    private float attackCooldown;
    [SerializeField] GameObject grenade;
    float windup;

    bool use;

    private void Start()
    {
        damageType = damageTypes.bounceOffProjectile;
    }

    public void Shot(InputAction.CallbackContext context)
    {
        use = context.action.triggered;
    }

    void Update()
    {
        if (attackCooldown > 0)
            attackCooldown -= Time.deltaTime;

        if (Input.GetMouseButton(0)) 
        {
            windup += Time.deltaTime;
        }
        if (use && attackCooldown <= 0) //use && attackCooldown <= 0 // Input.GetMouseButtonUp(0)
        {
            GameObject tempGrenade = Instantiate(grenade, transform.position, transform.rotation);
            tempGrenade.GetComponent<WeaponBase>().playerID = playerID;
            tempGrenade.GetComponent<WeaponBase>().damage = damage;
            tempGrenade.GetComponent<WeaponBase>().damageType = damageType;
            attackCooldown = maxAttackCooldown;


            if ((25 + windup * 7.5f) > 60)
                tempGrenade.GetComponent<GrenadeShot>().throwPower = 60;
            else
                tempGrenade.GetComponent<GrenadeShot>().throwPower = 25 + windup * 7.5f;

            windup = 0;
        }
    }
}
