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
    private float windup;

    [SerializeField] Slider windUpSlider;
    [SerializeField] Slider reloadSlider;

    private int attackState = 0;

    [SerializeField] GameObject rangeIndicator;
    [SerializeField] GameObject rangeCenter;
    private LineRenderer LR;
    private bool chargeSFX;

    private void Start()
    {
        damageType = damageTypes.bounceOffProjectile;
        LR = GetComponent<LineRenderer>();
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

            float throwPowerCalc = 5 + windup*90f;
            if (throwPowerCalc > 75)
                throwPowerCalc = 75;

            rangeIndicator.transform.position = transform.position + transform.forward + transform.forward * Mathf.Lerp(0,30,(windUpSlider.value *  (1 - (0.75f * Mathf.InverseLerp(75, 5, throwPowerCalc))))); // I CAN"T BELIEVE THIS WORKS

            LR.enabled = true;
            LR.SetPosition(0, transform.position);
            Vector3 airPos = (rangeCenter.transform.position + transform.position) / 2f;
            airPos.y = Vector3.Distance(rangeCenter.transform.position, transform.position) / 4.5f;
            LR.SetPosition(1, airPos);
            LR.SetPosition(2, rangeCenter.transform.position);

            windup += Time.deltaTime;

            windUpSlider.gameObject.SetActive(true);
            windUpSlider.value = Mathf.InverseLerp(0, 70,windup *90);

            charging = true;
            if (!chargeSFX)
            {
                SoundManager.singleton.PlayClip($"{Leaderboard.singleton.GetPlayerName(playerID)}Charge", transform.position, 1f, true, true);
                chargeSFX = true;
            }
        }
        if (attackState == -1 && attackCooldown <= 0)
        {
            LR.enabled = false;

            attackState = 0;

            windUpSlider.gameObject.SetActive(false);

            GameObject tempGrenade = Instantiate(grenade, transform.position, transform.rotation);
            tempGrenade.GetComponent<WeaponBase>().playerID = playerID;
            tempGrenade.GetComponent<WeaponBase>().damage = damage;
            tempGrenade.GetComponent<WeaponBase>().damageType = damageType;
            attackCooldown = maxAttackCooldown;


            if ((5 + windup * 90f) > 75)
                tempGrenade.GetComponent<GrenadeShot>().throwPower = 75;
            else
                tempGrenade.GetComponent<GrenadeShot>().throwPower = 5 + windup * 90f;

            GrenadeGFX.SetActive(false);

            windup = 0;

            SoundManager.singleton.PlayClip($"{Leaderboard.singleton.GetPlayerName(playerID)}Throw", transform.position, 1f, true, true);

            charging = false;
            releasing = true;
        }
    }
}
