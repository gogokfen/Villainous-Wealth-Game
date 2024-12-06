using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyManager : MonoBehaviour
{
    public static MoneyManager singleton { get; private set; }

    private CharacterControl[] characters;

    int redMoney;
    int greenMoney;
    int blueMoney;
    int yellowMoney;


    void Start()
    {
        characters = GameObject.FindObjectsOfType<CharacterControl>();
    }

    void Update()
    {

    }

    private void Awake()
    {
        singleton = this;
    }

    public void ModifyMoney(CharacterControl.PlayerTypes playerColor, int amount)
    {
        if (playerColor == CharacterControl.PlayerTypes.Red)
        {
            redMoney += amount;
            if (redMoney < 0)
                redMoney = 0;
        }
        else if (playerColor == CharacterControl.PlayerTypes.Green)
        {
            greenMoney += amount;
            if (greenMoney < 0)
                greenMoney = 0;
        }
        else if (playerColor == CharacterControl.PlayerTypes.Blue)
        {
            blueMoney += amount;
            if (blueMoney < 0)
                blueMoney = 0;
        }
        else if (playerColor == CharacterControl.PlayerTypes.Yellow)
        {
            yellowMoney += amount;
            if (yellowMoney < 0)
                yellowMoney = 0;
        }

        FindLeader();
    }

    public int GetMoney(CharacterControl.PlayerTypes playerColor)
    {
        if (playerColor == CharacterControl.PlayerTypes.Red)
        {
            return redMoney;
        }
        else if (playerColor == CharacterControl.PlayerTypes.Green)
        {
            return greenMoney;
        }
        else if (playerColor == CharacterControl.PlayerTypes.Blue)
        {
            return blueMoney;
        }
        else if (playerColor == CharacterControl.PlayerTypes.Yellow)
        {
            return yellowMoney;
        }
        else
            return 0;
    }

    /*
    public CharacterControl.PlayerTypes GetLeader()
    {
        if (redMoney > blueMoney && redMoney > greenMoney && redMoney > yellowMoney)
            return CharacterControl.PlayerTypes.Red;
        else if (blueMoney > redMoney && blueMoney > greenMoney && blueMoney > yellowMoney)
            return CharacterControl.PlayerTypes.Blue;
        else if (greenMoney > redMoney && greenMoney > blueMoney && greenMoney > yellowMoney)
            return CharacterControl.PlayerTypes.Green;
        else if (yellowMoney > redMoney && yellowMoney > greenMoney && yellowMoney > blueMoney)
            return CharacterControl.PlayerTypes.Blue;
        else
            return CharacterControl.PlayerTypes.Yellow;
    }
    */

    public void FindLeader()
    {
        CharacterControl.PlayerTypes leader;

        if (redMoney > blueMoney && redMoney > greenMoney && redMoney > yellowMoney)
            leader = CharacterControl.PlayerTypes.Red;
        else if (blueMoney > redMoney && blueMoney > greenMoney && blueMoney > yellowMoney)
            leader = CharacterControl.PlayerTypes.Blue;
        else if (greenMoney > redMoney && greenMoney > blueMoney && greenMoney > yellowMoney)
            leader = CharacterControl.PlayerTypes.Green;
        else if (yellowMoney > redMoney && yellowMoney > greenMoney && yellowMoney > blueMoney)
            leader = CharacterControl.PlayerTypes.Blue;
        else
            leader = CharacterControl.PlayerTypes.Yellow;

        for (int i=0;i<characters.Length;i++)
        {
            characters[i].SetLeader(leader);
        }
    }
}
