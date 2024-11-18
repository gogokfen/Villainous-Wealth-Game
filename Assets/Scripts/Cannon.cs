using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannon : MonoBehaviour
{
    [SerializeField] LayerMask collisionMask;
    Collider[] cannonBallSearch;
    Vector3 hitBox = new Vector3(1f, 0.75f, 0.75f);

    [SerializeField] GameObject coinShot;

    private int coinShotAmount;
    private float coinShotTimer;

    float test;

    void Start()
    {
        
    }

    void Update()
    {
        cannonBallSearch = Physics.OverlapBox(transform.position, hitBox, Quaternion.identity, collisionMask); //half extents

        if (cannonBallSearch.Length > 0)
        {
            for (int i = 0; i < cannonBallSearch.Length; i++)
            {
                int cannonBallAmount = Int32.Parse(cannonBallSearch[i].name);
                if (cannonBallAmount>0)
                {
                    cannonBallAmount--;
                    cannonBallSearch[i].name = "" + cannonBallAmount;

                    coinShotAmount = UnityEngine.Random.Range(3, 9);
                }
            }
        }

        if (coinShotAmount > 0)
        {
            if (Time.time >= coinShotTimer)
            {
                coinShotAmount--;
                coinShotTimer = Time.time + 0.25f;

                Instantiate(coinShot,Vector3.zero,Quaternion.Euler(0,UnityEngine.Random.Range(0,360f),0));
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawCube(transform.position, hitBox * 2); // the original is half extents
    }
}
