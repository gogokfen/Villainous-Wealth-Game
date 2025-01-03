using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using VInspector;
using TMPro;
using Unity.VisualScripting;

public class RoundManager : MonoBehaviour
{
    [Header("Round Settings")]
    public int totalRounds = 5;
    private int currentRound = 0;

    [Header("Player Management")]
    public static bool roundActive = false;


    [SerializeField] ShopManager shopManager;
    [SerializeField] StormManager stormManager;
    public GameObject winner;
    public GameObject[] winnerAndLosers;
    public GameObject controlsUI;
    public bool areWeWarming;
    private void Awake()
    {
        Cursor.visible = false;
    }

    void Start()
    {
        if (CustomizationManager.instance != null) totalRounds = CustomizationManager.instance.roundAmount;
        else totalRounds = 5;

        if (areWeWarming == true) StartCoroutine(WarmupRound());
        else StartCoroutine(RoundLoop());
        
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            DebugEndRound();
        }
        if ((Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.JoystickButton0)) && controlsUI.activeSelf)
        {
            controlsUI.SetActive(false);
            foreach (CharacterControl character in Leaderboard.singleton.characters)
            {
                character.GetComponent<CharacterController>().enabled = true;
            }
        }
    }

    private IEnumerator RoundLoop()
    {
        PickupManager.singleton.DropPowerups = true;
        stormManager.enabled = true;
        if (areWeWarming == false) PlayerManager.instance.StartRound();
        while (currentRound != totalRounds)
        {
            PickupManager.singleton.ResetCoinSackCount(); //resets Amount of Moneybag pickups able to spawn in a round
            Leaderboard.singleton.AnnounceText($"Round {currentRound + 1} / {totalRounds}"); //announce current round
            SoundManager.singleton.PlayNextClip(); //rotates between songs
            CameraManager.instance.PlayersToCameraGroup(); //add all active players to Camera Group
            yield return new WaitUntil(() => PlayerManager.roundOver == true); //waits until round is over
            stormManager.ResetStorm(); //resets storm, remove after alpha
            AssignWinner(); //gives winner of round all money dropped
            yield return new WaitForSeconds(3.5f); //waits for AssignWinner to finish
            Leaderboard.singleton.UpdateLeaderboard(); //shows Leaderboard
            yield return new WaitUntil(() => Leaderboard.singleton.leaderboard.activeSelf == false); //waits until leaderboard is deactivated
            //roundEnd.Invoke(); //Invokes an end of round event, currently does nothing
            currentRound++; //ups the round counter
            if (currentRound != totalRounds) //if game is only one round, it won't trigger shopping
            {
                TimeManager.instance.SlowTime(0f, 10f); //stopping time, to avoid game running when shop is open
                SoundManager.singleton.MaloMart(); //plays Shop Music
                Leaderboard.singleton.EmptyPlayerHands(); //drops currently equipped weapons, makes them single use
                shopManager.Shopping(); //activates the Shop UI and starts the shopping timer
                yield return new WaitUntil(() => shopManager.shopUI.activeSelf == false); //waits for Shopping to end
                PlayerManager.roundOver = false; //resets the bool for the next round
                PlayerManager.instance.PlayersNextRound(); //resets "dead" players prefabs, HP, and positions
            }
            MapManager.instance.ResetMap(); //resets map elements
            stormManager.ResetStorm(); //resets storm    
        }
        AssignUltimateWinnerAndLosers(); //Assigns winner with most coins, and losers, changes their animation accordingly
        WinnerManager.instance.WinnerScene(); //setup Winner Scene, moves winner and losers to positions
    }

    private IEnumerator WarmupRound()
    {
        PlayerManager.instance.StartRound(); //starts round
        PickupManager.singleton.DropPowerups = false;
        stormManager.enabled = false;
        MapManager.instance.Warmup(); //spawns Warmup Protectors equal to players present
        Leaderboard.singleton.AnnounceText("Warmup Round"); //announced that its the warmup round
        SoundManager.singleton.PlayNextClip(); //rotates between songs
        CameraManager.instance.PlayersToCameraGroup(); //add all active players to Camera Group
        controlsUI.SetActive(true);//show Controls
        yield return new WaitUntil(() => controlsUI.activeSelf == false);
        yield return new WaitUntil(() => PlayerManager.roundOver == true); //waits until round is over
        //function to reset kill and money counts
        yield return new WaitForSeconds(3.5f); //waits for AssignWinner to finish
        PlayerManager.roundOver = false; //resets the bool for the next round
        MapManager.instance.ResetMap(); //resets map elements
        PlayerManager.instance.PlayersNextRound(); //resets "dead" players prefabs, HP, and positions
        StartCoroutine(RoundLoop());
    }

    [Button]
    private void DebugEndRound()
    {
        PlayerManager.roundOver = true;
    }

    public static void NextRound()
    {
        CharacterControl[] characters = GameObject.FindObjectsOfType<CharacterControl>();
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

    public void AssignUltimateWinnerAndLosers()
    {
        CharacterControl[] characters = GameObject.FindObjectsOfType<CharacterControl>();
        CharacterControl.PlayerTypes playerIDWinner = CharacterControl.PlayerTypes.Red;
        int mostCoins = 0;
        int players = 0;
        foreach (CharacterControl character in characters)
        {
            int coins = MoneyManager.singleton.GetMoney(character.PlayerID);
            players++;
            if (coins > mostCoins)
            {
                mostCoins = coins;
                playerIDWinner = character.PlayerID;
                winner = character.gameObject;
            }
        }
        winnerAndLosers = new GameObject[players];
        winnerAndLosers[0] = winner;
        winner.GetComponent<CharacterControl>().VictoryOrLose(0);
        int LoserIndex = 1;
        foreach (CharacterControl character in characters)
        {
            if (character.PlayerID != playerIDWinner)
            {
                winnerAndLosers[LoserIndex] = character.gameObject;
                character.GetComponent<CharacterControl>().VictoryOrLose(LoserIndex);
                LoserIndex++;
            }
        }
    }


}
