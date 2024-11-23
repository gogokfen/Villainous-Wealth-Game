using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destructable : MonoBehaviour
{

    [SerializeField] int hp = 5;

    [SerializeField] LayerMask collisionMask;
    Collider[] projSearch;
    Vector3 hitBox = new Vector3(0.75f, 1f, 0.75f);
    float identicalDamageCD;
    private CharacterControl.PlayerTypes lastPlayerID;

    [SerializeField] ParticleSystem hitEffect;
    [SerializeField] ParticleSystem explosionEffect;

    [SerializeField] GameObject[] debrisList;

    private bool destroyed = false;

    void Start()
    {
        
    }

    void Update()
    {
        if (!destroyed)
        {
            projSearch = Physics.OverlapBox(transform.position, hitBox, Quaternion.identity, collisionMask); //half extents
            identicalDamageCD -= Time.deltaTime;

            if (projSearch.Length > 0)
            {
                for (int i = 0; i < projSearch.Length; i++)
                {
                    TakeDamage(projSearch[i].GetComponent<WeaponBase>().playerID, projSearch[i].GetComponent<WeaponBase>().damage, projSearch[i].GetComponent<WeaponBase>().damageType);
                    if (projSearch[i].GetComponent<WeaponBase>().damageType == WeaponBase.damageTypes.destructableProjectile)
                    {
                        Destroy(projSearch[i].gameObject);
                    }

                }
            }
        }
        else
        {
            if (Random.Range(0,3) == 0)
                PickupManager.singleton.SpawnPowerup(transform.position);
            explosionEffect.Play();
            explosionEffect.transform.SetParent(null);
            for (int i = 0; i<debrisList.Length;i++)
            {
                debrisList[i].SetActive(true);
                debrisList[i].GetComponent<Rigidbody>().AddForce(new Vector3(Random.Range(-7.5f, 7.5f), Random.Range(5, 15f), Random.Range(-7.5f, 7.5f)), ForceMode.Impulse);
                debrisList[i].transform.SetParent(null);
                Destroy(debrisList[i], 2);
            }
            Destroy(gameObject);
        }

    }

    private void TakeDamage(CharacterControl.PlayerTypes attackingPlayer, int damage, WeaponBase.damageTypes damageType)
    {
        if (!(attackingPlayer == lastPlayerID &&
            (damageType == WeaponBase.damageTypes.indestructableProjectile || damageType == WeaponBase.damageTypes.melee || damageType == WeaponBase.damageTypes.bounceOffProjectile) &&
            identicalDamageCD >= 0)) //making sure lootbox is not taking multiple instances of damage from the same attack
        {
            hp-= damage;
            hitEffect.Play();
            if (hp <= 0)
                destroyed = true;
        }


        lastPlayerID = attackingPlayer;
        identicalDamageCD = 0.1f;

    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawCube(transform.position, hitBox*2); // the original is half extents
    }
}
