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
    public UnityEvent gameStart;
    //public UnityEvent roundStart;
    public UnityEvent roundEnd;
    public UnityEvent gameEnd;
    [SerializeField] PlayerManager playerManager;
    [SerializeField] ShopManager shopManager;
    private void Awake() 
    {
        //playerManager = FindAnyObjectByType<PlayerManager>();
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        
    }

    void Start()
    {
        StartCoroutine(RoundLoop());
    }
    private void Update() {
        if (Input.GetKeyDown(KeyCode.T))
        {
            DebugEndRound();
        }
    }

    private IEnumerator RoundLoop()
    {
        gameStart.Invoke();
        while (currentRound != totalRounds)
        {
            Debug.Log($"Round {currentRound} start");
            yield return new WaitUntil(() => PlayerManager.roundOver == true);
            roundEnd.Invoke();
            shopManager.Shopping();
            yield return new WaitUntil(() => shopManager.shopUI.activeSelf == false);
            currentRound++;
            PlayerManager.roundOver = false;
            playerManager.PlayersNextRound();
            NextRound();         
        }
        gameEnd.Invoke();
    }

    [Button]
    private void DebugEndRound()
    {
        PlayerManager.roundOver = true;
    }

    public static void NextRound()
    {
        CharacterControl[] characters = GameObject.FindObjectsOfType<CharacterControl>();
        foreach (CharacterControl character in characters)
        {
            character.NextRound();
        }
    }
}
