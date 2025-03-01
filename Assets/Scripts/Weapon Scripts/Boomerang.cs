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
    [HideInInspector] public bool canThrow = true;

    [SerializeField] GameObject boomerang;
    [SerializeField] GameObject boomerangGFX;
    private GameObject boomerangReference;
    private float windup;

    [SerializeField] LayerMask boomerangPickupMask;
    private Collider[] boomrangSearch;
    private float catchCD;

    [SerializeField] Slider windUpSlider;

    private int attackState = 0;
    private bool chargeSFX;

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

        if (boomerangReference!= null)
        {
            Destroy(boomerangReference);

            catchCD = 0;
            boomerangReference = null;

            canThrow = true;
        }
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
        if (attackState == 1 && canThrow)
        {
            windup += Time.deltaTime;

            windUpSlider.gameObject.SetActive(true);
            windUpSlider.value = Mathf.InverseLerp(0, 35, windup * 65f);

            charging = true;
            if (!chargeSFX)
            {
                SoundManager.singleton.PlayClip($"{Leaderboard.singleton.GetPlayerName(playerID)}Charge", transform.position, 1f, true, true);
                chargeSFX = true;
            }
        }
        if (attackState == -1 && canThrow)
        {
            chargeSFX = false;
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
            tempBoomrang.GetComponent<BoomerangShot>().flySpeed = 5 + windup * 65f; //15 + windup * 37.5f
            if ((5+windup*65f)>40)
                tempBoomrang.GetComponent<BoomerangShot>().flySpeed = 40;

            boomerangReference = tempBoomrang;

            windup = 0;

            SoundManager.singleton.PlayClip($"{Leaderboard.singleton.GetPlayerName(playerID)}Throw", transform.position, 1f, true, true);
            
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
                        if (boomrangSearch[i].GetComponent<WeaponBase>().playerID == playerID && boomrangSearch[i].name == "Boomerang Shot")
                        {
                            catchCD = 0;
                            boomerangReference = null;
                            Destroy(boomrangSearch[i].gameObject);

                            canThrow = true;
                            boomerangGFX.SetActive(true);

                            SoundManager.singleton.PlayClip("BoomerangCatch", transform.position, 0.2f, true, true);
                        }
                    }
                }
            }
        }    
    }
}
