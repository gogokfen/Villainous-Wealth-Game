using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonBall : WeaponBase
{
    [SerializeField] float flySpeed;
    [SerializeField] ParticleSystem explosionVFX;
    [SerializeField] ParticleSystem trailVFX;
    [SerializeField] GameObject fakeShadow;
    SphereCollider SC;

    Color fakeShadowColor;

    //LayerMask floorMask;

    void Start()
    {
        SC = GetComponent<SphereCollider>();
        damageType = damageTypes.grenade;
        SC.enabled = false;
        //floorMask = 2056; //floor mask is 10 which supposed to be 2056

        fakeShadow.transform.rotation = Quaternion.identity;
        fakeShadowColor = new Color(0.25f, 0.25f, 0.25f, 0.0f);
    }

    void Update()
    {
        transform.Translate(Vector3.forward * flySpeed * Time.deltaTime);

        fakeShadow.transform.position = new Vector3(transform.position.x, 0.05f, transform.position.z);
        fakeShadowColor = new Color(fakeShadowColor.r-Time.deltaTime, fakeShadowColor.g - Time.deltaTime, fakeShadowColor.b - Time.deltaTime, fakeShadowColor.a + Time.deltaTime*2);
        fakeShadow.GetComponent<Renderer>().material.SetColor("_BaseColor", fakeShadowColor);

        if (SC.enabled)
            Destroy(gameObject);

        if (transform.position.y<=0)
        {
            SC.enabled = true;
            explosionVFX.transform.position += Vector3.up / 4; 
            explosionVFX.transform.SetParent(null);
            explosionVFX.transform.rotation = Quaternion.identity;
            explosionVFX.Play();
            Destroy(explosionVFX.gameObject, 3);
            trailVFX.Stop();
            trailVFX.transform.SetParent(null);
            Destroy(trailVFX.gameObject, 3);
            SoundManager.singleton.PlayClip("GrenadeExplode", transform.position, 0.125f, true, true);
        }
    }
}
