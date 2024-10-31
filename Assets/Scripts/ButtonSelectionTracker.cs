using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

public class ButtonSelectionTracker : MonoBehaviour, ISelectHandler
{
    public void OnSelect(BaseEventData eventData)
    {
        Debug.Log(this.gameObject.name + " was selected");
    }

    public void OnDeselect(BaseEventData eventData)
    {
        Debug.Log(this.gameObject.name + " was deselected");
    }
}