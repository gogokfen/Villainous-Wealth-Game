using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoomerangShot : WeaponBase
{
    public Transform lookAtTarget;
    public float flySpeed = 15;
    private Quaternion prevRotation;
    private float rotationAmount;

    void Update()
    {
        transform.Translate(Vector3.forward * flySpeed * Time.deltaTime);

        prevRotation = transform.rotation;
        transform.LookAt(lookAtTarget);
        rotationAmount += Time.deltaTime*2;
        transform.rotation = Quaternion.Lerp(prevRotation, transform.rotation, Time.deltaTime * rotationAmount);

        if (CharacterControl.weaponID!=3) //make the enum a public class?
        {
            Destroy(gameObject);
        }
    }
}
