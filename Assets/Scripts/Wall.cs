using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    LayerMask collisionMask; //[SerializeField] 
    Collider[] projSearch;

    //[SerializeField] Vector3 hitBoxSize;
    Vector3 hitBoxSize;

    void Start()
    {
        hitBoxSize = new Vector3(transform.localScale.x, transform.localScale.y, transform.localScale.z);
        //Debug.Log(collisionMask.value); //128 is the value of the projectile layer, I guess it works like bit numbers
        collisionMask.value = 128;
    }

    void Update()
    {
        projSearch = Physics.OverlapBox(transform.position, hitBoxSize/2, transform.rotation, collisionMask);
        if (projSearch.Length > 0)
        {
            for (int i = 0; i < projSearch.Length; i++)
            {
                WeaponBase attackWB = projSearch[i].GetComponent<WeaponBase>();

                if (projSearch[i].GetComponent<WeaponBase>().damageType == WeaponBase.damageTypes.destructableProjectile)
                {
                    Destroy(projSearch[i].gameObject);
                }

                else if (projSearch[i].GetComponent<WeaponBase>().damageType == WeaponBase.damageTypes.bounceOffProjectile)
                {
                    //projSearch[i].transform.Rotate(0,90,0); //need to think about the correct calculation
                    /*
                    float startingAngle;
                    float complementaryAngle;
                    float desiredRotationAngle;

                    startingAngle = projSearch[i].transform.eulerAngles.y;
                    complementaryAngle = 180 - startingAngle;
                    desiredRotationAngle = 180 - (2 * complementaryAngle);

                    //Debug.Log("Alpha: "+startingAngle);
                    //Debug.Log("Beta: "+complementaryAngle);
                    //Debug.Log("Gamma: "+desiredRotationAngle);
                    


                    projSearch[i].transform.Rotate(0, desiredRotationAngle, 0);
                    */
                    
                    //projSearch[i].transform.position = Vector3.Reflect(projSearch[i].transform.position, Vector3.right);

                    //prevent it from hitting the same wall multiple times
                    //Vector3 tempDirection = projSearch[i].transform.position - transform.position ;
                    //tempDirection.Normalize();
                    //tempDirection /= 2;

                    //projSearch[i].transform.position = new Vector3(projSearch[i].transform.position.x + tempDirection.x, projSearch[i].transform.position.y, projSearch[i].transform.position.z + tempDirection.z);

                }

            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawCube(transform.position, hitBoxSize);
    }
}
