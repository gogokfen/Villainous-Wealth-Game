using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using VInspector;

public class CharacterSelectionTracker : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    [SerializeField] GameObject characterPrefab;
    [Foldout("Selection Icons")]
    [SerializeField] public GameObject redSelectionIcon;
    [SerializeField] public GameObject greenSelectionIcon;
    [SerializeField] public GameObject blueSelectionIcon;
    [SerializeField] public GameObject yellowSelectionIcon;
    [EndFoldout]
    private void Start()
    {

    }

    public void OnSelect(BaseEventData eventData)
    {
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