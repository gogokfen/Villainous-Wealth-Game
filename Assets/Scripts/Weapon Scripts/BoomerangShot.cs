using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoomerangShot : WeaponBase
{
    public Transform lookAtTarget;
    public float flySpeed = 15;
    private Quaternion prevRotation;
    private float rotationAmount;

    private float upTime;

    private LayerMask wallMask;
    private RaycastHit wallHit;

    private void Start()
    {
        wallMask.value = 1;
    }

    void Update()
    {
        upTime += Time.deltaTime;

        transform.Translate(Vector3.forward * flySpeed * Time.deltaTime);

        if (flySpeed < 15)
        {
            flySpeed += Time.deltaTime * 5;
            rotationAmount += Time.deltaTime * 7.5f;
        }
            
        prevRotation = transform.rotation;
        transform.LookAt(lookAtTarget);
        rotationAmount += Time.deltaTime*2;

        if (upTime>=2)
            rotationAmount += Time.deltaTime * 25;

        transform.rotation = Quaternion.Lerp(prevRotation, transform.rotation, Time.deltaTime * rotationAmount);

        if (Physics.Raycast(transform.position, transform.forward, out wallHit, 1f, wallMask))
        {
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
