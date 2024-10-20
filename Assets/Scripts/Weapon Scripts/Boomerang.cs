using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Boomerang : WeaponBase
{

    private bool canThrow = true;
    [SerializeField] GameObject boomerang;
    [SerializeField] GameObject boomerangGFX;
    private float windup;

    [SerializeField] LayerMask boomerangPickupMask;
    Collider[] boomrangSearch;
    private float catchCD;
    bool use;

    private void Start()
    {
        damageType = damageTypes.bounceOffProjectile;
    }

    public void Thrown(InputAction.CallbackContext context)
    {
        use = context.action.triggered;
    }


    void Update()
    {
        if (Input.GetMouseButton(0)) // use && canThrow
        {
            windup += Time.deltaTime;
        }
        if (use && canThrow) // use && canThrow
        {
            boomerangGFX.SetActive(false);
            canThrow = false;
            GameObject tempBoomrang = Instantiate(boomerang, transform.position, transform.rotation);
            tempBoomrang.name = "Boomerang Shot";
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
                    for (int i=0;i<boomrangSearch.Length;i++)
                    {
                        if (boomrangSearch[i].GetComponent<WeaponBase>().playerID == playerID && boomrangSearch[i].name == "Boomerang Shot")
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
    }
}
