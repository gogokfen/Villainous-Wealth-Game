using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeepsOsherSaneManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Leaderboard.singleton.FindPlayers();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
