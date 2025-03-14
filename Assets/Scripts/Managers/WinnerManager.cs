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
    [SerializeField] Transform[] positions;
    [SerializeField] StormManager stormManager;
    [SerializeField] GameObject storm;
    private int coinWinner;
    //private GameObject playerWinner;
    private void Awake()
    {
        instance = this;
    }
    public void WinnerScene()
    {
        stormManager.enabled = false;
        storm.SetActive(false);
        coinWinner = Leaderboard.singleton.FindLeaderMoney();
        //playerWinner = roundManager.winner;
        winnerText.text = Leaderboard.singleton.NameSelectedButton(Leaderboard.singleton.FindLeaderName()); 
        // playerWinner.GetComponent<CharacterControl>().enabled = false;
        // playerWinner.GetComponent<CharacterControl>().bodyPartsGFX[6].SetActive(false);
        // playerWinner.GetComponent<CharacterControl>().bodyPartsGFX[7].SetActive(false);
        coinText.text = coinWinner.ToString();
        mainCamera.SetActive(false);
        winnerCamera.SetActive(true);
        //playerWinner.transform.eulerAngles = new Vector3 (0, 180, 0);
        //playerWinner.transform.position = winnerLocation.position;
        winnerUI.SetActive(true);
        confettiParticle.SetActive(true);
        //Leaderboard.singleton.DisplayPlayerSacks();
        Leaderboard.singleton.ArrangeOnPodium(positions);
        SoundManager.singleton.PlayClip($"{Leaderboard.singleton.FindLeaderName()}WinGame", transform.position, 1f, false, false);


    }
}
