using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boomerang : WeaponBase
{

    private bool canThrow = true;
    [SerializeField] GameObject boomerang;
    [SerializeField] GameObject boomerangGFX;
    private float windup;

    [SerializeField] LayerMask boomerangPickupMask;
    Collider[] boomrangSearch;
    private float catchCD;

    private void Start()
    {
        damageType = damageTypes.IndestructableProjectile;
    }


    void Update()
    {
        if (Input.GetMouseButton(0) && canThrow)
        {
            windup += Time.deltaTime;
        }
        if (Input.GetMouseButtonUp(0) && canThrow)
        {
            boomerangGFX.SetActive(false);
            canThrow = false;
            GameObject tempBoomrang = Instantiate(boomerang, transform.position, transform.rotation);
            tempBoomrang.GetComponent<WeaponBase>().playerID = playerID;
            tempBoomrang.GetComponent<WeaponBase>().damage = damage;
            tempBoomrang.GetComponent<WeaponBase>().damageType = damageType;

            tempBoomrang.GetComponent<BoomerangShot>().lookAtTarget = transform;
            tempBoomrang.GetComponent<BoomerangShot>().flySpeed = 15 + windup * 5;
            if ((15+windup*5)>40)
                tempBoomrang.GetComponent<BoomerangShot>().flySpeed = 40;
            windup = 0;
        }
        if (!canThrow)
        {
            catchCD += Time.deltaTime;
            if (catchCD>=1)
            {
                boomrangSearch = Physics.OverlapSphere(transform.position, 1f, boomerangPickupMask);
                if (boomrangSearch.Length > 0)
                {
                    catchCD = 0;
                    Destroy(boomrangSearch[0].gameObject);

                    canThrow = true;
                    boomerangGFX.SetActive(true);
                }
            }
        }
    }
}
