using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class QualityLootDestructable : MonoBehaviour
{
    //projectile layer is 7

    [SerializeField] int hitsNeeded;

    [SerializeField] Slider pickupSlider;

    [SerializeField] LayerMask collisionMask;
    [SerializeField] Animator animator;
    Collider[] projSearch;
    float identicalDamageCD;
    private CharacterControl.PlayerTypes lastPlayerID;

    [SerializeField] ParticleSystem hitEffect;

    private bool looted = false;

    [SerializeField] bool isTreasureChest = false;

    int coinNumber;
    //Vector3 coinPosition;
    float coinTimer;

    [SerializeField] GameObject coin;
    Rigidbody coinRB;

    void Start()
    {
        pickupSlider.value = 1;

        if (SceneManager.GetActiveScene().name != "OsherScene")
            CameraManager.instance.AddToGroup(gameObject);
    }

    
    void Update()
    {
        projSearch = Physics.OverlapBox(transform.position, new Vector3(1f, 0.75f, 0.75f), Quaternion.identity, collisionMask); //half extents
        identicalDamageCD -= Time.deltaTime;

        if (projSearch.Length > 0 && !looted)
        {
            for (int i = 0; i < projSearch.Length; i++)
            {
                //pickupSlider.value -= (float)(1/(float)hitsNeeded);
                TakeDamage(projSearch[i].GetComponent<WeaponBase>().playerID, projSearch[i].GetComponent<WeaponBase>().damage, projSearch[i].GetComponent<WeaponBase>().damageType);
                if (projSearch[i].GetComponent<WeaponBase>().damageType == WeaponBase.damageTypes.destructableProjectile)
                {
                    Destroy(projSearch[i].gameObject);
                }

                if (pickupSlider.value<=0) //&& !looted
                {
                    looted = true;
                    if (isTreasureChest)
                    {
                        DropCoins();
                    }
                    else
                    {
                        EnableLoot();
                    }
                }
            }
        }

        if (coinNumber>0)
        {
            if (Time.time>=coinTimer)
            {
                coinNumber--;
                coinTimer = Time.time + 0.05f;

                //transferred to pickupManager to have control over all the coins in the scene
                PickupManager.singleton.SpawnTreasureChestCoin(new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z));

                /*
                coinPosition = new Vector3(Random.Range(-5f, 5f), Random.Range(4, 12f), Random.Range(-5f, 5f));
                GameObject tempCoin = Instantiate(coin, transform.position, transform.rotation);
                tempCoin.name = coin.name;
                coinRB = tempCoin.GetComponent<Rigidbody>();
                coinRB.AddForce(coinPosition, ForceMode.Impulse);
                Destroy(tempCoin, 10);
                */
            }
        }
    }

    private void TakeDamage(CharacterControl.PlayerTypes attackingPlayer, int damage, WeaponBase.damageTypes damageType)
    {
        if (!(attackingPlayer == lastPlayerID &&
            (damageType == WeaponBase.damageTypes.indestructableProjectile || damageType == WeaponBase.damageTypes.melee || damageType == WeaponBase.damageTypes.bounceOffProjectile) &&
            identicalDamageCD >= 0)) //making sure lootbox is not taking multiple instances of damage from the same attack
        {
            if (damageType!= WeaponBase.damageTypes.zone)
            {
                pickupSlider.value -= (float)(damage / (float)hitsNeeded);
                animator.Play("ChestGettingHit");
                hitEffect.Play();
                SoundManager.singleton.PlayClip($"DestructableHit", transform.position, 0.15f, true, true); //return with sound
            }
        }

        lastPlayerID = attackingPlayer;
        identicalDamageCD = 0.1f;
        
    }

    private void EnableLoot()
    {
        gameObject.layer = 6;
        Destroy(pickupSlider.gameObject);
    }

    private void DropCoins()
    {
        coinNumber = Random.Range(5,16);
        //animator.SetTrigger("Open");
        animator.Play("ChestOpen");

        if (SceneManager.GetActiveScene().name != "OsherScene")
            CameraManager.instance.RemoveFromCameraGroup(gameObject);

        //SoundManager.singleton.ChestOpen(transform.position);
        SoundManager.singleton.PlayClip("ChestOpen", transform.position, 0.25f, true, true);
        /*
        for (int i=0;i<coinNumber;i++)
        {
            coinPosition = new Vector3(Random.Range(-5f, 5f), Random.Range(4, 12f), Random.Range(-5f, 5f));
            GameObject tempCoin = Instantiate(coin, transform.position, transform.rotation);
            tempCoin.name = coin.name;
            coinRB = tempCoin.GetComponent<Rigidbody>();
            coinRB.AddForce(coinPosition, ForceMode.Impulse);
            Destroy(tempCoin, 10);
        }
        */
        //Destroy(pickupSlider.gameObject);
        pickupSlider.gameObject.SetActive(false);

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
