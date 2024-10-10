using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QualityLootDestructable : MonoBehaviour
{
    //projectile layer is 7

    [SerializeField] Slider pickupSlider;

    [SerializeField] LayerMask collisionMask;
    Collider[] projSearch;

    bool destroyed = false;

    void Start()
    {
        pickupSlider.value = 1;
    }

    
    void Update()
    {
        projSearch = Physics.OverlapBox(transform.position, new Vector3(3f, 3f, 3f), Quaternion.identity, collisionMask);

        if (projSearch.Length > 0)
        {
            for (int i = 0; i < projSearch.Length; i++)
            {
                pickupSlider.value -= 0.25f;
                if (projSearch[i].GetComponent<WeaponBase>().damageType == WeaponBase.damageTypes.destructableProjectile)
                {
                    Destroy(projSearch[i].gameObject);
                }
            }
        }


        if (pickupSlider.value == 0 && !destroyed)
        {
            destroyed = true;
            gameObject.layer = 6;
            Destroy(pickupSlider.gameObject);
        }
    }

    /* //projectiles don't have rigidbodies, can't detect gunshots
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("yes");
        if (other.gameObject.layer == 7)
        {
            Debug.Log("projectile");
            pickupSlider.value -= 0.25f;

            /*
            WeaponBase WB = other.GetComponent<WeaponBase>();
            TakeDamage(WB.playerID, WB.damage, WB.damageType);
            
        }
        
    }
    */

}
