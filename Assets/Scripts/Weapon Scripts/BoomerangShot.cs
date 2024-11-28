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

    LayerMask wallMask;
    RaycastHit wallHit;

    private void Start()
    {
        wallMask.value = 1;
    }

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


        if (Physics.Raycast(transform.position, transform.forward, out wallHit, 1f, wallMask))
        {
            /*
            float startingAngle;
            float complementaryAngle;
            float desiredRotationAngle;

            if (wallHit.normal == Vector3.right || wallHit.normal == Vector3.left)
            {
                startingAngle = transform.eulerAngles.y;
                complementaryAngle = 180 - startingAngle;
                desiredRotationAngle = (2 * complementaryAngle);
            }
            else
            {
                startingAngle = transform.eulerAngles.y;
                complementaryAngle = 90 - startingAngle;
                desiredRotationAngle = (2 * complementaryAngle);
            }
            */
            //transform.Rotate(0, desiredRotationAngle, 0);

            Vector3 newDirection = Vector3.Reflect(transform.forward, wallHit.normal);

            newDirection.y = 0;

            transform.forward = newDirection;

            Vector3 tempDirection = wallHit.transform.position - transform.position;
            tempDirection.Normalize();
            tempDirection /= 2;

            transform.position = new Vector3(transform.position.x + tempDirection.x, transform.position.y, transform.position.z + tempDirection.z);
        }

    }
}
