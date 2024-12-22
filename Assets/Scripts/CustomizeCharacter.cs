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
                break;
            case 2:
                greenSelectionIcon.SetActive(true);
                break;
            case 3:
                blueSelectionIcon.SetActive(true);
                break;
            case 4:
                yellowSelectionIcon.SetActive(true);
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
            redSelectionIcon.SetActive(true);
        else if (num == 2)
            greenSelectionIcon.SetActive(true);
        else if (num == 3)
            blueSelectionIcon.SetActive(true);
        else if (num == 4)
            yellowSelectionIcon.SetActive(true);
    }
}
