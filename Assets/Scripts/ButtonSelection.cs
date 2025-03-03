using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class ButtonSelection : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    [Header("UI Elements")]
    public Button button;
    [SerializeField] private Sprite offSprite;
    [SerializeField] private Sprite onSprite;
    [Header("Shader Target")]
    [SerializeField] private Image targetImage;
    private Material instanceMaterial;
    private void Start()
    {
        if (targetImage != null)
        {
            instanceMaterial = new Material(targetImage.material);
            targetImage.material = instanceMaterial;
        }
    }
    public void OnSelect(BaseEventData eventData)
    {
        button.image.sprite = onSprite;
        SoundManager.singleton.PlayClip("UI", transform.position, 1f, false, false);
        if (instanceMaterial != null)
        {
            instanceMaterial.SetFloat("_Amount", 1f);
        }
    }
    public void OnDeselect(BaseEventData eventData)
    {
        button.image.sprite = offSprite;
        if (instanceMaterial != null)
        {
            instanceMaterial.SetFloat("_Amount", 0f);
        }
    }
}
