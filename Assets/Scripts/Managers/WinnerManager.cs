using UnityEngine;
using TMPro;

public class WinnerManager : MonoBehaviour
{
    public static WinnerManager instance;
    [SerializeField] RoundManager roundManager;
    [SerializeField] Transform winnerLocation;
    [SerializeField] TextMeshProUGUI coinText;
    [SerializeField] GameObject mainCamera;
    [SerializeField] GameObject winnerCamera;
    [SerializeField] GameObject winnerUI;
    [SerializeField] GameObject confettiParticle;
    [SerializeField] TextMeshProUGUI winnerText;
    private int coinWinner;
    private GameObject playerWinner;

    private void Awake() 
    {
        instance = this;
    }
    public void WinnerScene()
    {
        coinWinner = roundManager.winner.GetComponent<CharacterControl>().coins;
        playerWinner = roundManager.winner;
        winnerText.text = playerWinner.GetComponent<CharacterControl>().HeadGFX.name;
        playerWinner.GetComponent<CharacterControl>().enabled = false;
        playerWinner.GetComponent<CharacterControl>().bodyPartsGFX[6].SetActive(false);
        playerWinner.GetComponent<CharacterControl>().bodyPartsGFX[7].SetActive(false);
        coinText.text = coinWinner.ToString();
        mainCamera.SetActive(false);
        winnerCamera.SetActive(true);
        playerWinner.transform.eulerAngles = new Vector3 (0, 180, 0);
        playerWinner.transform.position = winnerLocation.position;
        winnerUI.SetActive(true);
        confettiParticle.SetActive(true);
    }
}
