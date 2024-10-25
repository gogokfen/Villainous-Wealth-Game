using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeShot : WeaponBase
{
    public float maxExplosionTime;
    [SerializeField] int explosionDamage = 6;
    [HideInInspector] public float throwPower; //HideInInspector
    private float deAccel = 0.1f;
    private float explosionTime;
    private SphereCollider SC;
    private BoxCollider BC;

    [SerializeField] GameObject GFX;
    float rotationAmount;

    LayerMask wallMask; //[SerializeField] 
    RaycastHit wallHit;

    void Start()
    {
        //Debug.Log(wallMask.value); // default layer value is 1
        wallMask.value = 1;

        SC = GetComponent<SphereCollider>();
        BC = GetComponent<BoxCollider>();
        SC.enabled = false;
        explosionTime = maxExplosionTime;
    }

    void Update()
    {
        transform.Translate(Vector3.forward * throwPower * Time.deltaTime);




        if (SC.enabled) //DON"T CHANGE THE ORDER OF THE IFS
        {
            Destroy(gameObject);
        }

        if (maxExplosionTime<=0)
        {
            damageType = damageTypes.grenade;
            damage = explosionDamage;
            BC.enabled = false;
            SC.enabled = true;
        }

        //deAccel += Time.deltaTime * 1;
        deAccel *= (1 + Time.deltaTime*5);
        if (throwPower - deAccel >= 0)
        {
            throwPower -= deAccel;
        }
        else
            throwPower = 0;

        maxExplosionTime -= Time.deltaTime;

        rotationAmount = maxExplosionTime * 1000;
        GFX.transform.Rotate(0, rotationAmount * Time.deltaTime, 0);

        if (Physics.Raycast(transform.position, transform.forward, out wallHit, 1f,wallMask))
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
