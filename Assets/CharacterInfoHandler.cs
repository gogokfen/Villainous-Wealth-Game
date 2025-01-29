using UnityEngine;

public class CharacterInfoHandler : MonoBehaviour
{
    public static CharacterInfoHandler instance;
    public Sprite[] portraits;
    private void Awake()
    {
        instance = this;
    }
    public Sprite Portrait(string name)
    {
        switch (name)
        {
            case "Dragon":
                return portraits[0];

            case "Monopoly Dude":
                return portraits[1];

            case "Dummy":
                return portraits[2];

            case "Conquistadorette":
                return portraits[3];

            case "Orc":
                return portraits[4];

            case "Cat":
                return portraits[5];

            case "Leprechaun":
                return portraits[6];

            case "Mafia":
                return portraits[7];

            case "Pirate":
                return portraits[8];

            case "Shark":
                return portraits[9];
            default: return null;
        }
    }
}
