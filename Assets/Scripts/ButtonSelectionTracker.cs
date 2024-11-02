using UnityEngine;
using UnityEngine.EventSystems;

using System.Collections;
using UnityEngine.UI;

using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

using TMPro;
using VInspector;

public class ButtonSelectionTracker : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    public int itemPrice;

    [SerializeField] TextMeshProUGUI priceText;

    [Foldout("Selection Rectangles")]
    [SerializeField] GameObject redSelectionRectangle;
    [SerializeField] GameObject greenSelectionRectangle;
    [SerializeField] GameObject blueSelectionRectangle;
    [SerializeField] GameObject yellowSelectionRectangle;
    [EndFoldout]
    private void Start()
    {
        priceText.text = itemPrice.ToString();
    }

    public void OnSelect(BaseEventData eventData)
    {
        //Debug.Log(this.gameObject.name + " was selected");
        //Debug.Log(eventData.currentInputModule.GetComponent<CharacterControl>().PlayerID);

        if (eventData.currentInputModule.GetComponent<CharacterControl>().PlayerID == CharacterControl.PlayerTypes.Red)
            redSelectionRectangle.SetActive(true);
        else if (eventData.currentInputModule.GetComponent<CharacterControl>().PlayerID == CharacterControl.PlayerTypes.Green)
            greenSelectionRectangle.SetActive(true);
        else if (eventData.currentInputModule.GetComponent<CharacterControl>().PlayerID == CharacterControl.PlayerTypes.Blue)
            blueSelectionRectangle.SetActive(true);
        else if (eventData.currentInputModule.GetComponent<CharacterControl>().PlayerID == CharacterControl.PlayerTypes.Yellow)
            yellowSelectionRectangle.SetActive(true);
        
    }

    public void OnDeselect(BaseEventData eventData)
    {
        if (eventData.currentInputModule.GetComponent<CharacterControl>().PlayerID == CharacterControl.PlayerTypes.Red)
            redSelectionRectangle.SetActive(false);
        else if (eventData.currentInputModule.GetComponent<CharacterControl>().PlayerID == CharacterControl.PlayerTypes.Green)
            greenSelectionRectangle.SetActive(false);
        else if (eventData.currentInputModule.GetComponent<CharacterControl>().PlayerID == CharacterControl.PlayerTypes.Blue)
            blueSelectionRectangle.SetActive(false);
        else if (eventData.currentInputModule.GetComponent<CharacterControl>().PlayerID == CharacterControl.PlayerTypes.Yellow)
            yellowSelectionRectangle.SetActive(false);
    }
}