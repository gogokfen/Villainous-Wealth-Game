using MelenitasDev.SoundsGood;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

public class MenuPlayer : MonoBehaviour
{
    public int playerNum = 1;
    public int selectedChar;
    public GameObject pickedCharButton;
    public bool ready;
    private PlayerInput PI;
    private GameObject selectedButton;
    Sound announce;

    private void Start() 
    {
        PI = GetComponent<PlayerInput>();
    }

    public void PickCharacter(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            Pick(PI);
        }
    }

    public void CancelPickCharacter(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            Cancel(PI);
        }
    }

    private void Pick(PlayerInput player)
    {
        MultiplayerEventSystem playerEventSystem = player.GetComponent<MultiplayerEventSystem>();
        selectedButton = playerEventSystem.currentSelectedGameObject;
        if (selectedButton.GetComponent<CustomizeCharacter>().picked == false)
        {
            MenuPlayer menuPlayer = playerEventSystem.GetComponent<MenuPlayer>();
            menuPlayer.selectedChar = selectedButton.GetComponent<CustomizeCharacter>().characterNum;
            menuPlayer.pickedCharButton = selectedButton;
            playerEventSystem.GetComponent<MenuPlayer>().selectedChar = selectedButton.GetComponent<CustomizeCharacter>().characterNum;
            PlayerManager.instance.characterPicks[playerEventSystem.GetComponent<MenuPlayer>().selectedChar] = playerEventSystem.GetComponent<MenuPlayer>().playerNum;
            //selectedButton.GetComponent<Button>().interactable = false;
            selectedButton.GetComponent<CustomizeCharacter>().pickedUI.SetActive(true);
            playerEventSystem.SetSelectedGameObject(null);
            selectedButton.GetComponent<CustomizeCharacter>().picked = true;
            selectedButton.GetComponent<CustomizeCharacter>().CheckIfUnselect();
            if (ready == false)
            {
                ready = true;
                PlayerManager.instance.readyPlayers++;
            }
            SoundManager.singleton.PlayClip($"{selectedButton.name}CS", transform.position, 1f, false, false);
        }
    }

    private void Cancel(PlayerInput player)
    {
        //MenuPlayer menuPlayer = player.GetComponent<MenuPlayer>();
        if (selectedChar > -1)
        {
            GameObject selectedButton = pickedCharButton;
            PlayerManager.instance.characterPicks[selectedChar] = 0;
            selectedChar = 0;
            selectedButton.GetComponent<Button>().interactable = true;
            selectedButton.GetComponent<CustomizeCharacter>().pickedUI.SetActive(false);
            selectedButton.GetComponent<CustomizeCharacter>().picked = false;
            MultiplayerEventSystem playerEventSystem = player.GetComponent<MultiplayerEventSystem>();
            playerEventSystem.SetSelectedGameObject(null);
            playerEventSystem.SetSelectedGameObject(selectedButton);
            pickedCharButton = null;
            if (ready == true)
            {
                ready = false;
                PlayerManager.instance.readyPlayers--;
            }
        }
    }
}
