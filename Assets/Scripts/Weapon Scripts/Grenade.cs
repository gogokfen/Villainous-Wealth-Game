using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Grenade : WeaponBase
{
    [HideInInspector] public bool charging = false;
    [HideInInspector] public bool releasing = false;

    [SerializeField] GameObject GrenadeGFX;
    [SerializeField] float maxAttackCooldown = 4f;
    private float attackCooldown;
    [SerializeField] GameObject grenade;
    float windup;

    [SerializeField] Slider windUpSlider;

    int attackState = 0;

    private void Start()
    {
        damageType = damageTypes.bounceOffProjectile;
    }

    private void OnEnable()
    {
        attackState = 0;
        GrenadeGFX.SetActive(true);
    }

    private void OnDisable()
    {
        GrenadeGFX.SetActive(false);
    }

    public void Shot(InputAction.CallbackContext context)
    {
        if (context.started && attackCooldown <= 0)
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
        if (attackCooldown > 0)
        {
            attackCooldown -= Time.deltaTime;
            if (attackCooldown<=0)
            {
                GrenadeGFX.SetActive(true);
            }
        }
            

        if (attackState ==1 && attackCooldown<=0) 
        {
            windup += Time.deltaTime;

            windUpSlider.gameObject.SetActive(true);
            windUpSlider.value = Mathf.InverseLerp(0, 50,windup *75);

            charging = true;
        }
        if (attackState == -1 && attackCooldown <= 0) //use && attackCooldown <= 0 // Input.GetMouseButtonUp(0)
        {
            attackState = 0;

            windUpSlider.gameObject.SetActive(false);

            GameObject tempGrenade = Instantiate(grenade, transform.position, transform.rotation);
            tempGrenade.GetComponent<WeaponBase>().playerID = playerID;
            tempGrenade.GetComponent<WeaponBase>().damage = damage;
            tempGrenade.GetComponent<WeaponBase>().damageType = damageType;
            attackCooldown = maxAttackCooldown;


            if ((25 + windup * 75f) > 75)
                tempGrenade.GetComponent<GrenadeShot>().throwPower = 75;
            else
                tempGrenade.GetComponent<GrenadeShot>().throwPower = 25 + windup * 75f;

            GrenadeGFX.SetActive(false);

            windup = 0;

            SoundManager.singleton.BombThrow();

            charging = false;
            releasing = true;
        }
    }
}
