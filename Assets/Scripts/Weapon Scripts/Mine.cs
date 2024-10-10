using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mine : WeaponBase
{
    [SerializeField] GameObject minePrefab;
    [SerializeField] int startingAmmo;
    int uses;

    /*
    void Start()
    {
        uses = startingAmmo;
    }
    */

    private void OnEnable()
    {
        uses = startingAmmo;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (uses>0)
            {
                uses--;
                GameObject tempMine = Instantiate(minePrefab,new Vector3(transform.position.x,0,transform.position.z),Quaternion.identity,null);
                tempMine.GetComponent<WeaponBase>().playerID = playerID;
                tempMine.GetComponent<WeaponBase>().damage = damage;
                tempMine.GetComponent<WeaponBase>().damageType = damageType;

                if (uses<=0) //out of mines, refill and discard weapon
                {
                    uses = startingAmmo;
                    CharacterControl.DiscardWeapon();
                }
            }

        }
    }
}
