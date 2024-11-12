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
    [SerializeField] public GameObject soldUI;

    [Foldout("Selection Icons")]
    [SerializeField] public GameObject redSelectionIcon;
    [SerializeField] public GameObject greenSelectionIcon;
    [SerializeField] public GameObject blueSelectionIcon;
    [SerializeField] public GameObject yellowSelectionIcon;
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
            redSelectionIcon.SetActive(true);
        else if (eventData.currentInputModule.GetComponent<CharacterControl>().PlayerID == CharacterControl.PlayerTypes.Green)
            greenSelectionIcon.SetActive(true);
        else if (eventData.currentInputModule.GetComponent<CharacterControl>().PlayerID == CharacterControl.PlayerTypes.Blue)
            blueSelectionIcon.SetActive(true);
        else if (eventData.currentInputModule.GetComponent<CharacterControl>().PlayerID == CharacterControl.PlayerTypes.Yellow)
            yellowSelectionIcon.SetActive(true);
        
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
}