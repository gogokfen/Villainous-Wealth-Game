using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using VInspector;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;

public class RoundManager : MonoBehaviour
{
    public static RoundManager instance;
    [Header("Round Settings")]
    public int totalRounds = 5;
    public int currentRound = 0;

    [Header("Player Management")]
    public static bool roundActive = false;

    [SerializeField] Animator curtain;
    [SerializeField] ShopManager shopManager;
    [SerializeField] StormManager stormManager;
    [SerializeField] ShopStall shopStall;
    [SerializeField] RoundStart roundStart;

    public GameObject controlsUI;
    public bool areWeWarming;
    bool roundstart;
    private void Awake()
    {
        instance = this;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
    }

    void Start()
    {
        if (CustomizationManager.instance != null) totalRounds = CustomizationManager.instance.roundAmount;
        else totalRounds = 5;
        
        
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
            Leaderboard.singleton.EnableCharacterController();
        }
        if (SceneManager.GetActiveScene().name == "MainScene" && !roundstart)
        {
            if (areWeWarming == true) StartCoroutine(WarmupRound());
            else StartCoroutine(RoundLoop());

            roundstart = true;
        }
    }

    private IEnumerator RoundLoop()
    { 
        curtain.gameObject.SetActive(true);
        curtain.Play("Curtain");
        stormManager.enabled = true;
        yield return new WaitUntil(() => curtain.GetCurrentAnimatorStateInfo(0).IsName("Curtain"));
        if (areWeWarming == false)
        {
            PlayerManager.instance.StartRound();
            Leaderboard.singleton.FindPlayers();
        }
        areWeWarming = false;
        while (currentRound != totalRounds)
        {
            PickupManager.singleton.ResetCoinSackCount(); //resets Amount of Moneybag pickups able to spawn in a round
            //Leaderboard.singleton.AnnounceText($"Round {currentRound + 1} / {totalRounds}"); //announce current round
            SoundManager.singleton.PlayNextClip(); //rotates between songs
            CameraManager.instance.PlayersToCameraGroup(); //add all active players to Camera Group
            Leaderboard.singleton.TurnOffPlayersGFX();
            yield return new WaitForSeconds(1f);
            StartCoroutine(roundStart.Intro());
            yield return new WaitUntil(() => roundStart.playing == false);
            PickupManager.singleton.DropPowerups = true;
            Leaderboard.singleton.EnableCharacterWeapons();
            yield return new WaitUntil(() => PlayerManager.instance.roundOver == true); //waits until round is over
            stormManager.ResetStorm(); //resets storm, remove after alpha
            PickupManager.singleton.DropPowerups = false;
            Leaderboard.singleton.AssignWinner(); //gives winner of round all money dropped
            yield return new WaitForSeconds(3.5f); //waits for AssignWinner to finish
            Leaderboard.singleton.EmptyPlayerHands(); //drops currently equipped weapons, makes them single use
            Leaderboard.singleton.UpdateLeaderboard(); //shows Leaderboard
            PickupManager.singleton.DestroyAllPickups();
            yield return new WaitForSeconds(6f); //time to see leaderboards
            Leaderboard.singleton.ResetLeaderboard(); //waits until leaderboard is deactivated
            currentRound++; //ups the round counter
            if (currentRound != totalRounds) //if game is only one round, it won't trigger shopping
            {
                StartCoroutine(shopStall.StallTime());
                shopManager.WantedPosters();
                yield return new WaitUntil(() => shopStall.fadingFirst == false);
                //TimeManager.instance.SlowTime(0f, 10f); //stopping time, to avoid game running when shop is open
                TimeManager.instance.StopTime();
                SoundManager.singleton.MaloMart(); //plays Shop Music
                Leaderboard.singleton.DisableCharacterWeapons();
                shopManager.Shopping(); //activates the Shop UI and starts the shopping timer
                yield return new WaitUntil(() => shopManager.shopping == false); //waits for Shopping to end
                StartCoroutine(shopStall.StallTimeOver());
                yield return new WaitUntil(() => shopStall.fadingSecond == false);
                shopManager.shopUI.SetActive(false);
                TimeManager.instance.ResumeTime();
                //shopStall.fading = false;
                PlayerManager.instance.roundOver = false; //resets the bool for the next round
                Leaderboard.singleton.NextRound(); //resets "dead" players prefabs, HP, and positions
            }
            MapManager.instance.ResetMap(); //resets map elements
            stormManager.ResetStorm(); //resets storm    
        }
        Leaderboard.singleton.AssignUltimateWinnerAndLosers(); //Assigns winner with most coins, and losers, changes their animation accordingly
        WinnerManager.instance.WinnerScene(); //setup Winner Scene, moves winner and losers to positions
    }

    private IEnumerator WarmupRound()
    {
        curtain.gameObject.SetActive(true);
        curtain.Play("Curtain");
        PlayerManager.instance.StartRound(); //starts round
        Leaderboard.singleton.FindPlayers();
        Leaderboard.singleton.StartWarmupRound(); //Osher added
        PickupManager.singleton.DropPowerups = false;
        stormManager.enabled = false;
        MapManager.instance.Warmup(); //spawns Warmup Protectors equal to players present
        Leaderboard.singleton.AnnounceText("Warmup Round"); //announced that its the warmup round
        SoundManager.singleton.PlayNextClip(); //rotates between songs
        CameraManager.instance.PlayersToCameraGroup(); //add all active players to Camera Group
        controlsUI.SetActive(true); //show Controls
        yield return new WaitUntil(() => controlsUI.activeSelf == false);
        //yield return new WaitUntil(() => PlayerManager.instance.roundOver == true); //waits until round is over
        yield return new WaitForSeconds(20f); //Osher Changed
        Leaderboard.singleton.AssignWinner();
        //function to reset kill and money counts
        yield return new WaitForSeconds(3.5f); //waits for AssignWinner to finish
        Leaderboard.singleton.EmptyPlayerHands(); //Osher added
        Leaderboard.singleton.ResetPlayersMoney();
        PlayerManager.instance.roundOver = false; //resets the bool for the next round
        MapManager.instance.ResetMap(); //resets map elements
        Leaderboard.singleton.NextRound(); //resets "dead" players prefabs, HP, and positions
        StartCoroutine(RoundLoop());
    }

    [Button]
    private void DebugEndRound()
    {
        PlayerManager.instance.roundOver = true;
    }
}
