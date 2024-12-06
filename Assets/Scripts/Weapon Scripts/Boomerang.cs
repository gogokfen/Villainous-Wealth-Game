using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using VInspector;

public class Boomerang : WeaponBase
{
    [HideInInspector] public bool charging = false;
    [HideInInspector] public bool releasing = false;

    [Foldout("Upgrades")]
    public bool TripleBoomerangUpgrade = false;
    private int catchCount;

    [EndFoldout]

    [SerializeField] GameObject boomerang;
    [SerializeField] GameObject boomerangGFX;
    private bool canThrow = true;
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
        boomerangGFX.SetActive(true);
    }
    private void OnDisable()
    {
        boomerangGFX.SetActive(false);
    }

    public void Thrown(InputAction.CallbackContext context)
    {
        if (context.started && canThrow)
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

            charging = true;
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

            if (TripleBoomerangUpgrade)
            {
                GameObject tempBoomrangL = Instantiate(boomerang, transform.position, Quaternion.Euler(new Vector3(transform.rotation.x, transform.rotation.y + 45, transform.rotation.z)));
                tempBoomrangL.name = "Boomerang ShotL";
                tempBoomrangL.GetComponent<WeaponBase>().playerID = playerID;
                tempBoomrangL.GetComponent<WeaponBase>().damage = damage;
                tempBoomrangL.GetComponent<WeaponBase>().damageType = damageType;

                tempBoomrangL.GetComponent<BoomerangShot>().lookAtTarget = transform;
                tempBoomrangL.GetComponent<BoomerangShot>().flySpeed = 15 + windup * 37.5f;
                if ((15 + windup * 37.5f) > 40)
                    tempBoomrangL.GetComponent<BoomerangShot>().flySpeed = 40;

                GameObject tempBoomrangR = Instantiate(boomerang, transform.position, Quaternion.Euler(new Vector3(transform.rotation.x, transform.rotation.y - 45, transform.rotation.z)));
                tempBoomrangR.name = "Boomerang ShotR";
                tempBoomrangR.GetComponent<WeaponBase>().playerID = playerID;
                tempBoomrangR.GetComponent<WeaponBase>().damage = damage;
                tempBoomrangR.GetComponent<WeaponBase>().damageType = damageType;

                tempBoomrangR.GetComponent<BoomerangShot>().lookAtTarget = transform;
                tempBoomrangR.GetComponent<BoomerangShot>().flySpeed = 15 + windup * 37.5f;
                if ((15 + windup * 37.5f) > 40)
                    tempBoomrangR.GetComponent<BoomerangShot>().flySpeed = 40;
            }

            windup = 0;

            SoundManager.singleton.BoomerangThrow();

            charging = false;
            releasing = true;
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
                        if (TripleBoomerangUpgrade)
                        {
                            if (boomrangSearch[i].GetComponent<WeaponBase>().playerID == playerID && boomrangSearch[i].name == "Boomerang Shot" || boomrangSearch[i].GetComponent<WeaponBase>().playerID == playerID && boomrangSearch[i].name == "Boomerang ShotL" || boomrangSearch[i].GetComponent<WeaponBase>().playerID == playerID && boomrangSearch[i].name == "Boomerang ShotR")
                            {
                                catchCount++;
                                Destroy(boomrangSearch[i].gameObject);
                                SoundManager.singleton.BoomerangCatch();
                                if (catchCount == 3)
                                {
                                    catchCD = 0;
                                    catchCount = 0;

                                    canThrow = true;
                                    boomerangGFX.SetActive(true);
                                }
                            }
                        }
                        else
                        {
                            if (boomrangSearch[i].GetComponent<WeaponBase>().playerID == playerID && boomrangSearch[i].name == "Boomerang Shot")
                            {
                                catchCD = 0;
                                Destroy(boomrangSearch[i].gameObject);

                                canThrow = true;
                                boomerangGFX.SetActive(true);

                                SoundManager.singleton.BoomerangCatch();
                            }
                        }
                    }
                }
            }
        }    
    }
}
