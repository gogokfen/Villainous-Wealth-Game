using UnityEngine;
using UnityEngine.EventSystems;

using System.Collections;
using UnityEngine.UI;

using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

using TMPro;
using VInspector;
using System;

public class ButtonSelectionTracker : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    public int itemPrice;

    //[SerializeField] Sprite[] portraits;
    [SerializeField] public TextMeshProUGUI priceText;
    [SerializeField] public GameObject soldUI;
    public shopItemType type;

    [Foldout("Selection Icons")]
    [SerializeField] public GameObject redSelectionIcon;
    [SerializeField] public GameObject greenSelectionIcon;
    [SerializeField] public GameObject blueSelectionIcon;
    [SerializeField] public GameObject yellowSelectionIcon;
    [EndFoldout]

    [Foldout("Selection Portraits")]
    [SerializeField] Image redPortrait;
    [SerializeField] Image greenPortrait;
    [SerializeField] Image bluePortrait;
    [SerializeField] Image yellowPortrait;
    [EndFoldout]

    private CharacterControl.PlayerTypes currentPlayerID;

    public enum shopItemType
    {
        Weapon,
        Consumable
    }

    private void Start()
    {
        priceText.text = itemPrice.ToString();
    }

    public void OnSelect(BaseEventData eventData)
    {
        currentPlayerID = eventData.currentInputModule.GetComponent<CharacterControl>().PlayerID;

        SoundManager.singleton.PlayClip($"UI", transform.position, 1f, false, false); //return with sound

        if (currentPlayerID == CharacterControl.PlayerTypes.Red)
        {
            redPortrait.sprite = Leaderboard.singleton.SetPlayerPortrait(Leaderboard.singleton.GetPlayerName(currentPlayerID));
            redSelectionIcon.SetActive(true);
        }
        else if (currentPlayerID == CharacterControl.PlayerTypes.Green)
        {
            greenPortrait.sprite = Leaderboard.singleton.SetPlayerPortrait(Leaderboard.singleton.GetPlayerName(currentPlayerID));
            greenSelectionIcon.SetActive(true);
        }
        else if (currentPlayerID == CharacterControl.PlayerTypes.Blue)
        {
            bluePortrait.sprite = Leaderboard.singleton.SetPlayerPortrait(Leaderboard.singleton.GetPlayerName(currentPlayerID));
            blueSelectionIcon.SetActive(true);
        }
        else if (currentPlayerID == CharacterControl.PlayerTypes.Yellow)
        {
            yellowPortrait.sprite = Leaderboard.singleton.SetPlayerPortrait(Leaderboard.singleton.GetPlayerName(currentPlayerID));
            yellowSelectionIcon.SetActive(true);
        }

    }

    public void OnDeselect(BaseEventData eventData)
    {
        currentPlayerID = eventData.currentInputModule.GetComponent<CharacterControl>().PlayerID;

        if (currentPlayerID == CharacterControl.PlayerTypes.Red)
            redSelectionIcon.SetActive(false);
        else if (currentPlayerID == CharacterControl.PlayerTypes.Green)
            greenSelectionIcon.SetActive(false);
        else if (currentPlayerID == CharacterControl.PlayerTypes.Blue)
            blueSelectionIcon.SetActive(false);
        else if (currentPlayerID == CharacterControl.PlayerTypes.Yellow)
            yellowSelectionIcon.SetActive(false);
    }

    // Sprite ChangePortraitInPin(string headName)
    // {
    //     switch (headName)
    //     {
    //         case "Dragon":
    //             return portraits[0];

    //         case "Monopoly Dude":
    //             return portraits[1];

    //         case "Dummy":
    //             return portraits[2];

    //         case "Boxhead":
    //             return portraits[3];

    //         case "Orc":
    //             return portraits[4];

    //         case "Cat":
    //             return portraits[5];

    //         case "Leprechaun":
    //             return portraits[6];

    //         case "Mafia":
    //             return portraits[7];

    //         case "Pirate":
    //             return portraits[8];

    //         case "Shark":
    //             return portraits[9];
    //         default: return null;
    //     }
    // }
}