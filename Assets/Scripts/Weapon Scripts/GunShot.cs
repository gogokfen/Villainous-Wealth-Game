using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunShot : WeaponBase
{
    private LayerMask wallMask;
    private RaycastHit wallHit;

    [SerializeField] float shotSpeed;
    [SerializeField] float destroyTimer;
    void Start()
    {
        wallMask = 1; //default layer
        Destroy(gameObject, destroyTimer);
    }

    void Update()
    {
        transform.Translate(Vector3.forward * shotSpeed * Time.deltaTime);
    }
}
