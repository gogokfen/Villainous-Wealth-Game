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
    public TextMeshProUGUI roundAnnouncer;
    public Animator roundAnnouncerAnimation;

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
    [SerializeField] Animator redRankingUIAnim;
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
    [SerializeField] Animator greenRankingUIAnim;
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
    [SerializeField] Animator blueRankingUIAnim;
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
    [SerializeField] Animator yellowRankingUIAnim;
    [EndFoldout]

    public RectTransform[] rankingTransforms;

    public int playerCount;

    private struct PlayerStats
    {
        public PlayerStats(CharacterControl characterReference, MultiplayerEventSystem eventSystem, PlayerInput input, CharacterController controller, Animator animator, Sprite portrait, Transform transform, CharacterControl.PlayerTypes color, string name, int roundStartMoney, int currentMoney, int kills, int deaths, bool wonThisRound, int rank)
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
            this.animator = animator;

        }

        public CharacterControl characterReference;
        public CharacterControl.PlayerTypes color;
        public MultiplayerEventSystem eventSystem;
        public PlayerInput input;
        public CharacterController controller;
        public Animator animator;
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

    private Ranking[] ranking;

    public void FindPlayers()
    {
        CharacterControl[] characters;
        characters = GameObject.FindObjectsOfType<CharacterControl>();

        players = new PlayerStats[characters.Length];
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
                players[0].animator = character.charAnim;
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
                players[1].animator = character.charAnim;
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
                players[2].animator = character.charAnim;
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
                players[3].animator = character.charAnim;
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

        if (killingPlayer != dyingPlayer) //to prevent self bombing getting money
        {
            ModifyMoney(killingPlayer, 5); //giving money to the killer

            players[(int)killingPlayer].characterReference.GetKillingMoney(5);
        }

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
        for (int i = 0; i < players.Length; i++)
        {
            players[i].currentMoney = 0;
            players[i].roundStartMoney = 0;
        }
    }

    public void UpdateLeaderboard()
    {
        leaderboard.SetActive(true);
        SoundManager.singleton.PlayClip("WinLeaderboard", transform.position, 1f, false, false);
        if (leaderboard.activeSelf)
        {
            if (leaderboardPlayerPrefabs[0].activeSelf)
            {
                redPlayerCoins.text = players[0].currentMoney.ToString();
                redPlayerKills.text = players[0].kills.ToString();
                if (players[0].rank == 1)
                {
                    redRankingUIAnim.Play("LeadingPlayerLB");
                }
            }
            if (leaderboardPlayerPrefabs[1].activeSelf)
            {
                greenPlayerCoins.text = players[1].currentMoney.ToString();
                greenPlayerKills.text = players[1].kills.ToString();
                if (players[1].rank == 1)
                {
                    greenRankingUIAnim.Play("LeadingPlayerLB");
                }
            }
            if (leaderboardPlayerPrefabs[2].activeSelf)
            {
                bluePlayerCoins.text = players[2].currentMoney.ToString();
                bluePlayerKills.text = players[2].kills.ToString();
                if (players[2].rank == 1)
                {
                    blueRankingUIAnim.Play("LeadingPlayerLB");
                }
            }
            if (leaderboardPlayerPrefabs[3].activeSelf)
            {
                yellowPlayerCoins.text = players[3].currentMoney.ToString();
                yellowPlayerKills.text = players[3].kills.ToString();
                if (players[3].rank == 1)
                {
                    yellowRankingUIAnim.Play("LeadingPlayerLB");
                }
            }
        }
    }

    public void ResetLeaderboard()
    {
        redRankingUIAnim.Play("DefaultLB");
        greenRankingUIAnim.Play("DefaultLB");
        if (playerCount == 3)
        {
            blueRankingUIAnim.Play("DefaultLB");
            if (playerCount == 4)
            {
                yellowRankingUIAnim.Play("DefaultLB");
            }
        }
        leaderboard.SetActive(false);
    }

    public void UpdateRanking()
    {
        if (playerCount > 0)
        {
            //Red Player
            redPlayerCoins.text = players[0].currentMoney.ToString();
            redPlayerCoinsRanking.text = players[0].currentMoney.ToString();
            redPlayerKills.text = players[0].kills.ToString();
            redPlayerRank.text = players[0].rank.ToString();

            redRankingUI.localPosition = rankingTransforms[(players[0].rank - 1)].localPosition;

            if (players[0].rank == 1)
            {
                redRankingUI.localScale = Vector3.one * 1.5f;
                redRankingUI.GetComponent<Image>().color = new Color(1, 0.75f, 0);
            }
            else
            {
                redRankingUI.localScale = Vector3.one;
                redRankingUI.GetComponent<Image>().color = new Color(0.65f, 0.65f, 0.65f);
            }
        }
        if (playerCount > 1)
        {
            //Green Player
            greenPlayerCoins.text = players[1].currentMoney.ToString();
            greenPlayerCoinsRanking.text = players[1].currentMoney.ToString();
            greenPlayerKills.text = players[1].kills.ToString();
            greenPlayerRank.text = players[1].rank.ToString();

            greenRankingUI.localPosition = rankingTransforms[(players[1].rank - 1)].localPosition;

            if (players[1].rank == 1)
            {
                greenRankingUI.localScale = Vector3.one * 1.5f;
                greenRankingUI.GetComponent<Image>().color = new Color(1, 0.75f, 0);
            }
            else
            {
                greenRankingUI.localScale = Vector3.one;
                greenRankingUI.GetComponent<Image>().color = new Color(0.65f, 0.65f, 0.65f);
            }
        }
        if (playerCount > 2)
        {
            //Blue Player
            bluePlayerCoins.text = players[2].currentMoney.ToString();
            bluePlayerCoinsRanking.text = players[2].currentMoney.ToString();
            bluePlayerKills.text = players[2].kills.ToString();
            bluePlayerRank.text = players[2].rank.ToString();

            blueRankingUI.localPosition = rankingTransforms[(players[2].rank - 1)].localPosition;

            if (players[2].rank == 1)
            {
                blueRankingUI.localScale = Vector3.one * 1.5f;
                blueRankingUI.GetComponent<Image>().color = new Color(1, 0.75f, 0);
            }
            else
            {
                blueRankingUI.localScale = Vector3.one;
                blueRankingUI.GetComponent<Image>().color = new Color(0.65f, 0.65f, 0.65f);
            }
        }
        if (playerCount > 3)
        {
            //Yellow Player
            yellowPlayerCoins.text = players[3].currentMoney.ToString();
            yellowPlayerCoinsRanking.text = players[3].currentMoney.ToString();
            yellowPlayerKills.text = players[3].kills.ToString();
            yellowPlayerRank.text = players[3].rank.ToString();

            yellowRankingUI.localPosition = rankingTransforms[(players[3].rank - 1)].localPosition;

            if (players[3].rank == 1)
            {
                yellowRankingUI.localScale = Vector3.one * 1.5f;
                yellowRankingUI.GetComponent<Image>().color = new Color(1, 0.75f, 0);
            }
            else
            {
                yellowRankingUI.localScale = Vector3.one;
                yellowRankingUI.GetComponent<Image>().color = new Color(0.65f, 0.65f, 0.65f);
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

    public CharacterControl[] GetPlayerReferences()
    {
        CharacterControl[] cc = new CharacterControl[playerCount];
        for (int i = 0; i < playerCount; i++)
        {
            cc[i] = players[i].characterReference;
        }
        return cc;
    }
    public Animator[] GetPlayerAnimators()
    {
        Animator[] anim = new Animator[playerCount];
        for (int i = 0; i < playerCount; i++)
        {
            anim[i] = players[i].animator;
        }
        return anim;
    }

    public void TurnOffPlayersGFX()
    {
        for (int i = 0; i < playerCount; i++)
        {
            players[i].characterReference.characterGFX.SetActive(false);
        }
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

    public void AnnounceTextRoundStart(string text)
    {
        roundAnnouncer.text = text;
        roundAnnouncerAnimation.Play("Round Start", 0, 0.0f);
    }

    public void StopForwardMomentum(CharacterControl.PlayerTypes playerColor)
    {
        players[(int)playerColor].characterReference.StopForwardMomentum();
    }

    public void DisableCharacterWeapons()
    {
        for (int i = 0; i < players.Length; i++)
        {
            players[i].characterReference.DisableWeaponScripts();
        }
    }

    public void EnableCharacterWeapons()
    {
        for (int i = 0; i < players.Length; i++)
        {
            players[i].characterReference.EnableWeaponScripts();
        }
    }

    public void EnableCharacterControl()
    {
        for (int i = 0; i < players.Length; i++)
        {
            players[i].characterReference.enabled = true;
        }
    }

    public void DisableCharacterControl()
    {
        for (int i = 0; i < players.Length; i++)
        {
            players[i].characterReference.enabled = false;
        }
    }

    public void DisableCharacterController()
    {
        for (int i = 0; i < players.Length; i++)
        {
            players[i].controller.enabled = false;
        }
    }
    public void EnableCharacterController()
    {
        for (int i = 0; i < players.Length; i++)
        {
            players[i].controller.enabled = true;
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
            }
        }
        return players[leaderIndex].name;
    }

    public string DetermineLosers(int index)
    {
        List<PlayerStats> losers = new List<PlayerStats>();
        foreach (var player in players)
        {
            if (player.rank >= 2)
            {
                losers.Add(player);
            }
        }
        return losers[index].name;
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

        //for security:
        for (int i = 0; i < ranking.Length; i++)
        {
            ranking[i].money = -10;
            ranking[i].color = CharacterControl.PlayerTypes.Yellow;
        }

        int indexToRemember = 0;

        for (int i = 0; i < ranking.Length; i++)
        {
            tempArray[i].money = players[i].currentMoney;
            tempArray[i].color = players[i].color;
        }

        for (int i = 0; i < ranking.Length; i++)
        {
            for (int j = 0; j < ranking.Length; j++)
            {
                if (tempArray[j].money > ranking[i].money)
                {
                    ranking[i].money = tempArray[j].money;
                    ranking[i].color = tempArray[j].color;
                    indexToRemember = j;
                }
            }
            tempArray[indexToRemember].money = -1; //move to lower for?
        }
        for (int i = 0; i < ranking.Length; i++)
        {
            players[(int)ranking[i].color].rank = (i + 1); // for instance if if the array ranking contains a yellow player on the first place -> [0] that means that that players[3] = 1 
        }
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
}
