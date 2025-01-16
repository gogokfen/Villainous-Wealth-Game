using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using VInspector;
using UnityEngine.UI;

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
        public PlayerStats(CharacterControl characterReference, string name, int roundStartMoney, int currentMoney, int kills, int deaths, bool wonThisRound)
        {
            this.characterReference = characterReference;
            this.name = name;
            this.roundStartMoney = roundStartMoney;
            this.currentMoney = currentMoney;
            this.kills = kills;
            this.deaths = deaths;
            this.wonThisRound = wonThisRound;
        }

        public CharacterControl characterReference;
        public string name;
        public int roundStartMoney;
        public int currentMoney;
        public int kills;
        public int deaths;
        public bool wonThisRound;
    }
    PlayerStats redPlayer = new PlayerStats
                    (null,             //character reference
                    "",                //name
                    0,                 //round start money
                    0,                 //current money
                    0,                 //kills
                    0,                 //deaths
                    false);            //won this round
    PlayerStats greenPlayer = new PlayerStats(null, "", 0, 0, 0, 0, false);
    PlayerStats bluePlayer = new PlayerStats(null, "", 0, 0, 0, 0, false);
    PlayerStats yellowPlayer = new PlayerStats(null, "", 0, 0, 0, 0, false);

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
        if (killingPlayer == CharacterControl.PlayerTypes.Red)
        {
            killsAnnouncer.text = $"<sprite name=\"{redPlayer.name}\">";
            redPlayer.kills++;
        }
        else if (killingPlayer == CharacterControl.PlayerTypes.Green)
        {
            killsAnnouncer.text = $"<sprite name=\"{greenPlayer.name}\">";
            greenPlayer.kills++;
        }
        else if (killingPlayer == CharacterControl.PlayerTypes.Blue)
        {
            killsAnnouncer.text = $"<sprite name=\"{bluePlayer.name}\">";
            bluePlayer.kills++;
        }
        else if (killingPlayer == CharacterControl.PlayerTypes.Yellow)
        {
            killsAnnouncer.text = $"<sprite name=\"{yellowPlayer.name}\">";
            yellowPlayer.kills++;
        }

        if (dyingPlayer == CharacterControl.PlayerTypes.Red)
        {
            killsAnnouncer.text += " has killed " + $"<sprite name=\"{redPlayer.name}\">";
            redPlayer.deaths++;
        }
        else if (dyingPlayer == CharacterControl.PlayerTypes.Green)
        {
            killsAnnouncer.text += " has killed " + $"<sprite name=\"{greenPlayer.name}\">";
            greenPlayer.deaths++;
        }
        else if (dyingPlayer == CharacterControl.PlayerTypes.Blue)
        {
            killsAnnouncer.text += " has killed " + $"<sprite name=\"{bluePlayer.name}\">";
            bluePlayer.deaths++;
        }
        else if (dyingPlayer == CharacterControl.PlayerTypes.Yellow)
        {
            killsAnnouncer.text += " has killed " + $"<sprite name=\"{yellowPlayer.name}\">";
            yellowPlayer.deaths++;
        }

        killsAnnouncerAnimation.Play("Announcement");

        /*
        killsAnnouncer.text = killingPlayer + " Player has killed " + dyingPlayer + " Player!";
        killsAnnouncerAnimation.Play("Announcement");
        */
    }

    public void AnnounceKill(CharacterControl.PlayerTypes playerWhoDiedToZone) //in case of player killed by the zone
    {
        if (playerWhoDiedToZone == CharacterControl.PlayerTypes.Red)
        {
            killsAnnouncer.text = "The Zone has killed " + $"<sprite name=\"{redPlayer.name}\">";
            redPlayer.deaths++;
        }
        else if (playerWhoDiedToZone == CharacterControl.PlayerTypes.Green)
        {
            killsAnnouncer.text = "The Zone has killed " + $"<sprite name=\"{greenPlayer.name}\">";
            greenPlayer.deaths++;
        }
        else if (playerWhoDiedToZone == CharacterControl.PlayerTypes.Blue)
        {
            killsAnnouncer.text = "The Zone has killed " + $"<sprite name=\"{bluePlayer.name}\">";
            bluePlayer.deaths++;
        }
        else if (playerWhoDiedToZone == CharacterControl.PlayerTypes.Yellow)
        {
            killsAnnouncer.text = "The Zone has killed " + $"<sprite name=\"{yellowPlayer.name}\">";
            yellowPlayer.deaths++;
        }

        killsAnnouncerAnimation.Play("Announcement");

        /*
        killsAnnouncer.text = killingPlayer + " Player has killed " + dyingPlayer + " Player!";
        killsAnnouncerAnimation.Play("Announcement");
        */
    }

    // public void AnnounceKill(string killingPlayer, string dyingPlayer)
    // {
    //     killsAnnouncer.text = $"<sprite name=\"{killingPlayer}\"> Player has killed <sprite name=\"{dyingPlayer}\"> Player!";

    //     killsAnnouncerAnimation.Play("Announcement");

    //     //character.HeadGFX.name;
    // }

    public void UpdateLeaderboard()
    {
        //leaderboardActive = !leaderboardActive;
        leaderboard.SetActive(true);

        if (leaderboard.activeSelf)
        {
            if (leaderboardPlayerPrefabs[0].activeSelf)
            {
                redPlayer.roundStartMoney = MoneyManager.singleton.GetRoundMoney(CharacterControl.PlayerTypes.Red);
                redPlayer.currentMoney = MoneyManager.singleton.GetMoney(CharacterControl.PlayerTypes.Red);
                redPlayerCoins.text = redPlayer.currentMoney.ToString();
                redPlayerKills.text = redPlayer.kills.ToString();
            }
            if (leaderboardPlayerPrefabs[1].activeSelf)
            {
                greenPlayer.roundStartMoney = MoneyManager.singleton.GetRoundMoney(CharacterControl.PlayerTypes.Green);
                greenPlayer.currentMoney = MoneyManager.singleton.GetMoney(CharacterControl.PlayerTypes.Green);
                greenPlayerCoins.text = greenPlayer.currentMoney.ToString();
                greenPlayerKills.text = greenPlayer.kills.ToString();
            }
            if (leaderboardPlayerPrefabs[2].activeSelf)
            {
                bluePlayer.roundStartMoney = MoneyManager.singleton.GetRoundMoney(CharacterControl.PlayerTypes.Blue);
                bluePlayer.currentMoney = MoneyManager.singleton.GetMoney(CharacterControl.PlayerTypes.Blue);
                bluePlayerCoins.text = bluePlayer.currentMoney.ToString();
                bluePlayerKills.text = bluePlayer.kills.ToString();
            }
            if (leaderboardPlayerPrefabs[3].activeSelf)
            {
                yellowPlayer.roundStartMoney = MoneyManager.singleton.GetRoundMoney(CharacterControl.PlayerTypes.Yellow);
                yellowPlayer.currentMoney = MoneyManager.singleton.GetMoney(CharacterControl.PlayerTypes.Yellow);
                yellowPlayerCoins.text = yellowPlayer.currentMoney.ToString();
                yellowPlayerKills.text = yellowPlayer.kills.ToString();
            }
        }
    }
    public void EmptyPlayerHands()
    {
        for (int i = 0; i < Leaderboard.singleton.characters.Length; i++)
        {
            Leaderboard.singleton.characters[i].EmptyHand();
        }
    }

    public GameObject GetPlayerReference(CharacterControl.PlayerTypes playerColor)
    {
        if (playerColor == CharacterControl.PlayerTypes.Red)
            return redPlayer.characterReference.gameObject;
        if (playerColor == CharacterControl.PlayerTypes.Green)
            return greenPlayer.characterReference.gameObject;
        if (playerColor == CharacterControl.PlayerTypes.Blue)
            return bluePlayer.characterReference.gameObject;
        if (playerColor == CharacterControl.PlayerTypes.Yellow)
            return yellowPlayer.characterReference.gameObject;

        return null;
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

            case "Big Daddy":
                return portraits[4];

            case "Guardtron":
                return portraits[5];

            case "RI":
                return portraits[6];

            case "The Sheeper":
                return portraits[7];

            case "Jacko":
                return portraits[8];

            case "PC Pirate":
                return portraits[9];

            case "Kerenboy":
                return portraits[10];

            case "Nuke Man":
                return portraits[11];

            case "Stronghold Smasher":
                return portraits[12];

            case "Zolda":
                return portraits[13];

            case "Donte":
                return portraits[14];

            case "Booba":
                return portraits[15];

            case "Shamayim":
                return portraits[16];
            default: return null;
        }
    }
    public void AnnounceText(string text)
    {
        Leaderboard.singleton.killsAnnouncer.text = text;
        Leaderboard.singleton.killsAnnouncerAnimation.Play("Announcement");
    }

    public void StopForwardMomentum(CharacterControl.PlayerTypes playerColor)
    {
        if (playerColor == CharacterControl.PlayerTypes.Red)
            redPlayer.characterReference.StopForwardMomentum();
        if (playerColor == CharacterControl.PlayerTypes.Green)
            greenPlayer.characterReference.StopForwardMomentum();
        if (playerColor == CharacterControl.PlayerTypes.Blue)
            bluePlayer.characterReference.StopForwardMomentum();
        if (playerColor == CharacterControl.PlayerTypes.Yellow)
            yellowPlayer.characterReference.StopForwardMomentum();
    }

    public void DisableCharacterControl()
    {
        for (int i=0;i<characters.Length;i++)
        {
            characters[i].GetComponent<CharacterControl>().enabled = false;
        }
    }
    public void EnableCharacterControl()
    {
        for (int i = 0; i < characters.Length; i++)
        {
            characters[i].GetComponent<CharacterControl>().enabled = true;
        }
    }

    public void DisplayPlayerSacks()
    {
        for (int i = 0; i < characters.Length; i++)
        {
            characters[i].GetComponent<CharacterControl>().DisplaySack();
        }
    }
}
