using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Leaderboard : MonoBehaviour
{
    public static Leaderboard singleton { get; private set; }

    [SerializeField] TextMeshProUGUI killsAnnouncer;
    [SerializeField] Animator killsAnnouncerAnimation;

    [SerializeField] GameObject leaderboard;
    private bool leaderboardActive = false;

    [SerializeField] TextMeshProUGUI redPlayerText;
    [SerializeField] TextMeshProUGUI greenPlayerText;
    [SerializeField] TextMeshProUGUI bluePlayerText;
    [SerializeField] TextMeshProUGUI yellowPlayerText;

    private CharacterControl[] characters;
    /*
    private string redPlayerName;
    private string greenPlayerName;
    private string bluePlayerName;
    private string yellowPlayerName;
    */

    private struct PlayerStats
    {
        public PlayerStats(CharacterControl characterRefrence, string name, int roundStartMoney, int currentMoney, int kills, int deaths, bool wonThisRound)
        {
            this.characterRefrence = characterRefrence;
            this.name = name;
            this.roundStartMoney = roundStartMoney;
            this.currentMoney = currentMoney;
            this.kills = kills;
            this.deaths = deaths;
            this.wonThisRound = wonThisRound;
        }

        public CharacterControl characterRefrence;
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
        characters = GameObject.FindObjectsOfType<CharacterControl>();

        foreach (CharacterControl character in characters)
        {
            if (character.PlayerID == CharacterControl.PlayerTypes.Red)
            {
                redPlayer.characterRefrence = character;
                redPlayer.name = character.HeadGFX.name;
            }

            else if (character.PlayerID == CharacterControl.PlayerTypes.Green)
            {
                greenPlayer.characterRefrence = character;
                greenPlayer.name = character.HeadGFX.name;
            }
            else if (character.PlayerID == CharacterControl.PlayerTypes.Blue)
            {
                bluePlayer.characterRefrence = character;
                bluePlayer.name = character.HeadGFX.name;
            }
            else if (character.PlayerID == CharacterControl.PlayerTypes.Yellow)
            {
                yellowPlayer.characterRefrence = character;
                yellowPlayer.name = character.HeadGFX.name;
            }

        }
    }

    private void Awake()
    {
        singleton = this;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ShowLeaderboard();
        }
    }

    public void AnnounceKill(CharacterControl.PlayerTypes killingPlayer, CharacterControl.PlayerTypes dyingPlayer)
    {
        if (killingPlayer == CharacterControl.PlayerTypes.Red)
        {
            killsAnnouncer.text = redPlayer.name;
            redPlayer.kills++;
        }
        else if (killingPlayer == CharacterControl.PlayerTypes.Green)
        {
            killsAnnouncer.text = greenPlayer.name;
            greenPlayer.kills++;
        }
        else if (killingPlayer == CharacterControl.PlayerTypes.Blue)
        {
            killsAnnouncer.text = bluePlayer.name;
            bluePlayer.kills++;
        }
        else if (killingPlayer == CharacterControl.PlayerTypes.Yellow)
        {
            killsAnnouncer.text = yellowPlayer.name;
            yellowPlayer.kills++;
        }

        if (dyingPlayer == CharacterControl.PlayerTypes.Red)
        {
            killsAnnouncer.text += " has killed " + redPlayer.name;
            redPlayer.deaths++;
        }
        else if (dyingPlayer == CharacterControl.PlayerTypes.Green)
        {
            killsAnnouncer.text += " has killed " + greenPlayer.name;
            greenPlayer.deaths++;
        }
        else if (dyingPlayer == CharacterControl.PlayerTypes.Blue)
        {
            killsAnnouncer.text += " has killed " + bluePlayer.name;
            bluePlayer.deaths++;
        }
        else if (dyingPlayer == CharacterControl.PlayerTypes.Yellow)
        {
            killsAnnouncer.text += " has killed " + yellowPlayer.name;
            yellowPlayer.deaths++;
        }

        killsAnnouncerAnimation.Play("Announcement");

        /*
        killsAnnouncer.text = killingPlayer + " Player has killed " + dyingPlayer + " Player!";
        killsAnnouncerAnimation.Play("Announcement");
        */
    }

    public void AnnounceKill(string killingPlayer, string dyingPlayer)
    {
        killsAnnouncer.text = killingPlayer + " Player has killed " + dyingPlayer + " Player!";
        killsAnnouncerAnimation.Play("Announcement");

        //character.HeadGFX.name;
    }

    public void ShowLeaderboard()
    {
        leaderboardActive = !leaderboardActive;
        leaderboard.SetActive(leaderboardActive);

        if (leaderboardActive)
        {
            if (redPlayer.characterRefrence == null)
                redPlayerText.gameObject.SetActive(false);
            else
            {
                redPlayer.roundStartMoney = MoneyManager.singleton.GetRoundMoney(CharacterControl.PlayerTypes.Red);
                redPlayer.currentMoney = MoneyManager.singleton.GetMoney(CharacterControl.PlayerTypes.Red);
                //                                  color name   round$cur$killsdeaths
                redPlayerText.text = string.Format("{0,-6}{1,-25}{2,8:N0}{3,8:N0}{4,6:N0}{5,6:N0}",
                    CharacterControl.PlayerTypes.Red, redPlayer.name, redPlayer.roundStartMoney, (redPlayer.currentMoney-redPlayer.roundStartMoney), redPlayer.kills, redPlayer.deaths);

                //Debug.Log(redPlayerText.text);
                /*
                string output;
                output = string.Format("{0,-12}{1,8:yyyy}{2,12:N0}{3,8:yyyy}{4,12:N0}{5,14:P1}",
                       "hello", 1958, 1234567, 1970, 7654321,
                       (7654321 - 1234567) / (double)1234567);
                Debug.Log(output);
                */
            }

            if (greenPlayer.characterRefrence == null)
                greenPlayerText.gameObject.SetActive(false);
            else
            {
                greenPlayer.roundStartMoney = MoneyManager.singleton.GetRoundMoney(CharacterControl.PlayerTypes.Green);
                greenPlayer.currentMoney = MoneyManager.singleton.GetMoney(CharacterControl.PlayerTypes.Green);
                greenPlayerText.text = string.Format("{0,-6}{1,-25}{2,8:N0}{3,8:N0}{4,6:N0}{5,6:N0}",
                    CharacterControl.PlayerTypes.Green, greenPlayer.name, greenPlayer.roundStartMoney, (greenPlayer.currentMoney - greenPlayer.roundStartMoney), greenPlayer.kills, greenPlayer.deaths);

                //Debug.Log(greenPlayerText.text);
            }

            if (bluePlayer.characterRefrence == null)
                bluePlayerText.gameObject.SetActive(false);
            else
            {
                bluePlayer.roundStartMoney = MoneyManager.singleton.GetRoundMoney(CharacterControl.PlayerTypes.Blue);
                bluePlayer.currentMoney = MoneyManager.singleton.GetMoney(CharacterControl.PlayerTypes.Blue);
                bluePlayerText.text = string.Format("{0,-6}{1,-25}{2,8:N0}{3,8:N0}{4,6:N0}{5,6:N0}",
                    CharacterControl.PlayerTypes.Blue, bluePlayer.name, bluePlayer.roundStartMoney, (bluePlayer.currentMoney - bluePlayer.roundStartMoney), bluePlayer.kills, bluePlayer.deaths);

                //Debug.Log(greenPlayerText.text);
            }

            if (yellowPlayer.characterRefrence == null)
                yellowPlayerText.gameObject.SetActive(false);
            else
            {
                yellowPlayer.roundStartMoney = MoneyManager.singleton.GetRoundMoney(CharacterControl.PlayerTypes.Yellow);
                yellowPlayer.currentMoney = MoneyManager.singleton.GetMoney(CharacterControl.PlayerTypes.Yellow);
                yellowPlayerText.text = string.Format("{0,-6}{1,-25}{2,8:N0}{3,8:N0}{4,6:N0}{5,6:N0}",
                    CharacterControl.PlayerTypes.Yellow, yellowPlayer.name, yellowPlayer.roundStartMoney, (yellowPlayer.currentMoney - yellowPlayer.roundStartMoney), yellowPlayer.kills, yellowPlayer.deaths);

                //Debug.Log(greenPlayerText.text);
            }
        }
    }
}
