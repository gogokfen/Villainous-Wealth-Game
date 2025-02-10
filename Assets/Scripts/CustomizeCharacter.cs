using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using VInspector;
public class CustomizeCharacter : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    public int characterNum;
    public bool picked;
    public GameObject pickedUI;

    [Foldout("Selection Icons")]
    [SerializeField] public GameObject redSelectionIcon;
    [SerializeField] public GameObject greenSelectionIcon;
    [SerializeField] public GameObject blueSelectionIcon;
    [SerializeField] public GameObject yellowSelectionIcon;
    [EndFoldout]
    private Button button;
    private void Start()
    {
        button = GetComponent<Button>();
    }

    public void CheckIfUnselect()
    {
        if (picked && (!redSelectionIcon.activeSelf && !greenSelectionIcon.activeSelf && !blueSelectionIcon.activeSelf && !yellowSelectionIcon.activeSelf))
            button.interactable = false;
    }

    public void OnSelect(BaseEventData eventData)
    {
        var menuPlayer = eventData.currentInputModule?.GetComponent<MenuPlayer>();
        if (menuPlayer == null) return;
        switch (menuPlayer.playerNum)
        {
            case 1:
                redSelectionIcon.SetActive(true);
                MenuManager.instance.playerShowcases[0].sprite = ChangeShowcase(gameObject.name);
                MenuManager.instance.selectedCharacterName[0].text = NameSelectedButton(gameObject.name);
                pickedUI.GetComponent<Image>().color = new Color32(204, 67, 152, 255);
                break;
            case 2:
                greenSelectionIcon.SetActive(true);
                MenuManager.instance.playerShowcases[1].sprite = ChangeShowcase(gameObject.name);
                MenuManager.instance.selectedCharacterName[1].text = NameSelectedButton(gameObject.name);
                pickedUI.GetComponent<Image>().color = new Color32(96, 196, 71, 255);
                break;
            case 3:
                blueSelectionIcon.SetActive(true);
                MenuManager.instance.playerShowcases[2].sprite = ChangeShowcase(gameObject.name);
                MenuManager.instance.selectedCharacterName[2].text = NameSelectedButton(gameObject.name);
                pickedUI.GetComponent<Image>().color = new Color32(54, 111, 218, 255);
                break;
            case 4:
                yellowSelectionIcon.SetActive(true);
                MenuManager.instance.playerShowcases[3].sprite = ChangeShowcase(gameObject.name);
                MenuManager.instance.selectedCharacterName[3].text = NameSelectedButton(gameObject.name);
                pickedUI.GetComponent<Image>().color = new Color32(51, 189, 190, 255);
                break;
        }
    }
    public void OnDeselect(BaseEventData eventData)
    {
        if (eventData.currentInputModule.GetComponent<MenuPlayer>().playerNum == 1)
            redSelectionIcon.SetActive(false);
        else if (eventData.currentInputModule.GetComponent<MenuPlayer>().playerNum == 2)
            greenSelectionIcon.SetActive(false);
        else if (eventData.currentInputModule.GetComponent<MenuPlayer>().playerNum == 3)
            blueSelectionIcon.SetActive(false);
        else if (eventData.currentInputModule.GetComponent<MenuPlayer>().playerNum == 4)
            yellowSelectionIcon.SetActive(false);

        if (picked && !(redSelectionIcon.activeSelf && greenSelectionIcon.activeSelf && blueSelectionIcon.activeSelf && yellowSelectionIcon.activeSelf))
            button.interactable = false;
    }
    private void OnDisable()
    {
        redSelectionIcon.SetActive(false);
        greenSelectionIcon.SetActive(false);
        blueSelectionIcon.SetActive(false);
        yellowSelectionIcon.SetActive(false);
        pickedUI.SetActive(false);
        button.interactable = true;
    }
    public void FirstTimeSelection(int num)
    {
        if (num == 1)
        {
            redSelectionIcon.SetActive(true);
            MenuManager.instance.playerShowcases[0].sprite = ChangeShowcase(gameObject.name);
            MenuManager.instance.selectedCharacterName[0].text = NameSelectedButton(gameObject.name);
        }
        else if (num == 2)
        {
            greenSelectionIcon.SetActive(true);
            MenuManager.instance.playerShowcases[1].sprite = ChangeShowcase(gameObject.name);
            MenuManager.instance.selectedCharacterName[1].text = NameSelectedButton(gameObject.name);
        }
        else if (num == 3)
        {
            MenuManager.instance.playerShowcases[2].sprite = ChangeShowcase(gameObject.name);
            MenuManager.instance.selectedCharacterName[2].text = NameSelectedButton(gameObject.name);
            blueSelectionIcon.SetActive(true);
        }
        else if (num == 4)
        {
            MenuManager.instance.playerShowcases[3].sprite = ChangeShowcase(gameObject.name);
            MenuManager.instance.selectedCharacterName[3].text = NameSelectedButton(gameObject.name);
            yellowSelectionIcon.SetActive(true);
        }
    }
    private Sprite ChangeShowcase(string name)
    {
        switch (name)
        {
            case "Dragon":
                return MenuManager.instance.showcaseImages[0];
            case "Capitalist":
                return MenuManager.instance.showcaseImages[1];
            case "Raccoon":
                return MenuManager.instance.showcaseImages[2];
            case "Conquistadorette":
                return MenuManager.instance.showcaseImages[3];
            case "Orc":
                return MenuManager.instance.showcaseImages[4];
            case "Cat":
                return MenuManager.instance.showcaseImages[5];
            case "Leprechaun":
                return MenuManager.instance.showcaseImages[6];
            case "Mafia":
                return MenuManager.instance.showcaseImages[7];
            case "Pirate":
                return MenuManager.instance.showcaseImages[8];
            case "Shark":
                return MenuManager.instance.showcaseImages[9];
            default: return null;
        }
    }

    private string NameSelectedButton(string name)
    {
        switch (name)
        {
        case "Dragon":
                return "Bob";
            case "Capitalist":
                return "Cappy Talist";
            case "Raccoon":
                return "Robin Banks";
            case "Conquistadorette":
                return "Emba Zelment";
            case "Orc":
                return "Buzzuz";
            case "Cat":
                return "Tabby Treasure";
            case "Leprechaun":
                return "Goldie Gone";
            case "Mafia":
                return "Tex Avesion";
            case "Pirate":
                return "Gol D. Iger";
            case "Shark":
                return "Loany";
            default: return null;
        }
    }
}
