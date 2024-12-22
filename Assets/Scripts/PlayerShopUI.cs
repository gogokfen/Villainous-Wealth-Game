using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class PlayerShopUI : MonoBehaviour
{
    string objectName;
    [SerializeField] Image portrait;
    [SerializeField] Sprite[] portraitImages;
    [SerializeField] public TextMeshProUGUI coinUI;
    void Start()
    {
    }
    private void OnEnable()
    {   
        objectName = gameObject.name;
        SetPlayerShopUI(objectName);
    }
    private void SetPlayerShopUI(string name)
    {
        switch (name)
        {
            case "Dragon":
                portrait.sprite = portraitImages[0];
                break;
            case "Monopoly Dude":
                portrait.sprite = portraitImages[1];
                break;
            case "Dummy":
                portrait.sprite = portraitImages[2];
                break;
            case "Boxhead":
                portrait.sprite = portraitImages[3];
                break;
            case "Big Daddy":
                portrait.sprite = portraitImages[4];
                break;
            case "Guardtron":
                portrait.sprite = portraitImages[5];
                break;
            case "RI":
                portrait.sprite = portraitImages[6];
                break;
            case "The Sheeper":
                portrait.sprite = portraitImages[7];
                break;
            case "Jacko":
                portrait.sprite = portraitImages[8];
                break;
            case "PC Pirate":
                portrait.sprite = portraitImages[9];
                break;
            case "Kerenboy":
                portrait.sprite = portraitImages[10];
                break;
            case "Nuke Man":
                portrait.sprite = portraitImages[11];
                break;
            case "Stronghold Smasher":
                portrait.sprite = portraitImages[12];
                break;
            case "Zolda":
                portrait.sprite = portraitImages[13];
                break;
            case "Donte":
                portrait.sprite = portraitImages[14];
                break;
            case "Booba":
                portrait.sprite = portraitImages[15];
                break;
            case "Shamayim":
                portrait.sprite = portraitImages[16];
                break;
        }
    }
}
