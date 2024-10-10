using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoomerangShot : WeaponBase
{
    public Transform lookAtTarget;
    public float flySpeed = 15;
    private Quaternion prevRotation;
    private float rotationAmount;

    float upTime;

    void Update()
    {
        upTime += Time.deltaTime;

        transform.Translate(Vector3.forward * flySpeed * Time.deltaTime);

        prevRotation = transform.rotation;
        transform.LookAt(lookAtTarget);
        rotationAmount += Time.deltaTime*2;

        if (upTime>=2)
        {
            rotationAmount += Time.deltaTime * 25;
        }

        transform.rotation = Quaternion.Lerp(prevRotation, transform.rotation, Time.deltaTime * rotationAmount);

        if (CharacterControl.weaponID != (int)CharacterControl.Weapons.Boomerang) //make the enum a public class?
        {
            Destroy(gameObject);
        }
        /*
        if (CharacterControl.weaponID != 3) //make the enum a public class?
        {
            CharacterControl.weaponID != CharacterControl.Weapons.Boomerang
            Destroy(gameObject);
        }
        */
    }
}
