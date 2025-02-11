using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using VInspector;

public class Gun : WeaponBase
{
    [Foldout("Upgrades")]
    public bool GunRicochetUpgrade = false;
    [EndFoldout]

    [SerializeField] GameObject gunGFX;
    [SerializeField] GameObject bullet;
    [SerializeField] float maxAttackCooldown = 0.25f;
    [SerializeField] bool lazerGun;

    private float attackCooldown;
    bool use;

    private void Start()
    {
        //playerID = CharacterControl.PlayerTypes.Red;
        //damage = 0;
        //damageType = damageTypes.destructableProjectile; //also used for the lazer gun
    }

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

            if (GunRicochetUpgrade)
                tempBullet.GetComponent<GunShot>().GunRicochetUpgrade = true;
            
            if (lazerGun)
                //SoundManager.singleton.LazerGunShot(transform.position);
                SoundManager.singleton.PlayClip("LaserGunShot", transform.position, 0.15f, true, true);
            else
                //SoundManager.singleton.GunShot(transform.position);
                SoundManager.singleton.PlayClip("GunShot", transform.position, 0.125f, true, true);
        }
    }
}
