using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pirate : MonoBehaviour
{
    [SerializeField] GameObject cannonV2GameObject;
    [SerializeField] CannonV2 cannonV2Script;

    private float payCD;
    private float AttackCD;


    [SerializeField] LayerMask collisionMask;
    Collider[] playerSearch;
    Vector3 hitBox = new Vector3(1f, 0.75f, 0.75f);

    void Start()
    {
        
    }


    void Update()
    {
        playerSearch = Physics.OverlapBox(transform.position, hitBox, Quaternion.identity, collisionMask); //half extents

        if (playerSearch.Length > 0)
        {
            for (int i = 0; i < playerSearch.Length; i++)
            {
                if (MoneyManager.singleton.GetMoney(playerSearch[i].GetComponent<CharacterControl>().PlayerID)>=5 && Time.time>=payCD)
                {
                    MoneyManager.singleton.ModifyMoney((playerSearch[i].GetComponent<CharacterControl>().PlayerID), -5);
                    cannonV2GameObject.SetActive(true);
                    cannonV2Script.UpdateShooter(playerSearch[i].GetComponent<CharacterControl>().PlayerID);

                    payCD = Time.time + 12;
                    AttackCD = Time.time + 7;
                }
            }
        }

        if (Time.time>=AttackCD)
        {
            cannonV2GameObject.SetActive(false);
        }
    }
}
