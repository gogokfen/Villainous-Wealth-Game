using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupManager : MonoBehaviour
{
    public static PickupManager singleton { get; private set; }

    public enum PowerupTypes
    {
        CoinSack,
        Health,
        Speed,
        Shield
    }
    public bool DropWeapons = false;
    public bool DropPowerups = false;

    [SerializeField] float pickupFrequency = 3;

    [SerializeField] GameObject pickupPrefab;

    [SerializeField] GameObject coinSackPowerup;
    [SerializeField] GameObject healthPowerup;
    [SerializeField] GameObject shieldPowerup;
    [SerializeField] GameObject speedPowerup;

    [SerializeField] GameObject[] weaponPickups;

    List<GameObject> pickups = new List<GameObject>();

    List<GameObject> coins = new List<GameObject>();
    [SerializeField] GameObject prefabCoin;
    [SerializeField] GameObject coinPickup;
    [SerializeField] GameObject coinShot;
    [SerializeField] GameObject coinPickupVFX;
    Transform winningPlayer;
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

                pickups.Add(tempPickup);
            }

            if (DropPowerups)
            {
                SpawnPowerUp();
            }
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
        Vector3 coinPosition = new Vector3(Random.Range(-7.5f, 7.5f), Random.Range(4, 10f), Random.Range(-7.5f, 7.5f));
        GameObject tempCoin = Instantiate(prefabCoin, treasureChestPosition,Quaternion.identity);
        tempCoin.name = prefabCoin.name;
        Rigidbody coinRB = tempCoin.GetComponent<Rigidbody>();
        coinRB.AddForce(coinPosition, ForceMode.Impulse);

        coins.Add(tempCoin);
    }

    public void SpawnDeadCharacterCoin(Vector3 deadPlayerPosition,int coinAmount)
    {
        GameObject tempGameObject = new GameObject();
        tempGameObject.transform.position = deadPlayerPosition;
        tempGameObject.transform.rotation = Quaternion.identity;

        float modifier;

        for (int i =0;i<coinAmount;i++)
        {
            modifier = i *0.5f;
            Vector3 coinPosition = tempGameObject.transform.forward + tempGameObject.transform.forward * modifier;
            modifier *= 0.25f;
            GameObject tempCoin = Instantiate(prefabCoin, deadPlayerPosition +tempGameObject.transform.forward+tempGameObject.transform.forward * modifier + Vector3.up*modifier, Quaternion.identity);
            tempCoin.name = prefabCoin.name;
            Rigidbody coinRB = tempCoin.GetComponent<Rigidbody>();
            coinRB.AddForce(coinPosition, ForceMode.Impulse);

            coins.Add(tempCoin);

            tempGameObject.transform.Rotate(0, 720/coinAmount, 0);

        }
        Destroy(tempGameObject);
    }

    private void SpawnPowerUp()
    {
        Vector3 tempPos = new Vector3(transform.position.x + Random.Range(-gizmosWidth / 2, gizmosWidth / 2), 1, transform.position.z + Random.Range(-gizmosLength / 2, gizmosLength / 2));
        int randomPowerup = Random.Range(0, 4);
        if (randomPowerup == 0)
        {
            if (currentCoinSackAmount >= maxCoinSacksPerRound)
                SpawnPowerUp();
            else
            {
                GameObject tempPowerup = Instantiate(coinSackPowerup, tempPos, Quaternion.identity);
                tempPowerup.name = "CoinSack";

                tempPowerup.GetComponent<Animator>().speed = Random.Range(0.9f, 1.1f);

                coins.Add(tempPowerup);

                currentCoinSackAmount++;
            }
        }
        else if (randomPowerup == 1)
        {
            if (currentCoinSackAmount >= maxCoinSacksPerRound)
                SpawnPowerUp();
            else
            {
                GameObject tempPowerup = Instantiate(coinSackPowerup, tempPos, Quaternion.identity);
                tempPowerup.name = "CoinSack";

                tempPowerup.GetComponent<Animator>().speed = Random.Range(0.9f, 1.1f);

                coins.Add(tempPowerup);

                currentCoinSackAmount++;
            }
        }
        else if (randomPowerup == 2)
        {
            GameObject tempPowerup = Instantiate(shieldPowerup, tempPos, Quaternion.identity);
            tempPowerup.name = "Shield";

            tempPowerup.GetComponent<Animator>().speed = Random.Range(0.9f, 1.1f);

            pickups.Add(tempPowerup);
            Destroy(tempPowerup, 20);
        }
        else if (randomPowerup == 3)
        {
            GameObject tempPowerup = Instantiate(speedPowerup, tempPos, Quaternion.identity);
            tempPowerup.name = "Speed";

            tempPowerup.GetComponent<Animator>().speed = Random.Range(0.9f, 1.1f);

            pickups.Add(tempPowerup);
            Destroy(tempPowerup, 20);
        }
    }

    public void SpawnPowerUp(Vector3 powerupPosition)
    {
        if (RoundManager.instance.areWeWarming == true) 
            return;

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

                tempPowerup.GetComponent<Animator>().speed = Random.Range(0.9f, 1.1f);

                coins.Add(tempPowerup);

                currentCoinSackAmount++;
            }
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

                tempPowerup.GetComponent<Animator>().speed = Random.Range(0.9f, 1.1f);

                coins.Add(tempPowerup);

                currentCoinSackAmount++;
            }
        }
        else if (randomPowerup == 2)
        {
            GameObject tempPowerup = Instantiate(shieldPowerup, powerupPosition, Quaternion.identity);
            tempPowerup.name = "Shield";
            tempPowerup.GetComponent<Animator>().enabled = true;
            tempPowerup.GetComponent<Animator>().Play("Pickup");
            tempPowerup.GetComponent<BoxCollider>().enabled = true;

            tempPowerup.GetComponent<Animator>().speed = Random.Range(0.9f, 1.1f);

            Destroy(tempPowerup, 20);
        }
        else if (randomPowerup == 3)
        {
            GameObject tempPowerup = Instantiate(speedPowerup, powerupPosition, Quaternion.identity);
            tempPowerup.name = "Speed";
            tempPowerup.GetComponent<Animator>().enabled = true;
            tempPowerup.GetComponent<Animator>().Play("Pickup");
            tempPowerup.GetComponent<BoxCollider>().enabled = true;

            tempPowerup.GetComponent<Animator>().speed = Random.Range(0.9f, 1.1f);

            Destroy(tempPowerup, 20);
        }
    }

    public void SpawnPowerUp(CharacterControl.Weapons weaponToSpawn,Vector3 spawnPos,Transform parentToSpawnUnder)
    {
        for (int i =0;i<weaponPickups.Length;i++)
        {
            if (weaponPickups[i].name == weaponToSpawn.ToString())
            {
                GameObject tempPowerup = Instantiate(weaponPickups[i], spawnPos, Quaternion.identity);
                tempPowerup.transform.SetParent(parentToSpawnUnder);
                tempPowerup.name = weaponPickups[i].name;
                tempPowerup.GetComponent<Animator>().enabled = true;
                tempPowerup.GetComponent<BoxCollider>().enabled = true;
                tempPowerup.GetComponent<Animator>().speed = Random.Range(0.9f, 1.1f);
            }
        }
    }

    public void SpawnPowerUp(PowerupTypes powerupToSpawn, Vector3 spawnPos, Transform parentToSpawnUnder)
    {
        if (powerupToSpawn == PowerupTypes.CoinSack)
        {
            GameObject tempPowerup = Instantiate(coinSackPowerup, spawnPos, Quaternion.identity);
            tempPowerup.transform.SetParent(parentToSpawnUnder);
            tempPowerup.name = "CoinSack";
            tempPowerup.GetComponent<Animator>().enabled = true;
            tempPowerup.GetComponent<BoxCollider>().enabled = true;
            tempPowerup.GetComponent<Animator>().speed = Random.Range(0.9f, 1.1f);

            coins.Add(tempPowerup);

            currentCoinSackAmount++;
        }
        else if (powerupToSpawn == PowerupTypes.Health)
        {
            GameObject tempPowerup = Instantiate(healthPowerup, spawnPos, Quaternion.identity);
            tempPowerup.transform.SetParent(parentToSpawnUnder);
            tempPowerup.name = "Health";
            tempPowerup.GetComponent<Animator>().enabled = true;
            tempPowerup.GetComponent<BoxCollider>().enabled = true;
        }
        if (powerupToSpawn == PowerupTypes.Speed)
        {
            GameObject tempPowerup = Instantiate(speedPowerup, spawnPos, Quaternion.identity);
            tempPowerup.transform.SetParent(parentToSpawnUnder);
            tempPowerup.name = "Speed";
            tempPowerup.GetComponent<Animator>().enabled = true;
            tempPowerup.GetComponent<BoxCollider>().enabled = true;
        }
        if (powerupToSpawn == PowerupTypes.Shield)
        {
            GameObject tempPowerup = Instantiate(shieldPowerup, spawnPos, Quaternion.identity);
            tempPowerup.transform.SetParent(parentToSpawnUnder);
            tempPowerup.name = "Shield";
            tempPowerup.GetComponent<Animator>().enabled = true;
            tempPowerup.GetComponent<BoxCollider>().enabled = true;
        }
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

    public void SetWinningPlayer(CharacterControl player) //transfer this whole function to leaderboard?
    {
        player.SetWinner();

        winningPlayer = player.transform;
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
