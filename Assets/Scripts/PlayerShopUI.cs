using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class PlayerShopUI : MonoBehaviour
{
    //string objectName;
    [SerializeField] Image portrait;
    //[SerializeField] Sprite[] portraitImages;
    [SerializeField] public TextMeshProUGUI coinUI;
    void Start()
    {
    }
    private void OnEnable()
    {   
        //objectName = gameObject.name;
        //SetPlayerShopUI(objectName);
        portrait.sprite = CharacterInfoHandler.instance.Portrait(gameObject.name);
    }
    // private void SetPlayerShopUI(string name)
    // {
    //     switch (name)
    //     {
    //         case "Dragon":
    //             portrait.sprite = portraitImages[0];
    //             break;
    //         case "Monopoly Dude":
    //             portrait.sprite = portraitImages[1];
    //             break;
    //         case "Dummy":
    //             portrait.sprite = portraitImages[2];
    //             break;
    //         case "Boxhead":
    //             portrait.sprite = portraitImages[3];
    //             break;
    //         case "Orc":
    //             portrait.sprite = portraitImages[4];
    //             break;
    //         case "Cat":
    //             portrait.sprite = portraitImages[5];
    //             break;
    //         case "Leprechaun":
    //             portrait.sprite = portraitImages[6];
    //             break;
    //         case "Mafia":
    //             portrait.sprite = portraitImages[7];
    //             break;
    //         case "Pirate":
    //             portrait.sprite = portraitImages[8];
    //             break;
    //         case "Shark":
    //             portrait.sprite = portraitImages[9];
    //             break;
    //     }
    // }
}
