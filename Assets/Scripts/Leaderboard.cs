using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using VInspector;
using UnityEngine.UI;
using Unity.VisualScripting;

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
    [SerializeField] TextMeshProUGUI redPlayerKills;
    [SerializeField] Image redPlayerPortrait;
    [EndFoldout]
    [Foldout("Green Player")]
    [SerializeField] TextMeshProUGUI greenPlayerName;
    [SerializeField] TextMeshProUGUI greenPlayerCoins;
    [SerializeField] TextMeshProUGUI greenPlayerKills;
    [SerializeField] Image greenPlayerPortrait;
    [EndFoldout]
    [Foldout("Blue Player")]
    [SerializeField] TextMeshProUGUI bluePlayerName;
    [SerializeField] TextMeshProUGUI bluePlayerCoins;
    [SerializeField] TextMeshProUGUI bluePlayerKills;
    [SerializeField] Image bluePlayerPortrait;
    [EndFoldout]
    [Foldout("Yellow Player")]
    [SerializeField] TextMeshProUGUI yellowPlayerName;
    [SerializeField] TextMeshProUGUI yellowPlayerCoins;
    [SerializeField] TextMeshProUGUI yellowPlayerKills;
    [SerializeField] Image yellowPlayerPortrait;
    [EndFoldout]

    public CharacterControl[] characters;

    public int playerCount;
    private struct PlayerStats
    {
        public PlayerStats(CharacterControl characterReference, CharacterControl.PlayerTypes color ,string name, int roundStartMoney, int currentMoney, int kills, int deaths, bool wonThisRound)
        {
            this.characterReference = characterReference;
            this.color = color;
            this.name = name;
            this.roundStartMoney = roundStartMoney;
            this.currentMoney = currentMoney;
            this.kills = kills;
            this.deaths = deaths;
            this.wonThisRound = wonThisRound;
        }

        public CharacterControl characterReference;
        public CharacterControl.PlayerTypes color;
        public string name;
        public int roundStartMoney;
        public int currentMoney;
        public int kills;
        public int deaths;
        public bool wonThisRound;
    }
    PlayerStats redPlayer = new PlayerStats
                    (null,                            //character reference
                    CharacterControl.PlayerTypes.Red, //player color
                    "",                               //name
                    0,                                //round start money
                    0,                                //current money
                    0,                                //kills
                    0,                                //deaths
                    false);                           //won this round
    PlayerStats greenPlayer = new PlayerStats(null, CharacterControl.PlayerTypes.Green, "", 0, 0, 0, 0, false);
    PlayerStats bluePlayer = new PlayerStats(null, CharacterControl.PlayerTypes.Blue, "", 0, 0, 0, 0, false);
    PlayerStats yellowPlayer = new PlayerStats(null, CharacterControl.PlayerTypes.Yellow, "", 0, 0, 0, 0, false);

    private PlayerStats[] players;

    private struct PlayerRank
    {
        public int money;
    }

    private int[] moneyRankings;
    private string[] nameRankings;

    private void Start()
    {

    }

    public void FindPlayers()
    {
        characters = GameObject.FindObjectsOfType<CharacterControl>();

        foreach (CharacterControl character in characters)
        {
            if (character.PlayerID == CharacterControl.PlayerTypes.Red)
            {
                leaderboardPlayerPrefabs[0].SetActive(true);
                redPlayer.characterReference = character;
                redPlayer.name = character.HeadGFX.name;
                redPlayerPortrait.sprite = SetPlayerPortrait(redPlayer.name);
                redPlayerName.text = redPlayer.name;
            }

            else if (character.PlayerID == CharacterControl.PlayerTypes.Green)
            {
                leaderboardPlayerPrefabs[1].SetActive(true);
                greenPlayer.characterReference = character;
                greenPlayer.name = character.HeadGFX.name;
                greenPlayerPortrait.sprite = SetPlayerPortrait(greenPlayer.name);
                greenPlayerName.text = greenPlayer.name;
            }
            else if (character.PlayerID == CharacterControl.PlayerTypes.Blue)
            {
                leaderboardPlayerPrefabs[2].SetActive(true);
                bluePlayer.characterReference = character;
                bluePlayer.name = character.HeadGFX.name;
                bluePlayerPortrait.sprite = SetPlayerPortrait(bluePlayer.name);
                bluePlayerName.text = bluePlayer.name;
            }
            else if (character.PlayerID == CharacterControl.PlayerTypes.Yellow)
            {
                leaderboardPlayerPrefabs[3].SetActive(true);
                yellowPlayer.characterReference = character;
                yellowPlayer.name = character.HeadGFX.name;
                yellowPlayerPortrait.sprite = SetPlayerPortrait(yellowPlayer.name);
                yellowPlayerName.text = yellowPlayer.name;
            }
            playerCount++;
        }


        players = new PlayerStats[playerCount];
        moneyRankings = new int[playerCount];
        nameRankings = new string[playerCount];

        for (int i =0;i<playerCount;i++) //same order as the charactercontrol.playertypes Red Green Blue Yellow 0 1 2 3
        {
            if (i==0)
                players[i] = redPlayer;
            if (i == 1)
                players[i] = greenPlayer;
            if (i == 2)
                players[i] = bluePlayer;
            if (i == 3)
                players[i] = yellowPlayer;

            //Debug.Log(players[i].name);
        }

    }

    private void Awake()
    {
        singleton = this;
    }

    private void Update()
    {
        if ((Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.JoystickButton0)) && leaderboard.activeSelf)
        {
            //leaderboard.SetActive(false);
        }
    }

    public void AnnounceKill(CharacterControl.PlayerTypes killingPlayer, CharacterControl.PlayerTypes dyingPlayer)
    {
        if (RoundManager.instance.areWeWarming == true) 
            return;

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
    private Sprite SetPlayerPortrait(string name)
    {
        switch (name)
        {
            case "Dragon":
                return portraits[0];

            case "Monopoly Dude":
                return portraits[1];

            case "Dummy":
                return portraits[2];

            case "Boxhead":
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
        for (int i=0;i<players.Length;i++)
        {
            //characters[i].GetComponent<CharacterControl>().enabled = false;
            players[i].characterReference.DisableWeaponScripts();
        }
    }
    public void EnableCharacterControl()
    {
        for (int i = 0; i < characters.Length; i++)
        {
            //characters[i].GetComponent<CharacterControl>().enabled = true;
            players[i].characterReference.EnableWeaponScripts();
            
        }
    }

    public void DisplayPlayerSacks()
    {
        for (int i = 0; i < players.Length; i++)
        {
            players[i].characterReference.DisplaySack();
        }
    }


    ///////////////////////// MOVING ALL OF MONEY MANAGER INTO LEADERBOARD
    
    public void ModifyMoney(CharacterControl.PlayerTypes playerColor, int amount)
    {
        players[(int)playerColor].currentMoney += amount;

        if (players[(int)playerColor].currentMoney < 0)
            players[(int)playerColor].currentMoney = 0;

        FindLeader();
        ArrangeMoneyRanking();
    }

    public int GetMoney(CharacterControl.PlayerTypes playerColor)
    {
        return players[(int)playerColor].currentMoney;
    }

    public void FindLeader()
    {
        CharacterControl.PlayerTypes leader = CharacterControl.PlayerTypes.Red;

        int leaderIndex = 0;
        for (int i =1;i<players.Length;i++)
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

    public int FindLeaderMoney()
    {
        return players[(int)FindLeaderColor()].currentMoney; //probably the most bizzare code line I wrote in a while
    }

    public void UpdateRoundMoney(CharacterControl.PlayerTypes playerColor, int roundMoney)
    {
        for (int i =0;i<players.Length;i++)
        {
            if (players[i].color == playerColor)
                players[i].roundStartMoney = roundMoney;
        }
    }

    public void ArrangeMoneyRanking()
    {
        //oh yeah, madmah time!

        int[] tempArray = new int[moneyRankings.Length];
        string[] tempNames = new string[moneyRankings.Length];

        int indexToRemember = 0;

        for (int i = 0;i<moneyRankings.Length;i++)
        {
            tempArray[i] = players[i].currentMoney;
            tempNames[i] = players[i].name;
        }

        for (int i = 0;i<moneyRankings.Length;i++)
        {
            for (int j =0;j<moneyRankings.Length;j++)
            {
                if (tempArray[j]>moneyRankings[i])
                {
                    moneyRankings[i] = tempArray[j];
                    nameRankings[i] = tempNames[j];
                    indexToRemember = j;
                }
                tempArray[indexToRemember] = 0;
            }
        }

        //Array.Sort(moneyRankings);
    }

    public int DeathMoney(CharacterControl.PlayerTypes deadPlayerColor)
    {
        int moneylost = 0;

        if (players.Length == 4)
        {
            if (players[(int)deadPlayerColor].name == nameRankings[0])
                moneylost = players[(int)deadPlayerColor].currentMoney / 2; //50%
            else if (players[(int)deadPlayerColor].name == nameRankings[1])
                moneylost = players[(int)deadPlayerColor].currentMoney / 4; //25%
            else if (players[(int)deadPlayerColor].name == nameRankings[2])
                moneylost = players[(int)deadPlayerColor].currentMoney / 10; //10%
            else
                moneylost = 0;

            ModifyMoney(deadPlayerColor, -moneylost);

        }
        if (players.Length == 3)
        {
            if (players[(int)deadPlayerColor].name == nameRankings[0])
                moneylost = players[(int)deadPlayerColor].currentMoney / 2; //50%
            else if (players[(int)deadPlayerColor].name == nameRankings[1])
                moneylost = players[(int)deadPlayerColor].currentMoney / 4; //25%
            else
                moneylost = players[(int)deadPlayerColor].currentMoney / 10; //10%

            ModifyMoney(deadPlayerColor, -moneylost);

        }
        if (players.Length == 2)
        {
            if (players[(int)deadPlayerColor].name == nameRankings[0])
                moneylost = (int)(players[(int)deadPlayerColor].currentMoney * 0.4f); //40%
            else
                moneylost = (int)(players[(int)deadPlayerColor].currentMoney * 0.2f); //20%

            ModifyMoney(deadPlayerColor, -moneylost);

        }

        return moneylost;
    }

}
