using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using VInspector;

public class Blunderbuss : WeaponBase
{
    [Foldout("Upgrades")]
    public bool BlunderbussShotUpgrade = false;
    [EndFoldout]

    [SerializeField] GameObject blunderbussGFX;

    [HideInInspector] public bool shoot = false;
    [HideInInspector] public bool reloading = false;
    [HideInInspector] public float holdTime = 0.35f;


    [SerializeField] float maxAttackCooldown = 0.75f;
    private float attackCooldown;
    [SerializeField] GameObject bullet;
    [SerializeField] int maxAmmo = 2;
    private int ammo; 

    [SerializeField] float bulletSpreadAngle;
    [SerializeField] int bulletSpreadAmount;
    private Quaternion startingShotPos;

    private bool weaponActive = false;

    [SerializeField] ParticleSystem gunShotEffect;

    [SerializeField] GameObject ammoVisual;
    [SerializeField] GameObject[] bulletsVisual;

    private void OnEnable()
    {
        ammo = maxAmmo;
        damageType = damageTypes.destructableProjectile;

        weaponActive = true;
        blunderbussGFX.SetActive(true);

    }

    private void Start()
    {
        if (BlunderbussShotUpgrade)
            bulletSpreadAmount = (int)(bulletSpreadAmount * 1.5);
    }

    private void OnDisable()
    {
        weaponActive = false;
        blunderbussGFX.SetActive(false);
    }


    public void Shot(InputAction.CallbackContext context)
    {
        //use = context.action.triggered;

        if (context.phase == InputActionPhase.Performed && weaponActive)
        {
            Shoot();
        }
    }

    private void Shoot()
    {
        if (reloading)
            ammoVisual.SetActive(false);
        else
            ammoVisual.SetActive(true);


        if (attackCooldown <= 0 && ammo != 0)
        {
            ammo--;
            shoot = true; //changed to false from CharacterControl

            gunShotEffect.Play();

            //SoundManager.singleton.BlunderbussShot(transform.position);
            SoundManager.singleton.PlayClip("BlunderbussShot", transform.position, 1f, false, true);

            /////////////////////////V1
            /*
            transform.Rotate(0, -bulletSpreadAngle / 2, 0);
            for (int i=0;i<bulletSpreadAmount;i++)
            {
                GameObject tempBullet = Instantiate(bullet, transform.position, transform.rotation);
                tempBullet.GetComponent<WeaponBase>().playerID = playerID;
                tempBullet.GetComponent<WeaponBase>().damage = damage;
                tempBullet.GetComponent<WeaponBase>().damageType = damageType;

                transform.Rotate(0, bulletSpreadAngle / bulletSpreadAmount, 0);
            }
            transform.Rotate(0, -bulletSpreadAngle / 2, 0);
            */

            /////////////////////////V2
            startingShotPos = transform.rotation;
            for (int i = 0; i < bulletSpreadAmount; i++)
            {
                transform.rotation = startingShotPos;
                transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y+Random.Range(-bulletSpreadAngle/2,bulletSpreadAngle/2), 0);

                GameObject tempBullet = Instantiate(bullet, transform.position, transform.rotation);
                tempBullet.GetComponent<WeaponBase>().playerID = playerID;
                tempBullet.GetComponent<WeaponBase>().damage = damage;
                tempBullet.GetComponent<WeaponBase>().damageType = damageType;
            }
            transform.rotation = startingShotPos;

            attackCooldown = maxAttackCooldown;

            ammoVisual.SetActive(true);

            for (int i =0;i<bulletsVisual.Length;i++) //turning all the bullets visual off then turning on the active ones
            {
                bulletsVisual[i].SetActive(false);
            }

            for (int i=0;i<ammo;i++)
            {
                bulletsVisual[i].SetActive(true);
            }
        }
        else if (ammo == 0)
        {
            reloading = true; //changed to false from CharacterControl
            ammo = maxAmmo;
            attackCooldown = 2 * maxAttackCooldown;

            //SoundManager.singleton.BlunderbussReload(transform.position);
            SoundManager.singleton.PlayClip("BlunderbussReload", transform.position, 1f, false, true);

            for (int i = 0; i < ammo; i++)
            {
                bulletsVisual[i].SetActive(true);
            }

            //Debug.Log("reloading!"); //don't delete this comment
        }
    }

    void Update()
    {
        if (attackCooldown > 0)
            attackCooldown -= Time.deltaTime;
        /*
        if (use && attackCooldown <= 0 && ammo !=0)
        {
            ammo--;
            shoot = true; //changed to false from CharacterControl
            GameObject tempBullet = Instantiate(bullet, transform.position, transform.rotation);
            tempBullet.GetComponent<WeaponBase>().playerID = playerID;
            tempBullet.GetComponent<WeaponBase>().damage = damage;
            tempBullet.GetComponent<WeaponBase>().damageType = damageType;
            attackCooldown = maxAttackCooldown;
        }
        else if (use && ammo == 0)
        {
            reloading = true; //changed to false from CharacterControl
            ammo = maxAmmo;
            attackCooldown = 2 * maxAttackCooldown;
        }
        */
    }
}
