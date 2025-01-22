using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupManager : MonoBehaviour
{
    public static PickupManager singleton { get; private set; }

    public bool DropWeapons = false;
    public bool DropPowerups = false;

    [SerializeField] float pickupFrequency = 3;

    [SerializeField] GameObject pickupPrefab;

    [SerializeField] GameObject coinSackPowerup;
    [SerializeField] GameObject healthPowerup;
    [SerializeField] GameObject shieldPowerup;
    [SerializeField] GameObject speedPowerup;

    List<GameObject> pickups = new List<GameObject>();


    List<GameObject> coins = new List<GameObject>();
    [SerializeField] GameObject prefabCoin;
    [SerializeField] GameObject coinPickup;
    [SerializeField] GameObject coinShot;
    [SerializeField] GameObject coinPickupVFX;
    GameObject winningPlayer;
    bool playerWon = false;
    float animTimer;

    float timer;

    private int currentCoinSackAmount;
    private int maxCoinSacksPerRound = 10;

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

        /* pickup no longer drop from the sky
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
        */
        for (int i = 0; i < coins.Count; i++) //updating the position of each pickup in the scene
        {
            if (coins[i].transform.position.y <= 1 && coins[i].name == "Coin")
            {
                GameObject tempCoin =Instantiate(coinPickup, new Vector3(coins[i].transform.position.x, 1, coins[i].transform.position.z),Quaternion.identity);
                tempCoin.name = "Coin2";
                coins[i].gameObject.SetActive(false);
                coins.Remove(coins[i]);

                coins.Add(tempCoin);
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
                //V2
                SpawnPowerUp();

                /** V1
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
                */
                
            }

            //GameObject tempCoin = Instantiate(prefabCoin, transform.position, Quaternion.identity);
            //coins.Add(tempCoin);
        }

        if (playerWon)
        {
            animTimer += Time.deltaTime;
            for (int i=0;i<coins.Count;i++)
            {
                if (coins[i].transform.name == "CoinShot")
                {
                    coins[i].GetComponent<BackToZero>().Return();
                    coins[i].transform.name = "Coin";
                }
                coins[i].transform.position = Vector3.Lerp(coins[i].transform.position, new Vector3 (winningPlayer.transform.position.x, winningPlayer.transform.position.y+1.5f, winningPlayer.transform.position.z), animTimer/3);
            }
            if (animTimer>=3.5f)
            {
                playerWon = false;
                animTimer = 0;
            }
        }

    }

    public void SpawnTreasureChestCoin(Vector3 treasureChestPosition)
    {
        //Vector3 coinPosition = new Vector3(Random.Range(-5f, 5f), Random.Range(4, 12f), Random.Range(-5f, 5f));
        Vector3 coinPosition = new Vector3(Random.Range(-7.5f, 7.5f), Random.Range(4, 10f), Random.Range(-7.5f, 7.5f));
        GameObject tempCoin = Instantiate(prefabCoin, treasureChestPosition,Quaternion.identity);
        tempCoin.name = prefabCoin.name;
        Rigidbody coinRB = tempCoin.GetComponent<Rigidbody>();
        coinRB.AddForce(coinPosition, ForceMode.Impulse);

        coins.Add(tempCoin);
        //Destroy(tempCoin, 10);
    }

    private void SpawnPowerUp()
    {
        //Vector3 tempPos = new Vector3(transform.position.x + Random.Range(-gizmosWidth / 2, gizmosWidth / 2), transform.position.y + Random.Range(-gizmosHight / 2, gizmosHight / 2), transform.position.z + Random.Range(-gizmosLength / 2, gizmosLength / 2));
        Vector3 tempPos = new Vector3(transform.position.x + Random.Range(-gizmosWidth / 2, gizmosWidth / 2), 1, transform.position.z + Random.Range(-gizmosLength / 2, gizmosLength / 2));
        //Debug.Log("PickupManager alive player" + tempPos);
        int randomPowerup = Random.Range(0, 4);
        if (randomPowerup == 0)
        {
            if (currentCoinSackAmount >= maxCoinSacksPerRound)
                SpawnPowerUp();
            else
            {
                GameObject tempPowerup = Instantiate(coinSackPowerup, tempPos, Quaternion.identity);
                tempPowerup.name = "CoinSack";

                //pickups.Add(tempPowerup);

                coins.Add(tempPowerup);

                currentCoinSackAmount++;
            }

            //Destroy(tempPowerup, 20);

        }
        else if (randomPowerup == 1)
        {
            if (currentCoinSackAmount >= maxCoinSacksPerRound)
                SpawnPowerUp();
            else
            {
                GameObject tempPowerup = Instantiate(coinSackPowerup, tempPos, Quaternion.identity);
                tempPowerup.name = "CoinSack";

                //pickups.Add(tempPowerup);

                coins.Add(tempPowerup);

                currentCoinSackAmount++;
            }

            //Destroy(tempPowerup, 20);


            //SpawnPowerUp();

            //disabled health pickup, time for cursed code
            /*
            GameObject tempPowerup = Instantiate(healthPowerup, tempPos, Quaternion.identity);
            tempPowerup.name = "Health";

            pickups.Add(tempPowerup);
            Destroy(tempPowerup, 20);
            */
        }
        else if (randomPowerup == 2)
        {
            GameObject tempPowerup = Instantiate(shieldPowerup, tempPos, Quaternion.identity);
            tempPowerup.name = "Shield";

            pickups.Add(tempPowerup);
            Destroy(tempPowerup, 20);
        }
        else if (randomPowerup == 3)
        {
            GameObject tempPowerup = Instantiate(speedPowerup, tempPos, Quaternion.identity);
            tempPowerup.name = "Speed";

            pickups.Add(tempPowerup);
            Destroy(tempPowerup, 20);
        }

    }

    public void SpawnPowerUp(Vector3 powerupPosition)
    {
        
        if (RoundManager.instance.areWeWarming == true) 
            return;

        

        //Debug.Log("PickupManager dead player" + powerupPosition);
        int randomPowerup = Random.Range(0, 4);
        if (randomPowerup == 0)
        {
            if (currentCoinSackAmount >= maxCoinSacksPerRound)
                SpawnPowerUp(powerupPosition);
            else
            {
                GameObject tempPowerup = Instantiate(coinSackPowerup, powerupPosition, Quaternion.identity);
                tempPowerup.name = "CoinSack";
                tempPowerup.GetComponent<Animator>().enabled = true;
                tempPowerup.GetComponent<Animator>().Play("Pickup");
                tempPowerup.GetComponent<BoxCollider>().enabled = true;

                coins.Add(tempPowerup);

                currentCoinSackAmount++;
            }



            //Destroy(tempPowerup, 20);
        }
        else if (randomPowerup == 1)
        {
            if (currentCoinSackAmount >= maxCoinSacksPerRound)
                SpawnPowerUp(powerupPosition);
            else
            {
                GameObject tempPowerup = Instantiate(coinSackPowerup, powerupPosition, Quaternion.identity);
                tempPowerup.name = "CoinSack";
                tempPowerup.GetComponent<Animator>().enabled = true;
                tempPowerup.GetComponent<Animator>().Play("Pickup");
                tempPowerup.GetComponent<BoxCollider>().enabled = true;

                coins.Add(tempPowerup);

                currentCoinSackAmount++;
            }


            //Destroy(tempPowerup, 20);



            //SpawnPowerUp();

            //disabled health pickup, time for cursed code
            /*
            GameObject tempPowerup = Instantiate(healthPowerup, powerupPosition, Quaternion.identity);
            tempPowerup.name = "Health";
            tempPowerup.GetComponent<Animator>().enabled = true;
            tempPowerup.GetComponent<BoxCollider>().enabled = true;
            Destroy(tempPowerup, 20);
            */
        }
        else if (randomPowerup == 2)
        {
            GameObject tempPowerup = Instantiate(shieldPowerup, powerupPosition, Quaternion.identity);
            tempPowerup.name = "Shield";
            tempPowerup.GetComponent<Animator>().enabled = true;
            tempPowerup.GetComponent<Animator>().Play("Pickup");
            tempPowerup.GetComponent<BoxCollider>().enabled = true;
            Destroy(tempPowerup, 20);
        }
        else if (randomPowerup == 3)
        {
            GameObject tempPowerup = Instantiate(speedPowerup, powerupPosition, Quaternion.identity);
            tempPowerup.name = "Speed";
            tempPowerup.GetComponent<Animator>().enabled = true;
            tempPowerup.GetComponent<Animator>().Play("Pickup");
            tempPowerup.GetComponent<BoxCollider>().enabled = true;
            Destroy(tempPowerup, 20);
        }


        //tempPowerup.GetComponent<Animator>().enabled = true;
        //tempPowerup.GetComponent<BoxCollider>().enabled = true;

        //Destroy(tempPowerup, 20);
    }

    public void CoinPickupVFX(Vector3 coinPos)
    {
        Instantiate(coinPickupVFX, coinPos, coinPickupVFX.transform.rotation);
    }

    public void CoinShot()
    {
        GameObject tempCoinShot = Instantiate(coinShot, Vector3.zero, Quaternion.Euler(0, Random.Range(0, 360f), 0));

        tempCoinShot.name = coinShot.name;
        coins.Add(tempCoinShot);
    }

    public void SetWinningPlayer(GameObject player)
    {
        player.GetComponent<CharacterControl>().winner = true; //transfer after

        winningPlayer = player;
        playerWon = true;

        for (int i = 0; i < coins.Count; i++)
        {
            Destroy(coins[i].GetComponent<Rigidbody>());
        }
    }

    public void RemovePickupFromList(GameObject pickup)
    {
        foreach (GameObject coin in coins)
        {
            if (coin == pickup)
            {
                coins.Remove(coin);
                return;
            }
        }
    }

    public void RemovePickupFromPickups(GameObject pickupToRemove)
    {
        foreach (GameObject pickup in pickups)
        {
            if (pickup == pickupToRemove)
            {
                pickups.Remove(pickupToRemove);
                return;
            }
        }
    }

    public void ResetCoinSackCount()
    {
        currentCoinSackAmount = 0;
        maxCoinSacksPerRound = Leaderboard.singleton.playerCount * 2 + 2;
    }

    public void DestroyAllPickups()
    {
        for (int i = 0; i < pickups.Count; i++)
        {
            Destroy(pickups[i]);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawCube(transform.position, new Vector3(gizmosWidth, gizmosHight, gizmosLength));
    }
}
