using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Pirate : MonoBehaviour
{
    [SerializeField] GameObject cannonV2GameObject;
    [SerializeField] CannonV2 cannonV2Script;
    [SerializeField] PlayerInput PI;
    [SerializeField] int price;

    private float payCD;
    private float AttackCD;

    private CharacterControl payingPlayerRef;

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
                if (Leaderboard.singleton.GetMoney(playerSearch[i].GetComponent<CharacterControl>().PlayerID)>=price && Time.time>=payCD)
                {
                    cannonV2GameObject.transform.position = transform.position;

                    payingPlayerRef = playerSearch[i].GetComponent<CharacterControl>();
                    //payingPlayerRef.StopAnimator();
                    payingPlayerRef.enabled = false;
                    payingPlayerRef.gameObject.SetActive(false);


                    Leaderboard.singleton.ModifyMoney((payingPlayerRef.PlayerID), -price);
                    cannonV2GameObject.SetActive(true);
                    cannonV2Script.UpdateShooter(payingPlayerRef.PlayerID);



                    //PI.user = 0;
                    //PI.SwitchCurrentControlScheme("Mouse & Keyboard",InputDevice[0].device);



                    payCD = Time.time + 12;
                    AttackCD = Time.time + 7;
                }
            }
        }

        if (Time.time>=AttackCD && payingPlayerRef!=null)
        {
            cannonV2GameObject.SetActive(false);
            payingPlayerRef.gameObject.SetActive(true); //consider making Character Control "Dead Stop" public and use it to fix animation bug
            payingPlayerRef.enabled = true;
            //payingPlayerRef.EnableAnimator();
            //payingPlayerRef.DeadStop();

            //considering making the pay available once per round
            gameObject.SetActive(false);
        }
    }
}
