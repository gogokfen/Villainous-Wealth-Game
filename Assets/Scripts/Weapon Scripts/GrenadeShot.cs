using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VInspector;

public class GrenadeShot : WeaponBase
{
    public float currentExplosionTime; //0.875
    [SerializeField] int explosionDamage = 6;
    [HideInInspector] public float throwPower;
    private float maxExplosionTime;
    private SphereCollider SC;
    private BoxCollider BC;

    [SerializeField] GameObject GFX;
    private float rotationAmount;

    private LayerMask wallMask;
    private RaycastHit wallHit;
    [SerializeField] ParticleSystem explosionEffect;
    [SerializeField] ParticleSystem trail;

    private float upwardforce; //for arc movement, aesthetic
    private float startingThrowPower;

    void Start()
    {
        wallMask.value = 1; // default layer value is 1

        SC = GetComponent<SphereCollider>();
        BC = GetComponent<BoxCollider>();
        SC.enabled = false;

        currentExplosionTime = currentExplosionTime - (0.75f * Mathf.InverseLerp(75, 5, throwPower));

        maxExplosionTime = currentExplosionTime;

        throwPower /= 2;
        startingThrowPower = throwPower;
        //delete
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
    }

    void Update()
    {
        transform.Translate(Vector3.forward * throwPower * Time.deltaTime);

        transform.Translate(Vector3.up * upwardforce * Time.deltaTime);

        upwardforce = (0.75f * startingThrowPower) - (Mathf.InverseLerp(maxExplosionTime, 0, currentExplosionTime) * (1.75f * startingThrowPower));

        if (SC.enabled) //DON'T CHANGE THE ORDER OF THE IFs
        {
            Destroy(gameObject);
        }

        if ((currentExplosionTime <= 0 || transform.position.y<=0) && BC.enabled  == true)
        {
            transform.rotation = Quaternion.identity;
            damageType = damageTypes.grenade;
            damage = explosionDamage;
            BC.enabled = false;
            SC.enabled = true;
            explosionEffect.transform.SetParent(null);
            explosionEffect.transform.position += Vector3.up/4; //making sure the effect is visble
            explosionEffect.Play();

            SoundManager.singleton.PlayClip("GrenadeExplode", transform.position, 0.15f, true, true);

            Destroy(explosionEffect.gameObject, 3f);
        }
        currentExplosionTime -= Time.deltaTime;

        rotationAmount = throwPower * 25;
        GFX.transform.Rotate(rotationAmount * Time.deltaTime,0 , 0);

        if (Physics.Raycast(transform.position, transform.forward, out wallHit, 1f,wallMask)) //wall collisions
        {
            Vector3 newDirection = Vector3.Reflect(transform.forward, wallHit.normal);

            transform.forward = newDirection;
            
            Vector3 tempDirection = wallHit.transform.position - transform.position;
            tempDirection.Normalize();
            tempDirection /= 2;

            transform.position = new Vector3(transform.position.x + tempDirection.x, transform.position.y, transform.position.z + tempDirection.z);
        }
    }

    private void OnDestroy()
    {
        Destroy(trail.gameObject, 0.5f);
        //ParticleSystem.MainModule bulletTrailMain = bulletTrail.main;
        trail.transform.SetParent(null);
        //transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
        //bulletTrailMain.startSpeed = 0;
    }
}
