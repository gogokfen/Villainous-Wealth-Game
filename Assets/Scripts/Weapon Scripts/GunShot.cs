using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VInspector;

public class GunShot : WeaponBase
{
    [Foldout("Upgrades")]
    [HideInInspector] public bool GunRicochetUpgrade = false;
    LayerMask wallMask;
    RaycastHit wallHit;
    [EndFoldout]

    [SerializeField] float shotSpeed;
    void Start()
    {
        wallMask = 1; //default layer
        Destroy(gameObject, 1.75f);
    }

    void Update()
    {
        transform.Translate(Vector3.forward * shotSpeed * Time.deltaTime);

        if (GunRicochetUpgrade)
        {
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
}
