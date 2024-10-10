using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MineShot : WeaponBase
{
    //[SerializeField] GameObject mineBaseColor;
    void Start()
    {
        Destroy(gameObject, 30);

        if (playerID == CharacterControl.PlayerTypes.Red)
        {
            GetComponent<Renderer>().material.color = Color.red;
        }
        else if (playerID == CharacterControl.PlayerTypes.Blue)
        {
            GetComponent<Renderer>().material.color = Color.blue;
        }
        else if (playerID == CharacterControl.PlayerTypes.Green)
        {
            GetComponent<Renderer>().material.color = Color.green;
        }
        else if (playerID == CharacterControl.PlayerTypes.Yellow)
        {
            GetComponent<Renderer>().material.color = Color.yellow;
        }

    }

    void Update()
    {

    }
}
