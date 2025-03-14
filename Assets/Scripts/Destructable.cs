using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destructable : MonoBehaviour
{
    [SerializeField] int hp = 5;

    [SerializeField] LayerMask collisionMask;
    private Collider[] projSearch;
    private Vector3 hitBox = new Vector3(1.25f, 1f, 1.25f);
    private float identicalDamageCD;
    private CharacterControl.PlayerTypes lastPlayerID;

    [SerializeField] ParticleSystem hitEffect;
    [SerializeField] ParticleSystem explosionEffect;

    [SerializeField] GameObject[] debrisList;

    private Animator anim;

    private bool destroyed = false;

    void Start()
    {
        anim = GetComponent<Animator>();

        anim.enabled = true;
        anim.speed = Random.Range(0.75f, 1.25f);
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
                PickupManager.singleton.SpawnPowerUp(transform.position);
            SoundManager.singleton.PlayClip($"DestructableDie", transform.position, 0.3f, true, true); //return with sound
            explosionEffect.Play();
            explosionEffect.transform.SetParent(null);
            Destroy(explosionEffect.gameObject, 3);
            for (int i = 0; i<debrisList.Length;i++)
            {
                if (Random.Range(0, 4) == 0) //randomizing the debris, no need for all of them
                {
                    debrisList[i].SetActive(true);
                    debrisList[i].GetComponent<Rigidbody>().AddForce(new Vector3(Random.Range(-6f, 6f), Random.Range(6f, 9f), Random.Range(-6f, 6f)), ForceMode.Impulse);
                    debrisList[i].GetComponent<Rigidbody>().AddTorque(new Vector3(Random.Range(-7.5f, 7.5f), Random.Range(-7.5f, 7.5f), Random.Range(-7.5f, 7.5f)), ForceMode.Impulse);
                }

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
            if (damageType != WeaponBase.damageTypes.zone)
            {
                hp -= damage;
                hitEffect.Play();
                anim.Play("ChestGettingHit");
                SoundManager.singleton.PlayClip($"DestructableHit", transform.position, 0.15f, true, true); //return with sound
                if (hp <= 0)
                    destroyed = true;
            }
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
