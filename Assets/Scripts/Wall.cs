using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    [SerializeField] LayerMask collisionMask;
    Collider[] projSearch;

    //[SerializeField] Vector3 hitBoxSize;
    Vector3 hitBoxSize;

    void Start()
    {
        hitBoxSize = new Vector3(transform.localScale.x, transform.localScale.y, transform.localScale.z);
    }

    void Update()
    {
        projSearch = Physics.OverlapBox(transform.position, hitBoxSize/2, Quaternion.identity, collisionMask);
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
                    projSearch[i].transform.Rotate(0,90,0);

                    //prevent it from hitting the same wall multiple times
                    Vector3 tempDirection = projSearch[i].transform.position - transform.position ;
                    tempDirection.Normalize();

                    projSearch[i].transform.position = new Vector3(projSearch[i].transform.position.x + tempDirection.x, projSearch[i].transform.position.y, projSearch[i].transform.position.z + tempDirection.z);

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
