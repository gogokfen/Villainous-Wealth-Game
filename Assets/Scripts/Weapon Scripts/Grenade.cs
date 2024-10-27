using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Grenade : WeaponBase
{
    [SerializeField] float maxAttackCooldown = 4f;
    private float attackCooldown;
    [SerializeField] GameObject grenade;
    float windup;

    [SerializeField] Slider windUpSlider;

    bool use;

    private void Start()
    {
        damageType = damageTypes.bounceOffProjectile;
    }

    public void Shot(InputAction.CallbackContext context)
    {
        use = context.action.triggered;
    }

    void Update()
    {
        if (attackCooldown > 0)
            attackCooldown -= Time.deltaTime;

        if (Input.GetMouseButton(0) && attackCooldown<=0) 
        {
            windup += Time.deltaTime;

            windUpSlider.gameObject.SetActive(true);
            windUpSlider.value = Mathf.InverseLerp(0, 50,windup *75);
        }
        if (Input.GetMouseButtonUp(0) && attackCooldown <= 0) //use && attackCooldown <= 0 // Input.GetMouseButtonUp(0)
        {
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

            windup = 0;
        }
    }
}
