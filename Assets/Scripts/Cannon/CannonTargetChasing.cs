using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonTargetChasing : MonoBehaviour
{
    Transform playerToFollow;

    private float switchup;
    void Start()
    {
        //FollowPlayer(Leaderboard.singleton.characters[0].gameObject);
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
            FollowPlayer(Leaderboard.singleton.FindLeaderColor());
            //FollowPlayer(Leaderboard.singleton.characters[(int)switchup % 3].gameObject);
        }

    }

    public void FollowPlayer(Transform bombedPlayer)
    {
        playerToFollow = bombedPlayer;
    }

    public void FollowPlayer(CharacterControl.PlayerTypes bombedPlayerColor)
    {
        playerToFollow = Leaderboard.singleton.FindLeaderInput().transform;
        /*
        for (int i=0;i<Leaderboard.singleton.playerCount;i++)
        {
            if (Leaderboard.singleton.players[i].PlayerID == bombedPlayerColor)
                playerToFollow = Leaderboard.singleton.players[i].gameObject;
        }
        */
    }
}
