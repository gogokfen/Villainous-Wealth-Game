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
    [SerializeField] GameObject rangeCenter;
    LineRenderer LR;

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

            float throwPowerCalc = 5 + windup*90f; //originally 125
            if (throwPowerCalc > 75)
                throwPowerCalc = 75;

            rangeIndicator.transform.position = transform.position + transform.forward + transform.forward * Mathf.Lerp(0,30,(windUpSlider.value *  (1 - (0.75f * Mathf.InverseLerp(75, 5, throwPowerCalc))))); // I CAN"T BELIEVE THIS WORKS

            LR.enabled = true;
            LR.SetPosition(0, transform.position);
            Vector3 airPos = (rangeCenter.transform.position + transform.position) / 2f;
            airPos.y = Vector3.Distance(rangeCenter.transform.position, transform.position) / 4.5f;
            //Vector3 airPos = new Vector3((  transform.position.x - rangeIndicator.transform.position.x) /2, 7.5f, ( transform.position.z - rangeIndicator.transform.position.z) / 2);
            LR.SetPosition(1, airPos);
            LR.SetPosition(2, rangeCenter.transform.position);

            windup += Time.deltaTime;

            windUpSlider.gameObject.SetActive(true);
            windUpSlider.value = Mathf.InverseLerp(0, 70,windup *90); //(0, 50,windup *75) //latest ver is 125

            charging = true;
        }
        if (attackState == -1 && attackCooldown <= 0) //use && attackCooldown <= 0 // Input.GetMouseButtonUp(0)
        {
            LR.enabled = false;

            attackState = 0;

            windUpSlider.gameObject.SetActive(false);

            GameObject tempGrenade = Instantiate(grenade, transform.position, transform.rotation);
            tempGrenade.GetComponent<WeaponBase>().playerID = playerID;
            tempGrenade.GetComponent<WeaponBase>().damage = damage;
            tempGrenade.GetComponent<WeaponBase>().damageType = damageType;
            attackCooldown = maxAttackCooldown;


            if ((5 + windup * 90f) > 75) //latest 125
                tempGrenade.GetComponent<GrenadeShot>().throwPower = 75;
            else
                tempGrenade.GetComponent<GrenadeShot>().throwPower = 5 + windup * 90f; //25 + windup * 75f; //125

            if (ExtraBounceUpgrade)
                tempGrenade.GetComponent<GrenadeShot>().ExtraBounceUpgrade = true;

            GrenadeGFX.SetActive(false);

            windup = 0;

            //SoundManager.singleton.BombThrow(transform.position);
            SoundManager.singleton.PlayClip("GrenadeThrow", transform.position, 1f, false, true);

            charging = false;
            releasing = true;
        }
    }
}
