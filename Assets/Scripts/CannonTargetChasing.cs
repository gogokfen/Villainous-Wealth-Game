using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonTargetChasing : MonoBehaviour
{
    GameObject playerToFollow;

    private float switchup;
    void Start()
    {
        FollowPlayer(Leaderboard.singleton.characters[0].gameObject);
        //FollowPlayer(CharacterControl.PlayerTypes.Green);
    }

    void Update()
    {
        //transform.position = playerToFollow.transform.position; //V0

        transform.position = Vector3.Lerp(transform.position,playerToFollow.transform.position,0.025f); //V1

        //transform.position = new Vector3(playerToFollow.transform.position.x+Random.Range(-1,1), playerToFollow.transform.position.y, playerToFollow.transform.position.z + Random.Range(-1, 1)); //V2

        switchup += Time.deltaTime;
        if ((int)switchup%2 ==0)
        {
            FollowPlayer(MoneyManager.singleton.FindLeaderColor());
            //FollowPlayer(Leaderboard.singleton.characters[(int)switchup % 3].gameObject);
        }

    }

    public void FollowPlayer(GameObject bombedPlayer)
    {
        playerToFollow = bombedPlayer;
    }

    public void FollowPlayer(CharacterControl.PlayerTypes bombedPlayerColor)
    {
        for (int i=0;i<Leaderboard.singleton.characters.Length;i++)
        {
            if (Leaderboard.singleton.characters[i].PlayerID == bombedPlayerColor)
                playerToFollow = Leaderboard.singleton.characters[i].gameObject;
        }
    }
}
