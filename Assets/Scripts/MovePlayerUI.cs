using UnityEngine;
using UnityEngine.UI;
public class PlayerUI : MonoBehaviour
{
    private RectTransform rectTransform;
    [SerializeField] Transform parent;
    string parentName;
    [SerializeField] Image frame;
    [SerializeField] Sprite[] frameImages;
    [SerializeField] Image portrait;
    [SerializeField] Sprite[] portraitImages;
    
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        //Transform parent = GetTopmostParent(transform);
        string parentName = parent.name;
        SetAnchors(parentName);
    }
    // private Transform GetTopmostParent(Transform child)
    // {
    //     Transform currentParent = child;
    //     while (currentParent.parent != null)
    //     {
    //         currentParent = currentParent.parent;
    //     }
    //     return currentParent;
    // }
    private void SetAnchors(string parentName)
    {
        switch (parentName)
        {
            case "Player_Red":
                rectTransform.pivot = new Vector2(1.7f, -0.6f);
                rectTransform.anchorMin = new Vector2(1, 0);
                rectTransform.anchorMax = new Vector2(1, 0);
                frame.sprite = frameImages[0];
                portrait.sprite = portraitImages[0];
                break;
            case "Player_Green":
                rectTransform.pivot = new Vector2(-0.7f, -0.6f);
                rectTransform.anchorMin = new Vector2(0, 0);
                rectTransform.anchorMax = new Vector2(0, 0);
                frame.sprite = frameImages[1];
                portrait.sprite = portraitImages[1];
                break;
            case "Player_Blue":
                rectTransform.pivot = new Vector2(-0.7f, 1.4f);
                rectTransform.anchorMin = new Vector2(0, 1);
                rectTransform.anchorMax = new Vector2(0, 1);
                frame.sprite = frameImages[2];
                portrait.sprite = portraitImages[2];
                break;
            case "Player_Yellow":
                rectTransform.pivot = new Vector2(1.7f, 1.4f);
                rectTransform.anchorMin = new Vector2(1, 1);
                rectTransform.anchorMax = new Vector2(1, 1);
                frame.sprite = frameImages[3];
                portrait.sprite = portraitImages[3];
                break;
        }
    }
}
