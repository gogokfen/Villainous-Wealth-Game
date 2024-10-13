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
        /*
        foreach (GameObject pickup in pickups) //updating the position of each pickup in the scene
        {
            if (pickup.transform.position.y>1) //0
            {
                pickup.transform.position = new Vector3(pickup.transform.position.x, pickup.transform.position.y - Time.deltaTime*3, pickup.transform.position.z);
                if (pickup.transform.position.y<=1)
                {
                    pickup.GetComponent<Animator>().enabled = true;
                    //pickups.Remove(pickup);
                }
            }
        }
        */

        
        for (int i=0;i<pickups.Count;i++) //updating the position of each pickup in the scene
        {
            if (pickups[i].transform.position.y > 1) //0
            {
                pickups[i].transform.position = new Vector3(pickups[i].transform.position.x, pickups[i].transform.position.y - Time.deltaTime * 3, pickups[i].transform.position.z);
                if (pickups[i].transform.position.y <= 1)
                {
                    pickups[i].GetComponent<Animator>().enabled = true;
                    pickups[i].GetComponent<BoxCollider>().enabled = true;
                    pickups.Remove(pickups[i]);
                }
            }
        }
        
        if (Time.time >= timer)
        {
            timer += pickupFrequency; //timer = Time.time + 3

            //////////////////////////////// V1 
            //generates a random pickup by giving the pickup a random name from the weapon list

            Vector3 tempPos = new Vector3(transform.position.x+Random.Range(-gizmosWidth / 2, gizmosWidth / 2), transform.position.y+ Random.Range(-gizmosHight / 2, gizmosHight / 2), transform.position.z+ Random.Range(-gizmosLength / 2, gizmosLength / 2));
            GameObject tempPickup = Instantiate(pickupPrefab, tempPos,Quaternion.identity);

            int nameRandomlyPicked = Random.Range(1, CharacterControl.Weapons.GetNames(typeof(CharacterControl.Weapons)).Length); //starts from 1 cause 0 is fist
            CharacterControl.Weapons weaponName;

            weaponName = (CharacterControl.Weapons)nameRandomlyPicked;
            tempPickup.name = weaponName.ToString();

            Destroy(tempPickup, 20); //prevents scene from bloating

            /*
            foreach (int index in CharacterControl.Weapons.GetValues(typeof(CharacterControl.Weapons))) 
            {
                weaponName = (CharacterControl.Weapons)index;
                tempPickup.name = weaponName.ToString();
                //Debug.Log(index);
            }
            */


            //tempPickup.name = pickupPrefab.name;

            pickups.Add(tempPickup);

            
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawCube(transform.position, new Vector3(gizmosWidth, gizmosHight, gizmosLength));
    }
}
