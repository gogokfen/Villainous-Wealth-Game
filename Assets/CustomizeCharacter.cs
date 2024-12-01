using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
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


    public void OnSelect(BaseEventData eventData)
    {
        //Debug.Log(this.gameObject.name + " was selected");
        //Debug.Log(eventData.currentInputModule.GetComponent<CharacterControl>().PlayerID);

        if (eventData.currentInputModule.GetComponent<MenuPlayer>().playerNum == 1)
            redSelectionIcon.SetActive(true);
        else if (eventData.currentInputModule.GetComponent<MenuPlayer>().playerNum == 2)
            greenSelectionIcon.SetActive(true);
        else if (eventData.currentInputModule.GetComponent<MenuPlayer>().playerNum == 3)
            blueSelectionIcon.SetActive(true);
        else if (eventData.currentInputModule.GetComponent<MenuPlayer>().playerNum == 4)
            yellowSelectionIcon.SetActive(true);
        
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
}
