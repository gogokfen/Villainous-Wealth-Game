using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Boomerang : WeaponBase
{

    private bool canThrow = true;
    [SerializeField] GameObject boomerang;
    [SerializeField] GameObject boomerangGFX;
    private float windup;

    [SerializeField] LayerMask boomerangPickupMask;
    Collider[] boomrangSearch;
    private float catchCD;

    [SerializeField] Slider windUpSlider;

    int attackState = 0;

    private void Start()
    {
        damageType = damageTypes.bounceOffProjectile;
    }

    private void OnEnable()
    {
        attackState = 0;
    }

    public void Thrown(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            attackState = 1;
        }
        else if (context.canceled)
        {
            if (attackState == 1)
                attackState = -1;
            else
                attackState = 0;
        }
    }


    void Update()
    {
        if (attackState == 1 && canThrow) // use && canThrow
        {
            windup += Time.deltaTime;

            windUpSlider.gameObject.SetActive(true);
            windUpSlider.value = Mathf.InverseLerp(0, 25, windup * 37.5f);
        }
        if (attackState == -1 && canThrow) // use && canThrow
        {
            attackState = 0;

            windUpSlider.gameObject.SetActive(false);

            boomerangGFX.SetActive(false);
            canThrow = false;
            GameObject tempBoomrang = Instantiate(boomerang, transform.position, transform.rotation);
            tempBoomrang.name = "Boomerang Shot";
            tempBoomrang.GetComponent<WeaponBase>().playerID = playerID;
            tempBoomrang.GetComponent<WeaponBase>().damage = damage;
            tempBoomrang.GetComponent<WeaponBase>().damageType = damageType;

            tempBoomrang.GetComponent<BoomerangShot>().lookAtTarget = transform;
            tempBoomrang.GetComponent<BoomerangShot>().flySpeed = 15 + windup * 37.5f;
            if ((15+windup*37.5f)>40)
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
