using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    private LayerMask collisionMask;
    private Collider[] projSearch;

    private Vector3 hitBoxSize;

    void Start()
    {
        hitBoxSize = new Vector3(transform.localScale.x, transform.localScale.y, transform.localScale.z);
        collisionMask.value = 128; //128 is the value of the projectile layer, I guess it works like bit numbers
    }

    void Update()
    {
        projSearch = Physics.OverlapBox(transform.position, hitBoxSize/2, transform.rotation, collisionMask);
        if (projSearch.Length > 0)
        {
            for (int i = 0; i < projSearch.Length; i++)
            {
                WeaponBase attackWB = projSearch[i].GetComponent<WeaponBase>();

                if (attackWB.damageType == WeaponBase.damageTypes.destructableProjectile)
                {
                    Destroy(projSearch[i].gameObject);
                }

                if (attackWB.damageType == WeaponBase.damageTypes.melee)
                {
                    Leaderboard.singleton.StopForwardMomentum(attackWB.playerID);
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
