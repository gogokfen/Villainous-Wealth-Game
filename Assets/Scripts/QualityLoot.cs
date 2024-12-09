using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QualityLoot : MonoBehaviour
{
    //pickup layer is 6, character layer is 8

    [SerializeField] Slider pickupChannel;
    [SerializeField] Image  fillColor;

    private int playersStandingCount;

    CharacterControl.PlayerTypes playerInControl;
    CharacterControl.PlayerTypes newPlayerStanding;

    [SerializeField] string pickupNameToAquire;
    void Start()
    {
        pickupChannel.value = 0;
    }

    void Update()
    {
        if (playersStandingCount == 1)
        {
            pickupChannel.value += Time.deltaTime / 3;
            
        }
        else if (playersStandingCount == 0)
        {
            pickupChannel.value -= Time.deltaTime / 3;
        }

        if (playerInControl == CharacterControl.PlayerTypes.Red)
        {
            fillColor.color = Color.red;
        }
        else if (playerInControl == CharacterControl.PlayerTypes.Blue)
        {
            fillColor.color = Color.blue;
        }
        else if (playerInControl == CharacterControl.PlayerTypes.Green)
        {
            fillColor.color = Color.green;
        }
        else if (playerInControl == CharacterControl.PlayerTypes.Yellow)
        {
            fillColor.color = Color.yellow;
        }

        if (pickupChannel.value==1)
        {
            transform.name = pickupNameToAquire;
            gameObject.layer = 6;
            //fillColor.color = Color.white;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 8)
        {
            playersStandingCount++;
            if (playersStandingCount == 1)
            {
                if (playerInControl != other.GetComponent<CharacterControl>().PlayerID)
                {
                    pickupChannel.value = 0;
                }
                playerInControl = other.GetComponent<CharacterControl>().PlayerID;
            }
            else if (playersStandingCount>1)
            {
                newPlayerStanding = other.GetComponent<CharacterControl>().PlayerID;
            }
        }
    }

    /**
    private void OnTriggerStay(Collider other)
    {
        
        if (other.gameObject.layer == 8)
        {
            pickupChannel.value += Time.deltaTime / 5;

            // other. get player ID check for multiple player IDs if contested
        }
        
    }
*/
    private void OnTriggerExit(Collider other)
    {

        if (other.gameObject.layer == 8)
        {
            playersStandingCount--;
            if (other.GetComponent<CharacterControl>().PlayerID == playerInControl && playersStandingCount>0)
            {
                playerInControl = newPlayerStanding;
                pickupChannel.value = 0;
            }
        }

        /**
        if (other.gameObject.layer == 8 && !full)
        {
            pickupChannel.value = 0;
        }
        */
    }
}
