using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunShot : WeaponBase
{
    [SerializeField] float shotSpeed;
    void Start()
    {
        Destroy(gameObject, 3);
    }

    void Update()
    {
        transform.Translate(Vector3.forward * shotSpeed * Time.deltaTime);
    }
}
