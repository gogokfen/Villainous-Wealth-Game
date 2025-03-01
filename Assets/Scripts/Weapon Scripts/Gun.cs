using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Gun : WeaponBase
{
    [SerializeField] GameObject gunGFX;
    [SerializeField] GameObject bullet;
    [SerializeField] float maxAttackCooldown = 0.25f;
    [SerializeField] bool lazerGun;

    private float attackCooldown;
    private bool use;

    private void OnEnable()
    {
        gunGFX.SetActive(true);
    }

    private void OnDisable()
    {
        gunGFX.SetActive(false);
    }
    public void Shot(InputAction.CallbackContext context)
    {
        use = context.action.triggered;
    }

    void Update()
    {
        if (attackCooldown > 0)
            attackCooldown -= Time.deltaTime;
        if (use && attackCooldown<=0)
        {
            GameObject tempBullet = Instantiate(bullet,transform.position,transform.rotation);
            tempBullet.GetComponent<WeaponBase>().playerID = playerID;
            tempBullet.GetComponent<WeaponBase>().damage = damage;
            tempBullet.GetComponent<WeaponBase>().damageType = damageType;
            attackCooldown = maxAttackCooldown;

            SoundManager.singleton.PlayClip("GunShot", transform.position, 0.125f, true, true);
        }
    }
}
