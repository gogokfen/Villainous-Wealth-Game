using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonSelection : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    public Button button;
    [SerializeField] Sprite offSprite;
    [SerializeField] Sprite onSprite;
    public void OnSelect(BaseEventData eventData)
    {
        button.image.sprite = onSprite;
        SoundManager.singleton.PlayClip($"UI", transform.position, 1f, false, false); //return with sound
    }

    public void OnDeselect(BaseEventData eventData)
    {
        button.image.sprite = offSprite;
    }
}
