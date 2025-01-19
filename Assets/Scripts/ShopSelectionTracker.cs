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

    [SerializeField] Sprite[] portraits;
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
        if (eventData.currentInputModule.GetComponent<CharacterControl>().PlayerID == CharacterControl.PlayerTypes.Red)
        {
            redPortrait.sprite = ChangePortraitInPin(eventData.currentInputModule.GetComponent<CharacterControl>().HeadGFX.name);
            redSelectionIcon.SetActive(true);
        }
        else if (eventData.currentInputModule.GetComponent<CharacterControl>().PlayerID == CharacterControl.PlayerTypes.Green)
        {
            greenPortrait.sprite = ChangePortraitInPin(eventData.currentInputModule.GetComponent<CharacterControl>().HeadGFX.name);
            greenSelectionIcon.SetActive(true);
        }
        else if (eventData.currentInputModule.GetComponent<CharacterControl>().PlayerID == CharacterControl.PlayerTypes.Blue)
        {
            bluePortrait.sprite = ChangePortraitInPin(eventData.currentInputModule.GetComponent<CharacterControl>().HeadGFX.name);
            blueSelectionIcon.SetActive(true);
        }
        else if (eventData.currentInputModule.GetComponent<CharacterControl>().PlayerID == CharacterControl.PlayerTypes.Yellow)
        {
            yellowPortrait.sprite = ChangePortraitInPin(eventData.currentInputModule.GetComponent<CharacterControl>().HeadGFX.name);
            yellowSelectionIcon.SetActive(true);
        }

    }

    public void OnDeselect(BaseEventData eventData)
    {
        if (eventData.currentInputModule.GetComponent<CharacterControl>().PlayerID == CharacterControl.PlayerTypes.Red)
            redSelectionIcon.SetActive(false);
        else if (eventData.currentInputModule.GetComponent<CharacterControl>().PlayerID == CharacterControl.PlayerTypes.Green)
            greenSelectionIcon.SetActive(false);
        else if (eventData.currentInputModule.GetComponent<CharacterControl>().PlayerID == CharacterControl.PlayerTypes.Blue)
            blueSelectionIcon.SetActive(false);
        else if (eventData.currentInputModule.GetComponent<CharacterControl>().PlayerID == CharacterControl.PlayerTypes.Yellow)
            yellowSelectionIcon.SetActive(false);
    }

    Sprite ChangePortraitInPin(string headName)
    {
        switch (headName)
        {
            case "Dragon":
                return portraits[0];

            case "Monopoly Dude":
                return portraits[1];

            case "Dummy":
                return portraits[2];

            case "Boxhead":
                return portraits[3];

            case "Orc":
                return portraits[4];

            case "Cat":
                return portraits[5];

            case "Leprechaun":
                return portraits[6];

            case "Mafia":
                return portraits[7];

            case "Pirate":
                return portraits[8];

            case "Shark":
                return portraits[9];
            default: return null;
        }
    }
}