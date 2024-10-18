using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using VInspector;

public class RoundManager : MonoBehaviour
{
    [Header("Round Settings")]
    public int totalRounds = 5;
    private int currentRound = 0;

    [Header("Player Management")]
    public static bool roundActive = false;

    [Header("Round Management")]
    public UnityEvent roundStart;
    public UnityEvent roundEnd;
    public UnityEvent gameEnd;
    [SerializeField] PlayerManager playerManager;
    [SerializeField] ShopManager shopManager;
    private void Awake() 
    {
        //playerManager = FindAnyObjectByType<PlayerManager>();
        
    }

    void Start()
    {
        StartCoroutine(RoundLoop());
    }

    private IEnumerator RoundLoop()
    {
        while (currentRound != totalRounds)
        {
            Debug.Log($"Round {currentRound} start");
            yield return new WaitUntil(() => playerManager.winnerAnnounced == true);
            roundEnd.Invoke();
            shopManager.Shopping();
            yield return new WaitUntil(() => shopManager.shopUI.activeSelf == false);
            currentRound++;
            playerManager.winnerAnnounced = false; 
            KillAllClones.KillAllCharacters();           
            roundStart.Invoke();
        }
        gameEnd.Invoke();
    }

    [Button]
    private void DebugEndRound()
    {
        playerManager.winnerAnnounced = true;
    }
}
