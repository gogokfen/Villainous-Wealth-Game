using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunShot : WeaponBase
{
    private LayerMask wallMask;
    private RaycastHit wallHit;

    [SerializeField] float shotSpeed;
    [SerializeField] float destroyTimer;
    [SerializeField] ParticleSystem bulletTrail;
    void Start()
    {
        wallMask = 1; //default layer
        Destroy(gameObject, destroyTimer);
    }

    void Update()
    {
        transform.Translate(Vector3.forward * shotSpeed * Time.deltaTime);
    }

    private void OnDestroy()
    {
        Destroy(bulletTrail, 0.5f);
        ParticleSystem.MainModule bulletTrailMain = bulletTrail.main;
        bulletTrail.transform.SetParent(null);
        bulletTrail.transform.localScale = new Vector3 (0.25f,0.25f,0.25f);
        bulletTrailMain.startSpeed = 0;
        //bulletTrail.Stop();
    }
}
