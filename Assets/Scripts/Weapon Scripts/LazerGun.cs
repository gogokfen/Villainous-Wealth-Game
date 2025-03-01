using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class LazerGun : WeaponBase
{
    [SerializeField] GameObject gunGFX;
    [SerializeField] GameObject bullet;
    [SerializeField] float maxAttackCooldown = 0.25f;

    private LineRenderer LR;

    private RaycastHit rayInfo;
    private LayerMask LM;

    private LayerMask noCharLM;

    private Vector3 boxcastSize = new Vector3(0.5f, 0.5f, 0f); //half extents
    private Collider[] sphereCollides;

    private float attackCooldown;
    private bool use;

    private void Start()
    {
        LR = GetComponent<LineRenderer>();
        LM = 257; //default layer is 1 character is 256 mask
        noCharLM = 1;
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
        if (use && attackCooldown <= 0)
        {
            sphereCollides = Physics.OverlapSphere(transform.position, 0.5f,noCharLM);
            if (sphereCollides.Length>0) //checking if inside a collider already
            {
                for (int i=0;i<sphereCollides.Length;i++)
                {
                    GameObject tempBullet = Instantiate(bullet, sphereCollides[i].transform.position, transform.rotation);
                    tempBullet.GetComponent<WeaponBase>().playerID = playerID;
                    tempBullet.GetComponent<WeaponBase>().damage = damage;
                    tempBullet.GetComponent<WeaponBase>().damageType = damageType;
                }
            }
            else if (Physics.BoxCast(transform.position,boxcastSize,transform.forward,out rayInfo,Quaternion.identity,50,LM))
            {
                LR.enabled = true;
                LR.SetPosition(0, transform.position);
                LR.SetPosition(1, rayInfo.point);

                LR.widthMultiplier = 0.35f;

                GameObject tempBullet = Instantiate(bullet, rayInfo.point, transform.rotation);
                tempBullet.GetComponent<WeaponBase>().playerID = playerID;
                tempBullet.GetComponent<WeaponBase>().damage = damage;
                tempBullet.GetComponent<WeaponBase>().damageType = damageType;
            }
            else
            {
                LR.enabled = true;
                LR.SetPosition(0, transform.position);
                LR.SetPosition(1, transform.position+transform.forward*50);

                LR.widthMultiplier = 0.35f;
            }
            attackCooldown = maxAttackCooldown;
            SoundManager.singleton.PlayClip("LaserGunShot", transform.position, 0.3f, true, true);
        }
        if (LR.widthMultiplier > 0)
            LR.widthMultiplier -= Time.deltaTime*1.5f;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.forward * 50);
    }
}
