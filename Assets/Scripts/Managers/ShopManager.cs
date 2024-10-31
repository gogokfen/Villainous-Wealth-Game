using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class ShopManager : MonoBehaviour
{
    public GameObject[] defaultButtons; // Assign the default button for each player
    int defaultButtonIndex = 0;
    public GameObject shopUI;
    public float shopTimer;

    public void Shopping()
    {
        Debug.Log("we shopping");
        shopUI.SetActive(true);
        PlayerInput[] playerInputs = FindObjectsOfType<PlayerInput>();
        defaultButtonIndex = 0;

        foreach (PlayerInput player in playerInputs)
        {
            // Find the EventSystem attached to this player
            MultiplayerEventSystem playerEventSystem = player.GetComponentInChildren<MultiplayerEventSystem>();
            if (playerEventSystem != null)
            {
                // Set the initial selected button for the player
                playerEventSystem.SetSelectedGameObject(null);
                playerEventSystem.SetSelectedGameObject(defaultButtons[defaultButtonIndex]);

                // Subscribe to the Submit action for each player
                player.actions["UI/Submit"].performed += ctx => OnSubmit(player);

                defaultButtonIndex++;
            }
        }

        StartCoroutine(TimerCloseShop(shopTimer));
    }

    private void OnSubmit(PlayerInput player)
    {
        // Get the EventSystem related to this player
        MultiplayerEventSystem playerEventSystem = player.GetComponentInChildren<MultiplayerEventSystem>();
        if (playerEventSystem != null)
        {
            // Get the currently selected button
            GameObject selectedButton = playerEventSystem.currentSelectedGameObject;

            if (selectedButton != null)
            {
                Debug.Log(PlayerManager.instance.playerList[player.playerIndex].GetComponent<CharacterControl>().PlayerID);
                //Debug.Log($"Player {player.playerIndex} pressed the submit button on {selectedButton.name}");
                // Handle the player's selection logic here, e.g., purchasing an item
            }
            else
            {
                Debug.Log($"Player {player.playerIndex} pressed submit, but no button is currently selected.");
            }
        }
    }

    private IEnumerator TimerCloseShop(float delay)
    {
        yield return new WaitForSeconds(delay);
        shopUI.SetActive(false);
        Debug.Log("Shop UI closed after 5 seconds.");

        // Unsubscribe from the Submit action when the shop closes
        PlayerInput[] playerInputs = FindObjectsOfType<PlayerInput>();
        foreach (PlayerInput player in playerInputs)
        {
            player.actions["UI/Submit"].performed -= ctx => OnSubmit(player);
        }
    }
}