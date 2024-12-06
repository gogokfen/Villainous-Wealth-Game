using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VInspector;

public class Sword : WeaponBase
{
    [Foldout("Upgrades")]
    public bool WindSlashUpgrade = false;
    [SerializeField] GameObject windSlashProjectile;
    private float windSlashCooldown;
    [HideInInspector] public Quaternion desiredRotation;
    [EndFoldout]


    [SerializeField] GameObject swordGFX;
    BoxCollider bCollider;


    

    private void Awake()
    {
        bCollider = GetComponent<BoxCollider>();
    }

    private void OnEnable()
    {
        swordGFX.SetActive(true);
    }

    private void OnDisable()
    {
        transform.GetComponent<BoxCollider>().enabled = false;
        swordGFX.SetActive(false);
    }

    private void Update()
    {
        transform.position = swordGFX.transform.position;
        transform.rotation = swordGFX.transform.rotation;

        if (WindSlashUpgrade)
        {
            if (bCollider.enabled && Time.time > windSlashCooldown)
            {
                //Instantiate(windSlashProjectile,transform.position,Quaternion.Euler(new Vector3 (0,transform.eulerAngles.y+45,0)));
                Instantiate(windSlashProjectile, transform.position, desiredRotation);
                windSlashCooldown = Time.time + 0.25f;
            }
        }
    }
}
