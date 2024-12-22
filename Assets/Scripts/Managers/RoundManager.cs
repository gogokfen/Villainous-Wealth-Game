using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using VInspector;
using TMPro;

public class RoundManager : MonoBehaviour
{
    [Header("Round Settings")]
    public int totalRounds = 5;
    private int currentRound = 0;

    [Header("Player Management")]
    public static bool roundActive = false;

    [Header("Round Management")]
    //public UnityEvent gameStart;
    //public UnityEvent roundStart;
    //public UnityEvent roundEnd;
    //public UnityEvent gameEnd;
    //[SerializeField] PlayerManager playerManager;
    [SerializeField] ShopManager shopManager;
    [SerializeField] StormManager stormManager;

    [Foldout("Winner UI")]
    [SerializeField] GameObject winnerBG;
    [SerializeField] TextMeshProUGUI winnerText;
    [EndFoldout]
    private void Awake()
    {
        //playerManager = FindAnyObjectByType<PlayerManager>();
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

    }

    void Start()
    {
        if (CustomizationManager.instance !=null) totalRounds = CustomizationManager.instance.roundAmount;
        else totalRounds = 5;
        StartCoroutine(RoundLoop());
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            DebugEndRound();
        }
    }

    private IEnumerator RoundLoop()
    {
        //gameStart.Invoke();
        
        PlayerManager.instance.StartRound(); //starts round
        //PauseMenu.instance.SubToPause();
        while (currentRound != totalRounds)
        {
            CameraManager.instance.PlayersToCameraGroup(); //add all active players to Camera Group
            //Debug.Log($"Round {currentRound + 1} start"); //displays current round
            yield return new WaitUntil(() => PlayerManager.roundOver == true); //waits until round is over
            AssignWinner(); //gives winner of round all money dropped
            yield return new WaitForSeconds(3.5f); //waits for AssignWinner to finish
            //roundEnd.Invoke(); //Invokes an end of round event, currently does nothing
            currentRound++; //ups the round counter
            if (currentRound != totalRounds) //if game is only one round, it won't trigger shopping
            {
                shopManager.Shopping(); //activates the Shop UI and starts the shopping timer
                yield return new WaitUntil(() => shopManager.shopUI.activeSelf == false); //waits for Shopping to end
                PlayerManager.roundOver = false; //resets the bool for the next round
                PlayerManager.instance.PlayersNextRound(); //resets "dead" players prefabs, HP, and positions
                //playerManager.PlayersNextRound(); 
            }
            MapManager.instance.ResetMap(); //resets map elements, currently only respawns the chest
            stormManager.ResetStorm(); //resets storm
            //NextRound(); not necessary anymore since PlayersNextRound does it anyway        
        }
        AssignUltimateWinner(); //Assigns winner with most coins
        //gameEnd.Invoke();
    }

    [Button]
    private void DebugEndRound()
    {
        PlayerManager.roundOver = true;
    }

    public static void NextRound()
    {
        CharacterControl[] characters = GameObject.FindObjectsOfType<CharacterControl>();
        /*
        for (int i=0;i<characters.Length;i++)
        {
            if (characters[i].dead == false)
            {
                PickupManager.singleton.SetWinningPlayer(characters[i].gameObject);
            }
        }
        */
        foreach (CharacterControl character in characters)
        {
            character.NextRound();
        }
    }

    public void AssignWinner()
    {
        CharacterControl[] characters = GameObject.FindObjectsOfType<CharacterControl>();
        for (int i = 0; i < characters.Length; i++)
        {
            if (characters[i].dead == false)
            {
                PickupManager.singleton.SetWinningPlayer(characters[i].gameObject);
                characters[i].gameObject.GetComponent<CharacterController>().enabled = false;
            }
        }
    }

    public void AssignUltimateWinner()
    {
        CharacterControl[] characters = GameObject.FindObjectsOfType<CharacterControl>();
        string playerIDWinner = null;
        int mostCoins = 0;

        foreach (CharacterControl character in characters)
        {
            int coins = character.coins;

            if (coins > mostCoins)
            {
                mostCoins = coins;
                playerIDWinner = character.HeadGFX.name;
            }
        }

        if (playerIDWinner != null)
        {
            winnerBG.SetActive(true);
            if (mostCoins == 1) winnerText.text = $"{playerIDWinner} Wins with {mostCoins} coin!";
            else winnerText.text = $"{playerIDWinner} Wins with {mostCoins} coins!";
        }
    }

}
