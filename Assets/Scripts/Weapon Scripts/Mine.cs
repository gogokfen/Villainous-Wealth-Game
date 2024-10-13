using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Mine : WeaponBase
{
    [SerializeField] GameObject minePrefab;
    [SerializeField] int startingAmmo;
    int uses;
    bool placed;

    /*
    void Start()
    {
        uses = startingAmmo;
    }
    */

    private void OnEnable()
    {
        uses = startingAmmo;
        Debug.Log(startingAmmo);
    }

    public void PlacingMine(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {

            //placed = context.action.triggered;
            PlaceMine();
        }
    }

    private void PlaceMine()
    {
        if (uses > 0)
        {
            uses--;
            GameObject tempMine = Instantiate(minePrefab, new Vector3(transform.position.x, 0, transform.position.z), Quaternion.identity, null);
            tempMine.GetComponent<WeaponBase>().playerID = playerID;
            tempMine.GetComponent<WeaponBase>().damage = damage;
            tempMine.GetComponent<WeaponBase>().damageType = damageType;
            if (uses <= 0) //out of mines, refill and discard weapon
            {
                CharacterControl.DiscardWeapon(playerID);
                //uses = startingAmmo;
            }
        }
    }

    void Update()
    {
        //if (placed)
        //{
        //placed = false;
        // if (uses>0)
        // {
        //     uses--;
        //     GameObject tempMine = Instantiate(minePrefab,new Vector3(transform.position.x,0,transform.position.z),Quaternion.identity,null);
        //     tempMine.GetComponent<WeaponBase>().playerID = playerID;
        //     tempMine.GetComponent<WeaponBase>().damage = damage;
        //     tempMine.GetComponent<WeaponBase>().damageType = damageType;
        //     if (uses<=0) //out of mines, refill and discard weapon
        //     {
        //         uses = startingAmmo;
        //         CharacterControl.DiscardWeapon();
        //     }
        // }

        //}
    }
}
