using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonBall : WeaponBase
{
    [SerializeField] float flySpeed;
    [SerializeField] ParticleSystem explosionVFX;
    SphereCollider SC;
    //LayerMask floorMask;

    void Start()
    {
        SC = GetComponent<SphereCollider>();
        damageType = damageTypes.grenade;
        SC.enabled = false;
        //floorMask = 2056; //floor mask is 10 which supposed to be 2056
    }

    void Update()
    {
        transform.Translate(Vector3.forward * flySpeed * Time.deltaTime);

        if (SC.enabled)
            Destroy(gameObject);

        if (transform.position.y<=0)
        {
            SC.enabled = true;
            explosionVFX.transform.position += Vector3.up / 4; 
            explosionVFX.transform.SetParent(null);
            explosionVFX.Play();
            Destroy(explosionVFX.gameObject, 3);

            //SoundManager.singleton.PlayClip("GrenadeExplode", transform.position, 1f, false, true);

        }
    }
}
