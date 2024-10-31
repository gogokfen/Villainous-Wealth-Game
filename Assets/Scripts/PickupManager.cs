using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupManager : MonoBehaviour
{
    public static PickupManager singleton { get; private set; }

    public bool DropWeapons = false;
    public bool DropPowerups = false;

    [SerializeField] GameObject pickupPrefab;

    List<GameObject> pickups = new List<GameObject>();

    [SerializeField] float pickupFrequency = 3;

    List<GameObject> coins = new List<GameObject>();
    [SerializeField] GameObject prefabCoin;
    //public static int uncollectedCoinsAmount = 0;
    GameObject winningPlayer;
    bool playerWon = false;
    float animTimer;

    float timer;

    [Header("Gizmos")]
    [SerializeField] int gizmosWidth;
    [SerializeField] int gizmosHight;
    [SerializeField] int gizmosLength;
    void Start()
    {
        timer = Time.time + pickupFrequency;
        playerWon = false;
    }

    private void Awake()
    {
        singleton = this;
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
            if (DropWeapons)
            {
                //////////////////////////////// V1 
                //generates a random pickup by giving the pickup a random name from the weapon list

                Vector3 tempPos = new Vector3(transform.position.x + Random.Range(-gizmosWidth / 2, gizmosWidth / 2), transform.position.y + Random.Range(-gizmosHight / 2, gizmosHight / 2), transform.position.z + Random.Range(-gizmosLength / 2, gizmosLength / 2));
                GameObject tempPickup = Instantiate(pickupPrefab, tempPos, Quaternion.identity);

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

            if (DropPowerups)
            {
                Vector3 tempPos = new Vector3(transform.position.x + Random.Range(-gizmosWidth / 2, gizmosWidth / 2), transform.position.y + Random.Range(-gizmosHight / 2, gizmosHight / 2), transform.position.z + Random.Range(-gizmosLength / 2, gizmosLength / 2));
                GameObject tempPickup = Instantiate(pickupPrefab, tempPos, Quaternion.identity);

                int nameRandomlyPicked = Random.Range(0, 4);

                if (nameRandomlyPicked == 0)
                    tempPickup.name = "Coin";
                else if (nameRandomlyPicked == 1)
                    tempPickup.name = "Health";
                else if (nameRandomlyPicked == 2)
                    tempPickup.name = "Shield";
                else if (nameRandomlyPicked == 3)
                    tempPickup.name = "Speed";

                pickups.Add(tempPickup);

                Destroy(tempPickup, 20);
            }

            //GameObject tempCoin = Instantiate(prefabCoin, transform.position, Quaternion.identity);
            //coins.Add(tempCoin);
        }

        if (playerWon)
        {
            animTimer += Time.deltaTime/3;
            for (int i=0;i<coins.Count;i++)
            {
                coins[i].transform.position = Vector3.Lerp(coins[i].transform.position, winningPlayer.transform.position, animTimer);
            }
            if (animTimer>=3.5f)
            {
                playerWon = false;
                animTimer = 0;
            }
        }

    }

    public void SpawnTreasureChestCoin(Transform treasureChest)
    {
        Vector3 coinPosition = new Vector3(Random.Range(-5f, 5f), Random.Range(4, 12f), Random.Range(-5f, 5f));
        GameObject tempCoin = Instantiate(prefabCoin, treasureChest.position, treasureChest.rotation);
        tempCoin.name = prefabCoin.name;
        Rigidbody coinRB = tempCoin.GetComponent<Rigidbody>();
        coinRB.AddForce(coinPosition, ForceMode.Impulse);

        coins.Add(tempCoin);
        //Destroy(tempCoin, 10);
    }

    public void SetWinningPlayer(GameObject player)
    {
        winningPlayer = player;
        playerWon = true;

        for (int i = 0; i < coins.Count; i++)
        {
            Destroy(coins[i].GetComponent<Rigidbody>());
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawCube(transform.position, new Vector3(gizmosWidth, gizmosHight, gizmosLength));
    }
}
