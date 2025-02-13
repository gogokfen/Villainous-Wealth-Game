using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using VInspector;
using UnityEngine.UI;
using Unity.VisualScripting;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

public class Leaderboard : MonoBehaviour
{
    public static Leaderboard singleton { get; private set; }

    public TextMeshProUGUI killsAnnouncer;
    public Animator killsAnnouncerAnimation;

    [SerializeField] public GameObject leaderboard;

    [SerializeField] GameObject[] leaderboardPlayerPrefabs;

    [SerializeField] Sprite[] portraits;

    [Foldout("Red Player")]
    [SerializeField] TextMeshProUGUI redPlayerName;
    [SerializeField] TextMeshProUGUI redPlayerCoins;
    [SerializeField] TextMeshProUGUI redPlayerCoinsRanking;
    [SerializeField] TextMeshProUGUI redPlayerKills;
    [SerializeField] TextMeshProUGUI redPlayerRank;
    [SerializeField] Image redPlayerPortrait;
    [SerializeField] Image redPlayerPortraitRanking;
    [SerializeField] RectTransform redRankingUI;
    [EndFoldout]
    [Foldout("Green Player")]
    [SerializeField] TextMeshProUGUI greenPlayerName;
    [SerializeField] TextMeshProUGUI greenPlayerCoins;
    [SerializeField] TextMeshProUGUI greenPlayerCoinsRanking;
    [SerializeField] TextMeshProUGUI greenPlayerKills;
    [SerializeField] TextMeshProUGUI greenPlayerRank;
    [SerializeField] Image greenPlayerPortrait;
    [SerializeField] Image greenPlayerPortraitRanking;
    [SerializeField] RectTransform greenRankingUI;
    [EndFoldout]
    [Foldout("Blue Player")]
    [SerializeField] TextMeshProUGUI bluePlayerName;
    [SerializeField] TextMeshProUGUI bluePlayerCoins;
    [SerializeField] TextMeshProUGUI bluePlayerCoinsRanking;
    [SerializeField] TextMeshProUGUI bluePlayerKills;
    [SerializeField] TextMeshProUGUI bluePlayerRank;
    [SerializeField] Image bluePlayerPortrait;
    [SerializeField] Image bluePlayerPortraitRanking;
    [SerializeField] RectTransform blueRankingUI;
    [EndFoldout]
    [Foldout("Yellow Player")]
    [SerializeField] TextMeshProUGUI yellowPlayerName;
    [SerializeField] TextMeshProUGUI yellowPlayerCoins;
    [SerializeField] TextMeshProUGUI yellowPlayerCoinsRanking;
    [SerializeField] TextMeshProUGUI yellowPlayerKills;
    [SerializeField] TextMeshProUGUI yellowPlayerRank;
    [SerializeField] Image yellowPlayerPortrait;
    [SerializeField] Image yellowPlayerPortraitRanking;
    [SerializeField] RectTransform yellowRankingUI;
    [EndFoldout]

    public RectTransform[] rankingTransforms;

    public int playerCount;

    private struct PlayerStats
    {
        public PlayerStats(CharacterControl characterReference, MultiplayerEventSystem eventSystem, PlayerInput input, CharacterController controller, Sprite portrait, Transform transform, CharacterControl.PlayerTypes color, string name, int roundStartMoney, int currentMoney, int kills, int deaths, bool wonThisRound, int rank)
        {
            this.characterReference = characterReference;
            this.eventSystem = eventSystem;
            this.input = input;
            this.controller = controller;
            this.portrait = portrait;
            this.transform = transform;
            this.color = color;
            this.name = name;
            this.roundStartMoney = roundStartMoney;
            this.currentMoney = currentMoney;
            this.kills = kills;
            this.deaths = deaths;
            this.rank = rank;
            this.wonThisRound = wonThisRound;

        }

        public CharacterControl characterReference;
        public CharacterControl.PlayerTypes color;
        public MultiplayerEventSystem eventSystem;
        public PlayerInput input;
        public CharacterController controller;
        public Sprite portrait;
        public Transform transform;
        public string name;
        public int roundStartMoney;
        public int currentMoney;
        public int kills;
        public int deaths;
        public int rank;
        public bool wonThisRound;
    }
    /*
    PlayerStats redPlayer = new PlayerStats
                    (null,                            //character Reference
                    null,                             //Event System
                    null,                             //Player Input  
                    null,                             //Character Controller
                    null,                             //Portrait
                    null,                             //Transform
                    CharacterControl.PlayerTypes.Red, //player color
                    "",                               //name
                    0,                                //round start money
                    0,                                //current money
                    0,                                //kills
                    0,                                //deaths
                    false,                            //won this round
                    -1);                               //Rank  
    PlayerStats greenPlayer = new PlayerStats(null, null, null, null, null, null, CharacterControl.PlayerTypes.Green, "", 0, 0, 0, 0, false, -1);
    PlayerStats bluePlayer = new PlayerStats(null, null, null, null, null, null, CharacterControl.PlayerTypes.Blue, "", 0, 0, 0, 0, false, -1);
    PlayerStats yellowPlayer = new PlayerStats(null, null, null, null, null, null, CharacterControl.PlayerTypes.Yellow, "", 0, 0, 0, 0, false, -1);
    */
    private PlayerStats[] players;

    private struct Ranking
    {
        public int money;
        public CharacterControl.PlayerTypes color;
    }

    //private int[] moneyRankings;
    private Ranking[] ranking;

    public void FindPlayers()
    {
        CharacterControl[] characters;
        characters = GameObject.FindObjectsOfType<CharacterControl>();

        players = new PlayerStats[characters.Length];
        //moneyRankings = new int[playerCount];
        ranking = new Ranking[characters.Length];

        foreach (CharacterControl character in characters)
        {
            if (character.PlayerID == CharacterControl.PlayerTypes.Red)
            {
                leaderboardPlayerPrefabs[0].SetActive(true);
                players[0].color = CharacterControl.PlayerTypes.Red;
                players[0].characterReference = character;
                players[0].name = character.HeadGFX.name;
                redPlayerPortrait.sprite = SetPlayerPortrait(players[0].name);
                redPlayerPortraitRanking.sprite = SetPlayerPortrait(players[0].name);
                players[0].portrait = redPlayerPortrait.sprite;
                redPlayerName.text = NameSelectedButton(players[0].name);
                players[0].rank = -1;
                players[0].eventSystem = character.gameObject.GetComponent<MultiplayerEventSystem>();
                players[0].input = character.gameObject.GetComponent<PlayerInput>();
                players[0].controller = character.gameObject.GetComponent<CharacterController>();
                players[0].transform = character.gameObject.transform;
                redRankingUI.gameObject.SetActive(true);
            }

            else if (character.PlayerID == CharacterControl.PlayerTypes.Green)
            {
                leaderboardPlayerPrefabs[1].SetActive(true);
                players[1].color = CharacterControl.PlayerTypes.Green;
                players[1].characterReference = character;
                players[1].name = character.HeadGFX.name;
                greenPlayerPortrait.sprite = SetPlayerPortrait(players[1].name);
                greenPlayerPortraitRanking.sprite = SetPlayerPortrait(players[1].name);
                players[1].portrait = greenPlayerPortrait.sprite;
                greenPlayerName.text = NameSelectedButton(players[1].name);
                players[1].rank = -1;
                players[1].eventSystem = character.gameObject.GetComponent<MultiplayerEventSystem>();
                players[1].input = character.gameObject.GetComponent<PlayerInput>();
                players[1].controller = character.gameObject.GetComponent<CharacterController>();
                players[1].transform = character.gameObject.transform;
                greenRankingUI.gameObject.SetActive(true);
            }
            else if (character.PlayerID == CharacterControl.PlayerTypes.Blue)
            {
                leaderboardPlayerPrefabs[2].SetActive(true);
                players[2].color = CharacterControl.PlayerTypes.Blue;
                players[2].characterReference = character;
                players[2].name = character.HeadGFX.name;
                bluePlayerPortrait.sprite = SetPlayerPortrait(players[2].name);
                bluePlayerPortraitRanking.sprite = SetPlayerPortrait(players[2].name);
                players[2].portrait = bluePlayerPortrait.sprite;
                bluePlayerName.text = NameSelectedButton(players[2].name);
                players[2].rank = -1;
                players[2].eventSystem = character.gameObject.GetComponent<MultiplayerEventSystem>();
                players[2].input = character.gameObject.GetComponent<PlayerInput>();
                players[2].controller = character.gameObject.GetComponent<CharacterController>();
                players[2].transform = character.gameObject.transform;
                blueRankingUI.gameObject.SetActive(true);
            }
            else if (character.PlayerID == CharacterControl.PlayerTypes.Yellow)
            {
                leaderboardPlayerPrefabs[3].SetActive(true);
                players[3].color = CharacterControl.PlayerTypes.Yellow;
                players[3].characterReference = character;
                players[3].name = character.HeadGFX.name;
                yellowPlayerPortrait.sprite = SetPlayerPortrait(players[3].name);
                yellowPlayerPortraitRanking.sprite = SetPlayerPortrait(players[3].name);
                players[3].portrait = yellowPlayerPortrait.sprite;
                yellowPlayerName.text = NameSelectedButton(players[3].name);
                players[3].rank = -1;
                players[3].eventSystem = character.gameObject.GetComponent<MultiplayerEventSystem>();
                players[3].input = character.gameObject.GetComponent<PlayerInput>();
                players[3].controller = character.gameObject.GetComponent<CharacterController>();
                players[3].transform = character.gameObject.transform;
                yellowRankingUI.gameObject.SetActive(true);
            }
            playerCount++;
        }
    }

    private void Awake()
    {
        singleton = this;
    }

    public void AnnounceKill(CharacterControl.PlayerTypes killingPlayer, CharacterControl.PlayerTypes dyingPlayer)
    {
        if (RoundManager.instance.areWeWarming == true)
            return;

        ModifyMoney(killingPlayer, 5); //giving money to the killer

        players[(int)killingPlayer].characterReference.GetKillingMoney(5);

        killsAnnouncer.text = $"<sprite name=\"{players[(int)killingPlayer].name}\">  has killed " + $"<sprite name=\"{players[(int)dyingPlayer].name}\">";
        players[(int)killingPlayer].kills++;
        players[(int)dyingPlayer].deaths++;

        killsAnnouncerAnimation.Play("Announcement");
    }

    public void AnnounceKill(CharacterControl.PlayerTypes playerWhoDiedToZone) //in case of player killed by the zone
    {
        killsAnnouncer.text = "The Zone has killed " + $"<sprite name=\"{players[(int)playerWhoDiedToZone].name}\">";
        players[(int)playerWhoDiedToZone].deaths++;

        killsAnnouncerAnimation.Play("Announcement");
    }

    public void ResetPlayersMoney()
    {
        for (int i=0;i<players.Length;i++)
        {
            players[i].currentMoney = 0;
            players[i].roundStartMoney = 0;
        }
    }

    public void UpdateLeaderboard()
    {
        //leaderboardActive = !leaderboardActive;
        leaderboard.SetActive(true);

        if (leaderboard.activeSelf)
        {
            /*
            for (int i =0;i<players.Length;i++)
            {
                players[i].roundStartMoney = MoneyManager.singleton.GetRoundMoney(players[i].color);
                players[i].currentMoney = MoneyManager.singleton.GetMoney(players[i].color);
            }
            */
            if (leaderboardPlayerPrefabs[0].activeSelf)
            {
                redPlayerCoins.text = players[0].currentMoney.ToString();
                redPlayerKills.text = players[0].kills.ToString();
            }
            if (leaderboardPlayerPrefabs[1].activeSelf)
            {
                greenPlayerCoins.text = players[1].currentMoney.ToString();
                greenPlayerKills.text = players[1].kills.ToString();

            }
            if (leaderboardPlayerPrefabs[2].activeSelf)
            {
                bluePlayerCoins.text = players[2].currentMoney.ToString();
                bluePlayerKills.text = players[2].kills.ToString();
            }
            if (leaderboardPlayerPrefabs[3].activeSelf)
            {
                yellowPlayerCoins.text = players[3].currentMoney.ToString();
                yellowPlayerKills.text = players[3].kills.ToString();
            }
        }
    }

    public void UpdateRanking()
    {
        /*
        Debug.Log(redRankingUI.position);
        Debug.Log(redRankingUI.rect);
        Debug.Log(redRankingUI.transform.position);

        Debug.Log(redRankingUI.localPosition);

        Debug.Log(redRankingUI.pivot);
        Debug.Log(redRankingUI.sizeDelta);
        Debug.Log(redRankingUI.rect.position);

        */
        if (playerCount > 0)
        {
            //Red Player
            redPlayerCoins.text = players[0].currentMoney.ToString();
            redPlayerCoinsRanking.text = players[0].currentMoney.ToString();
            redPlayerKills.text = players[0].kills.ToString();
            redPlayerRank.text = players[0].rank.ToString();

            //Debug.Log("red"+redRankingUI.localPosition);
            //Debug.Log("Red"+rankingTransforms[redPlayer.rank - 1].localPosition);

            //Debug.Log(players[0].rank);
            redRankingUI.localPosition = rankingTransforms[(players[0].rank - 1)].localPosition;
            //redRankingUI.localPosition = rankingTransforms[0].localPosition;

            //Debug.Log("red"+redRankingUI.localPosition);
            //Debug.Log("Red"+rankingTransforms[redPlayer.rank - 1].localPosition);
        }
        if (playerCount > 1)
        {
            //Green Player
            greenPlayerCoins.text = players[1].currentMoney.ToString();
            greenPlayerCoinsRanking.text = players[1].currentMoney.ToString();
            greenPlayerKills.text = players[1].kills.ToString();
            greenPlayerRank.text = players[1].rank.ToString();
            //Debug.Log("green" + greenRankingUI.localPosition);
            //Debug.Log("Green" + rankingTransforms[greenPlayer.rank - 1].localPosition);

            //Debug.Log(players[1].rank);
            greenRankingUI.localPosition = rankingTransforms[(players[1].rank-1)].localPosition;
            //greenRankingUI.localPosition = rankingTransforms[0].localPosition;

            //Debug.Log("green" + greenRankingUI.localPosition);
            //Debug.Log("Green" + rankingTransforms[greenPlayer.rank - 1].localPosition);
        }
        if (playerCount > 2)
        {
            //Blue Player
            bluePlayerCoins.text = players[2].currentMoney.ToString();
            bluePlayerCoinsRanking.text = players[2].currentMoney.ToString();
            bluePlayerKills.text = players[2].kills.ToString();
            bluePlayerRank.text = players[2].rank.ToString();

            blueRankingUI.localPosition = rankingTransforms[(players[2].rank-1)].localPosition;
        }
        if (playerCount > 3) 
        {
            //Yellow Player
            yellowPlayerCoins.text = players[3].currentMoney.ToString();
            yellowPlayerCoinsRanking.text = players[3].currentMoney.ToString();
            yellowPlayerKills.text = players[3].kills.ToString();
            yellowPlayerRank.text = players[3].rank.ToString();

            yellowRankingUI.localPosition = rankingTransforms[(players[3].rank-1)].localPosition;
        }
    }
    public void EmptyPlayerHands()
    {
        for (int i = 0; i < players.Length; i++)
        {
            players[i].characterReference.EmptyHand();
        }
    }

    public GameObject GetPlayerReference(CharacterControl.PlayerTypes playerColor)
    {
        return players[(int)playerColor].characterReference.gameObject;
    }

    public string GetPlayerName(CharacterControl.PlayerTypes playerColor)
    {
        return players[(int)playerColor].name;
    }

    public PlayerInput[] GetPlayerInputs()
    {
        PlayerInput[] PI = new PlayerInput[playerCount];
        for (int i = 0; i < playerCount; i++)
        {
            PI[i] = players[i].input;
        }
        return PI;
    }

    public Sprite SetPlayerPortrait(string name)
    {
        switch (name)
        {
            case "Dragon":
                return portraits[0];

            case "Capitalist":
                return portraits[1];

            case "Raccoon":
                return portraits[2];

            case "Conquistadorette":
                return portraits[3];

            case "Orc":
                return portraits[4];

            case "Cat":
                return portraits[5];

            case "Leprechaun":
                return portraits[6];

            case "Mafia":
                return portraits[7];

            case "Pirate":
                return portraits[8];

            case "Shark":
                return portraits[9];
            default: return null;
        }
    }

    public string NameSelectedButton(string name)
    {
        switch (name)
        {
            case "Dragon":
                return "Bob";
            case "Capitalist":
                return "Cappy Talist";
            case "Raccoon":
                return "Robin Banks";
            case "Conquistadorette":
                return "Emba Zelment";
            case "Orc":
                return "Buzzuz";
            case "Cat":
                return "Tabby Treasure";
            case "Leprechaun":
                return "Goldie Gone";
            case "Mafia":
                return "Tex Avesion";
            case "Pirate":
                return "Gol D. Iger";
            case "Shark":
                return "Loany";
            default: return null;
        }
    }
    public void AnnounceText(string text)
    {
        killsAnnouncer.text = text;
        killsAnnouncerAnimation.Play("Announcement");
    }

    public void StopForwardMomentum(CharacterControl.PlayerTypes playerColor)
    {
        players[(int)playerColor].characterReference.StopForwardMomentum();
    }

    public void DisableCharacterControl()
    {
        for (int i = 0; i < players.Length; i++)
        {
            //characters[i].GetComponent<CharacterControl>().enabled = false;
            players[i].characterReference.DisableWeaponScripts();
        }
    }
    public void EnableCharacterControl()
    {
        for (int i = 0; i < players.Length; i++)
        {
            //characters[i].GetComponent<CharacterControl>().enabled = true;
            players[i].characterReference.EnableWeaponScripts();

        }
    }

    public void DisableCharacterController()
    {
        for (int i = 0; i < players.Length; i++)
        {
            //characters[i].GetComponent<CharacterControl>().enabled = false;
            players[i].controller.enabled = false;
        }
    }
    public void EnableCharacterController()
    {
        for (int i = 0; i < players.Length; i++)
        {
            //characters[i].GetComponent<CharacterControl>().enabled = false;
            players[i].controller.enabled = true;
        }
    }

    public void DisplayPlayerSacks()
    {
        for (int i = 0; i < players.Length; i++)
        {
            players[i].characterReference.DisplaySack();
        }
    }

    public void ModifyMoney(CharacterControl.PlayerTypes playerColor, int amount)
    {
        players[(int)playerColor].currentMoney += amount;

        if (players[(int)playerColor].currentMoney < 0)
            players[(int)playerColor].currentMoney = 0;

        FindLeader();
        ArrangeMoneyRanking();
        UpdateRanking();
    }

    public int GetMoney(CharacterControl.PlayerTypes playerColor)
    {
        return players[(int)playerColor].currentMoney;
    }

    public void FindLeader()
    {
        CharacterControl.PlayerTypes leader = CharacterControl.PlayerTypes.Red;

        int leaderIndex = 0;
        for (int i = 1; i < players.Length; i++)
        {
            if (players[i].currentMoney > players[leaderIndex].currentMoney)
            {
                leaderIndex = i;
                leader = players[i].color;
            }
        }

        for (int i = 0; i < players.Length; i++)
        {
            players[i].characterReference.SetLeader(leader);
        }
    }

    public PlayerInput FindLeaderInput()
    {
        PlayerInput leaderInput = players[0].characterReference.gameObject.GetComponent<PlayerInput>();

        int leaderIndex = 0;
        for (int i = 1; i < players.Length; i++)
        {
            if (players[i].currentMoney > players[leaderIndex].currentMoney)
            {
                leaderIndex = i;
                leaderInput = players[i].characterReference.gameObject.GetComponent<PlayerInput>();
            }
        }
        return leaderInput;
    }

    public MultiplayerEventSystem FindLeaderEventSystem()
    {
        MultiplayerEventSystem leaderEventSystem = players[0].eventSystem;

        int leaderIndex = 0;
        for (int i = 1; i < players.Length; i++)
        {
            if (players[i].currentMoney > players[leaderIndex].currentMoney)
            {
                leaderIndex = i;
                leaderEventSystem = players[i].eventSystem;
            }
        }
        return leaderEventSystem;
    }

    public CharacterControl.PlayerTypes FindLeaderColor()
    {
        CharacterControl.PlayerTypes leaderColor = CharacterControl.PlayerTypes.Red;

        int leaderIndex = 0;
        for (int i = 1; i < players.Length; i++)
        {
            if (players[i].currentMoney > players[leaderIndex].currentMoney)
            {
                leaderIndex = i;
                leaderColor = players[i].color;
            }
        }

        return leaderColor;
    }

    public string FindLeaderName()
    {
        int leaderIndex = 0;
        for (int i = 1; i < players.Length; i++)
        {
            if (players[i].currentMoney > players[leaderIndex].currentMoney)
            {
                leaderIndex = i;
                //leaderName = players[i].name;
            }
        }

        return players[leaderIndex].name;
    }

    public int FindLeaderMoney()
    {
        return players[(int)FindLeaderColor()].currentMoney; //probably the most bizzare code line I wrote in a while
    }

    public void UpdateRoundMoney(CharacterControl.PlayerTypes playerColor, int roundMoney)
    {
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].color == playerColor)
                players[i].roundStartMoney = roundMoney;
        }
    }

    public void ArrangeMoneyRanking()
    {
        //oh yeah, madmah time!

        Ranking[] tempArray = new Ranking[players.Length];

        //int[] tempArray = new int[moneyRankings.Length];

        //CharacterControl.PlayerTypes[] playersRankingColor = null;
        //GameObject[] tempPlayers = new GameObject[moneyRankings.Length];

        //for security:
        for (int i=0;i<ranking.Length;i++)
        {
            ranking[i].money = -10;
            ranking[i].color = CharacterControl.PlayerTypes.Yellow;
        }

        int indexToRemember = 0;

        for (int i = 0; i < ranking.Length; i++)
        {
            tempArray[i].money = players[i].currentMoney;
            tempArray[i].color = players[i].color;
            //tempArray[i] = players[i].currentMoney;

            //playersRankingColor[i] = players[i].color;
            //tempPlayers[i] = players[i].characterReference.gameObject;
        }

        for (int i = 0; i < ranking.Length; i++)
        {
            for (int j = 0; j < ranking.Length; j++)
            {
                if (tempArray[j].money > ranking[i].money)
                {
                    ranking[i].money = tempArray[j].money;
                    ranking[i].color = tempArray[j].color;
                    //playersRankingColor[i] = 
                    //players[i].rank = i +1; //does this work as intended?
                    //playerRankings[i] = tempPlayers[j];
                    indexToRemember = j;
                }
                tempArray[indexToRemember].money = -1; //move to lower for?
            }
        }
        for (int i = 0; i < ranking.Length; i++)
        {
            players[(int)ranking[i].color].rank = (i + 1); // for instance if if the array ranking contains a yellow player on the first place -> [0] that means that that players[3] = 1 
            //Debug.Log(players[(int)ranking[i].color].rank+""+ players[(int)ranking[i].color].color);
        }

        //Array.Sort(moneyRankings);
    }

    public int DeathMoney(CharacterControl.PlayerTypes deadPlayerColor)
    {
        int moneylost = 0;

        if (players.Length == 4)
        {
            if (players[(int)deadPlayerColor].rank == 1)
                moneylost = players[(int)deadPlayerColor].currentMoney / 2; //50%
            else if (players[(int)deadPlayerColor].rank == 2)
                moneylost = players[(int)deadPlayerColor].currentMoney / 4; //25%
            else if (players[(int)deadPlayerColor].rank == 3)
                moneylost = players[(int)deadPlayerColor].currentMoney / 10; //10%
            else
                moneylost = 0;

            ModifyMoney(deadPlayerColor, -moneylost);

        }
        if (players.Length == 3)
        {
            if (players[(int)deadPlayerColor].rank == 1)
                moneylost = players[(int)deadPlayerColor].currentMoney / 2; //50%
            else if (players[(int)deadPlayerColor].rank == 2)
                moneylost = players[(int)deadPlayerColor].currentMoney / 4; //25%
            else
                moneylost = players[(int)deadPlayerColor].currentMoney / 10; //10%

            ModifyMoney(deadPlayerColor, -moneylost);

        }
        if (players.Length == 2)
        {
            if (players[(int)deadPlayerColor].rank == 1)
                moneylost = (int)(players[(int)deadPlayerColor].currentMoney * 0.4f); //40%
            else
                moneylost = (int)(players[(int)deadPlayerColor].currentMoney * 0.2f); //20%

            ModifyMoney(deadPlayerColor, -moneylost);

        }

        return moneylost;
    }

    public void AssignWinner()
    {
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].characterReference.dead == false)
            {
                PickupManager.singleton.SetWinningPlayer(players[i].characterReference);
                players[i].controller.enabled = false;
            }
        }
    }

    public void AssignUltimateWinnerAndLosers()
    {
        ArrangeMoneyRanking();
        for (int i = 0; i < players.Length; i++)
        {
            players[i].characterReference.VictoryOrLose((players[i].rank - 1)); //highest rank is 1, not 0
        }
    }

    public void ArrangeOnPodium(Transform[] positions)
    {
        ArrangeMoneyRanking();
        for (int i = 0; i < players.Length; i++)
        {
            players[i].characterReference.enabled = false;
            players[i].characterReference.bodyPartsGFX[6].SetActive(false); //Hides the player's UI
            players[i].characterReference.bodyPartsGFX[7].SetActive(false);
            players[i].characterReference.transform.position = positions[(players[i].rank - 1)].position; //rankings start from 1, positions from 0
            players[i].characterReference.transform.rotation = positions[(players[i].rank - 1)].rotation;
        }
    }

    public void NextRound()
    {
        PlayerManager.instance.playerCount = playerCount;
        for (int i = 0; i < players.Length; i++)
        {
            players[i].characterReference.NextRound();
            players[i].transform.position = MapManager.instance.startPositions[i].position;
        }
    }

    public void StartWarmupRound()
    {
        PlayerManager.instance.playerCount = playerCount;
        for (int i = 0; i < players.Length; i++)
        {
            players[i].characterReference.WarmupRound();
            players[i].transform.position = MapManager.instance.startPositions[i].position;
        }
    }

    /**
     *         foreach (CharacterControl character in characters)
        {
            if (character.PlayerID == CharacterControl.PlayerTypes.Red)
            {
                leaderboardPlayerPrefabs[0].SetActive(true);
                redPlayer.characterReference = character;
                redPlayer.name = character.HeadGFX.name;
                redPlayerPortrait.sprite = SetPlayerPortrait(redPlayer.name);
                redPlayerPortraitRanking.sprite = SetPlayerPortrait(redPlayer.name);
                redPlayer.portrait = redPlayerPortrait.sprite;
                redPlayerName.text = redPlayer.name;
                //redPlayer.rank = -1; //1
                redPlayer.eventSystem = character.gameObject.GetComponent<MultiplayerEventSystem>();
                redPlayer.input = character.gameObject.GetComponent<PlayerInput>();
                redPlayer.controller = character.gameObject.GetComponent<CharacterController>();
                redPlayer.transform = character.gameObject.transform;
                redRankingUI.gameObject.SetActive(true);
            }

            else if (character.PlayerID == CharacterControl.PlayerTypes.Green)
            {
                leaderboardPlayerPrefabs[1].SetActive(true);
                greenPlayer.characterReference = character;
                greenPlayer.name = character.HeadGFX.name;
                greenPlayerPortrait.sprite = SetPlayerPortrait(greenPlayer.name);
                greenPlayerPortraitRanking.sprite = SetPlayerPortrait(greenPlayer.name);
                greenPlayer.portrait = greenPlayerPortrait.sprite;
                greenPlayerName.text = greenPlayer.name;
                //greenPlayer.rank = -1; //2
                greenPlayer.eventSystem = character.gameObject.GetComponent<MultiplayerEventSystem>();
                greenPlayer.input = character.gameObject.GetComponent<PlayerInput>();
                greenPlayer.controller = character.gameObject.GetComponent<CharacterController>();
                greenPlayer.transform = character.gameObject.transform;
                greenRankingUI.gameObject.SetActive(true);
            }
            else if (character.PlayerID == CharacterControl.PlayerTypes.Blue)
            {
                leaderboardPlayerPrefabs[2].SetActive(true);
                bluePlayer.characterReference = character;
                bluePlayer.name = character.HeadGFX.name;
                bluePlayerPortrait.sprite = SetPlayerPortrait(bluePlayer.name);
                bluePlayerPortraitRanking.sprite = SetPlayerPortrait(bluePlayer.name);
                bluePlayer.portrait = bluePlayerPortrait.sprite;
                bluePlayerName.text = bluePlayer.name;
                //bluePlayer.rank = -1; //3
                bluePlayer.eventSystem = character.gameObject.GetComponent<MultiplayerEventSystem>();
                bluePlayer.input = character.gameObject.GetComponent<PlayerInput>();
                bluePlayer.controller = character.gameObject.GetComponent<CharacterController>();
                bluePlayer.transform = character.gameObject.transform;
                blueRankingUI.gameObject.SetActive(true);
            }
            else if (character.PlayerID == CharacterControl.PlayerTypes.Yellow)
            {
                leaderboardPlayerPrefabs[3].SetActive(true);
                yellowPlayer.characterReference = character;
                yellowPlayer.name = character.HeadGFX.name;
                yellowPlayerPortrait.sprite = SetPlayerPortrait(yellowPlayer.name);
                yellowPlayerPortraitRanking.sprite = SetPlayerPortrait(yellowPlayer.name);
                yellowPlayer.portrait = yellowPlayerPortrait.sprite;
                yellowPlayerName.text = yellowPlayer.name;
                //yellowPlayer.rank = -1; //4
                yellowPlayer.eventSystem = character.gameObject.GetComponent<MultiplayerEventSystem>();
                yellowPlayer.input = character.gameObject.GetComponent<PlayerInput>();
                yellowPlayer.controller = character.gameObject.GetComponent<CharacterController>();
                yellowPlayer.transform = character.gameObject.transform;
                yellowRankingUI.gameObject.SetActive(true);
            }
            playerCount++;
        }
*/

}
