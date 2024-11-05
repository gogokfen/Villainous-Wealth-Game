using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.EventSystems;
using System.Collections;
using UnityEngine.UI;
using TMPro;

public class ShopManager : MonoBehaviour
{
    public GameObject[] defaultButtons; // Assign the default button for each player
    int defaultButtonIndex = 0;
    public GameObject shopUI;
    public float shopTimer;
    [SerializeField] TextMeshProUGUI timerText;

    private void Update()
    {

    }

    public void Shopping()
    {
        //Debug.Log("we shopping");
        shopUI.SetActive(true);
        PlayerInput[] playerInputs = FindObjectsOfType<PlayerInput>();
        defaultButtonIndex = 0;
        
        // for (int i = 0; i < defaultButtons.Length; i++) //osher for loop for resetting bought items
        // {
        //     defaultButtons[i].GetComponent<Button>().interactable = true;
        //     defaultButtons[i].GetComponent<ButtonSelectionTracker>().soldUI.SetActive(false);
        //     defaultButtons[i].GetComponent<ButtonSelectionTracker>().redSelectionIcon.SetActive(false);
        //     defaultButtons[i].GetComponent<ButtonSelectionTracker>().blueSelectionIcon.SetActive(false);
        //     defaultButtons[i].GetComponent<ButtonSelectionTracker>().greenSelectionIcon.SetActive(false);
        //     defaultButtons[i].GetComponent<ButtonSelectionTracker>().yellowSelectionIcon.SetActive(false);
        // }

        ResetButtons(); //resets Button UI and interactibility
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
                //Debug.Log("Player "+PlayerManager.instance.playerList[player.playerIndex].GetComponent<CharacterControl>().PlayerID + " Bought "+selectedButton.name+"!");

                int itemPrice = selectedButton.GetComponent<ButtonSelectionTracker>().itemPrice; //checking & buying items
                if (PlayerManager.instance.playerList[player.playerIndex].GetComponent<CharacterControl>().coins >= itemPrice)
                {
                    PlayerManager.instance.playerList[player.playerIndex].GetComponent<CharacterControl>().coins -= itemPrice;
                    PlayerManager.instance.playerList[player.playerIndex].GetComponent<CharacterControl>().BuyWeapon(selectedButton.name);

                    selectedButton.GetComponent<Button>().interactable = false;
                    selectedButton.GetComponent<ButtonSelectionTracker>().soldUI.SetActive(true);
                    playerEventSystem.SetSelectedGameObject(null);
                }



                //PlayerManager.instance.playerList[player.playerIndex].GetComponent<InputSystemUIInputModule>().enabled = false;



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
        float remainingTime = delay;
        while (remainingTime > 0)
        {
            timerText.text = $"Timer: {remainingTime.ToString("F1")}";
            remainingTime -= Time.deltaTime;
            yield return null;
        }
        shopUI.SetActive(false);
        PlayerInput[] playerInputs = FindObjectsOfType<PlayerInput>();
        foreach (PlayerInput player in playerInputs)
        {
            player.actions["UI/Submit"].performed -= ctx => OnSubmit(player);
            MultiplayerEventSystem playerEventSystem = player.GetComponentInChildren<MultiplayerEventSystem>();
            if (playerEventSystem != null)
            {
                playerEventSystem.SetSelectedGameObject(null);
            }
        }
    }

    private void ResetButtons()
{
    foreach (GameObject button in defaultButtons)
    {
        ButtonSelectionTracker tracker = button.GetComponent<ButtonSelectionTracker>();
        button.GetComponent<Button>().interactable = true;
        tracker.soldUI.SetActive(false);
        tracker.redSelectionIcon.SetActive(false);
        tracker.blueSelectionIcon.SetActive(false);
        tracker.greenSelectionIcon.SetActive(false);
        tracker.yellowSelectionIcon.SetActive(false);
    }
}

}