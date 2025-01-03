using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using VInspector;

public class Grenade : WeaponBase
{
    [HideInInspector] public bool charging = false;
    [HideInInspector] public bool releasing = false;

    [Foldout("Upgrades")]
    public bool ExtraBounceUpgrade = false;

    [EndFoldout]

    [SerializeField] GameObject GrenadeGFX;
    [SerializeField] float maxAttackCooldown = 4f;
    private float attackCooldown;
    [SerializeField] GameObject grenade;
    float windup;

    [SerializeField] Slider windUpSlider;
    [SerializeField] Slider reloadSlider;

    int attackState = 0;

    [SerializeField] GameObject rangeIndicator;

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
            rangeIndicator.SetActive(false);

            attackCooldown -= Time.deltaTime;

            reloadSlider.gameObject.SetActive(true);
            reloadSlider.value = Mathf.InverseLerp(maxAttackCooldown, 0, attackCooldown);
            if (attackCooldown<=0)
            {
                GrenadeGFX.SetActive(true);
                reloadSlider.gameObject.SetActive(false);
            }
        }
            

        if (attackState ==1 && attackCooldown<=0) 
        {
            rangeIndicator.SetActive(true);

            float throwPowerCalc = 5 + windup*125f;
            if (throwPowerCalc > 75)
                throwPowerCalc = 75;

            rangeIndicator.transform.position = transform.position + transform.forward + transform.forward * Mathf.Lerp(0,30,(windUpSlider.value *  (1 - (0.75f * Mathf.InverseLerp(75, 5, throwPowerCalc))))); // I CAN"T BELIEVE THIS WORKS

            windup += Time.deltaTime;

            windUpSlider.gameObject.SetActive(true);
            windUpSlider.value = Mathf.InverseLerp(0, 70,windup *125); //(0, 50,windup *75)

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


            if ((5 + windup * 125f) > 75)
                tempGrenade.GetComponent<GrenadeShot>().throwPower = 75;
            else
                tempGrenade.GetComponent<GrenadeShot>().throwPower = 5 + windup * 125f; //25 + windup * 75f;

            if (ExtraBounceUpgrade)
                tempGrenade.GetComponent<GrenadeShot>().ExtraBounceUpgrade = true;

            GrenadeGFX.SetActive(false);

            windup = 0;

            SoundManager.singleton.BombThrow(transform.position);

            charging = false;
            releasing = true;
        }
    }
}
