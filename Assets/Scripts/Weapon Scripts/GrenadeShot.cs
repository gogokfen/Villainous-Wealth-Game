using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeShot : WeaponBase
{
    public float maxExplosionTime;
    [SerializeField] int explosionDamage = 6;
    [HideInInspector] public float throwPower; //HideInInspector
    //private float deAccel = 0.1f;
    private float explosionTime;
    private SphereCollider SC;
    private BoxCollider BC;

    [SerializeField] GameObject GFX;
    float rotationAmount;

    LayerMask wallMask; //[SerializeField] 
    RaycastHit wallHit;
    [SerializeField] ParticleSystem explosionEffect;

    float upwardforce; //for arc movement, aesthetic
    private float startingThrowPower;

    void Start()
    {
        //Debug.Log(wallMask.value); // default layer value is 1
        wallMask.value = 1;

        SC = GetComponent<SphereCollider>();
        BC = GetComponent<BoxCollider>();
        SC.enabled = false;
        explosionTime = maxExplosionTime;

        throwPower /= 2;
        startingThrowPower = throwPower;
        //delete
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
    }

    void Update()
    {
        transform.Translate(Vector3.forward * throwPower * Time.deltaTime);

        transform.Translate(Vector3.up * upwardforce * Time.deltaTime);

        upwardforce = (0.75f * startingThrowPower) - (Mathf.InverseLerp(explosionTime, 0, maxExplosionTime) * (1.75f * startingThrowPower));


        if (SC.enabled) //DON"T CHANGE THE ORDER OF THE IFS
        {
            Destroy(gameObject);
        }

        if ((maxExplosionTime <= 0 || transform.position.y<=0.5) && BC.enabled  == true) 
        {
            transform.rotation = Quaternion.identity;
            damageType = damageTypes.grenade;
            damage = explosionDamage;
            BC.enabled = false;
            SC.enabled = true;
            explosionEffect.transform.SetParent(null);
            explosionEffect.Play();
            Destroy(explosionEffect.gameObject, 3f); 
        }

        //grenade slowdown V1
        /*
        deAccel *= (1 + Time.deltaTime*5);
        if (throwPower - deAccel >= 0)
        {
            throwPower -= deAccel;
        }
        else
            throwPower = 0;
        */

        //grenade slowdown V2

        //throwPower /= (1 + Time.deltaTime*2.5f); //bring back after fixing upward force

        maxExplosionTime -= Time.deltaTime;

        //rotationAmount = maxExplosionTime * 1000;
        rotationAmount = throwPower * 25;
        GFX.transform.Rotate(rotationAmount * Time.deltaTime,0 , 0);

        if (Physics.Raycast(transform.position, transform.forward, out wallHit, 1f,wallMask)) //wall collisions
        {

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
                //startingAngle = transform.eulerAngles.y;
                //complementaryAngle = 180 - startingAngle;
                //desiredRotationAngle = 180 - (2 * complementaryAngle);

                startingAngle = transform.eulerAngles.y;
                complementaryAngle = 90 - startingAngle;
                desiredRotationAngle = (2 * complementaryAngle);
            }
            



            transform.Rotate(0, desiredRotationAngle, 0);

            Vector3 tempDirection = wallHit.transform.position - transform.position;
            tempDirection.Normalize();
            tempDirection /= 2;

            transform.position = new Vector3(transform.position.x + tempDirection.x, transform.position.y, transform.position.z + tempDirection.z);


        }

    }
}
