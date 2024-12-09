using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GasCloud : WeaponBase
{
    private SphereCollider SC;
    private float tickTimer;

    private void Awake()
    {
        Destroy(gameObject, 5);
        SC = GetComponent<SphereCollider>();
        SC.enabled = false;
        damage = 1;
        damageType = damageTypes.indestructableProjectile;
        tickTimer = Time.time + 0.5f;
    }

    void Update()
    {
        if (SC.enabled)
            SC.enabled = false;
        if (Time.time>=tickTimer)
        {
            tickTimer = Time.time + 0.5f;
            SC.enabled = true;
        }
    }
}
