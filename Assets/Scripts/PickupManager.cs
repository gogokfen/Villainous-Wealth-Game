using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupManager : MonoBehaviour
{
    [SerializeField] GameObject pickupPrefab;

    List<GameObject> pickups = new List<GameObject>();

    [SerializeField] float pickupFrequency = 3;

    float timer;


    [Header("Gizmos")]
    [SerializeField] int gizmosWidth;
    [SerializeField] int gizmosHight;
    [SerializeField] int gizmosLength;
    void Start()
    {
        timer = Time.time + pickupFrequency;
    }

    void Update()
    {
        foreach (GameObject pickup in pickups)
        {
            if (pickup.transform.position.y>=2) //0
            {
                pickup.transform.position = new Vector3(pickup.transform.position.x, pickup.transform.position.y - Time.deltaTime*3, pickup.transform.position.z);
            }
        }


        if (Time.time >= timer)
        {
            timer += pickupFrequency; //timer = Time.time + 3
            Vector3 tempPos = new Vector3(transform.position.x+Random.Range(-gizmosWidth / 2, gizmosWidth / 2), transform.position.y+ Random.Range(-gizmosHight / 2, gizmosHight / 2), transform.position.z+ Random.Range(-gizmosLength / 2, gizmosLength / 2));
            GameObject tempPickup = Instantiate(pickupPrefab, tempPos,Quaternion.identity);

            tempPickup.name = pickupPrefab.name;

            pickups.Add(tempPickup);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawCube(transform.position, new Vector3(gizmosWidth, gizmosHight, gizmosLength));
    }
}
