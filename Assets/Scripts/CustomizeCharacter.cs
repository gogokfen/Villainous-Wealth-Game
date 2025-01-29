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
    public void OnSelect(BaseEventData eventData)
    {
        var menuPlayer = eventData.currentInputModule?.GetComponent<MenuPlayer>();
        if (menuPlayer == null) return;
        switch (menuPlayer.playerNum)
        {
            case 1:
                redSelectionIcon.SetActive(true);
                MenuManager.instance.playerShowcases[0].sprite = ChangeShowcase(gameObject.name);
                break;
            case 2:
                greenSelectionIcon.SetActive(true);
                MenuManager.instance.playerShowcases[1].sprite = ChangeShowcase(gameObject.name);
                break;
            case 3:
                blueSelectionIcon.SetActive(true);
                MenuManager.instance.playerShowcases[2].sprite = ChangeShowcase(gameObject.name);
                break;
            case 4:
                yellowSelectionIcon.SetActive(true);
                MenuManager.instance.playerShowcases[3].sprite = ChangeShowcase(gameObject.name);
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
        }
        else if (num == 2)
        {
            greenSelectionIcon.SetActive(true);
            MenuManager.instance.playerShowcases[1].sprite = ChangeShowcase(gameObject.name);
        }
        else if (num == 3)
        {
            MenuManager.instance.playerShowcases[2].sprite = ChangeShowcase(gameObject.name);
            blueSelectionIcon.SetActive(true);
        }
        else if (num == 4)
        {
            MenuManager.instance.playerShowcases[3].sprite = ChangeShowcase(gameObject.name);
            yellowSelectionIcon.SetActive(true);
        }
    }
    private Sprite ChangeShowcase(string name)
    {
        switch (name)
        {
            case "Dragon":
                return MenuManager.instance.showcaseImages[0];
            case "MonopolyDude":
                return MenuManager.instance.showcaseImages[1];
            case "TestDummy":
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
}
