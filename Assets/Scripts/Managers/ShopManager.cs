using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using System.Collections;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
public class ShopManager : MonoBehaviour
{
    public GameObject[] defaultButtons;
    int defaultButtonIndex = 0;
    public GameObject shopUI;
    public float shopTimer;
    private float internalShopTimer;
    [SerializeField] TextMeshProUGUI timerText;
    [SerializeField] GameObject[] playerShopUI;
    int shopUIIndex;
    [SerializeField] Animator animator;
    //private string richestPlayer;
    private MultiplayerEventSystem richestEventSystem;
    //private PlayerInput richestPI;
    private List<MultiplayerEventSystem> poorPlayers = new List<MultiplayerEventSystem>();
    [SerializeField] Image wantedPosterPortrait;
    [SerializeField] Image wantedPosterTransition;

    private bool richShop = false;
    private bool poorShop = false;
    private int poorplayersbought = 0;
    private bool shortenTimer = false;

    public bool shopping;

    public Animator crowAnim;
    [SerializeField] HorizontalLayoutGroup crowSignLayoutGroup;
    [SerializeField] TextMeshProUGUI crowSignText;
    [SerializeField] Image firstLoserOrRichGuy;
    [SerializeField] Image[] crowSignPortraits;
    private void Start()
    {
        // //PlayerInput[] playerInputs = FindObjectsOfType<PlayerInput>();
        // foreach (PlayerInput player in playerInputs)
        // {
        // }
    }
    // public void Shopping()
    // {
    //     shopUI.SetActive(true);
    //     //PlayerInput[] playerInputs = FindObjectsOfType<PlayerInput>();
    //     defaultButtonIndex = 0;
    //     shopUIIndex = 0;
    //     ResetButtons();
    //     richestEventSystem = Leaderboard.singleton.FindLeaderEventSystem();
    //     foreach (PlayerInput player in PlayerManager.instance.activePlayers)
    //     {
    //         MultiplayerEventSystem playerEventSystem = player.GetComponentInChildren<MultiplayerEventSystem>();
    //         if (playerEventSystem != null)
    //         {
    //             playerEventSystem.SetSelectedGameObject(null);
    //             playerEventSystem.SetSelectedGameObject(defaultButtons[defaultButtonIndex]);
    //             player.actions["UI/Submit"].performed += ctx => OnSubmit(player);
    //             defaultButtonIndex++;
    //             CharacterControl characterControl = player.GetComponent<CharacterControl>();
    //             playerShopUI[shopUIIndex].name = characterControl.HeadGFX.name;
    //             playerShopUI[shopUIIndex].GetComponent<PlayerShopUI>().coinUI.text = Leaderboard.singleton.GetMoney(characterControl.PlayerID).ToString();
    //             playerShopUI[shopUIIndex].SetActive(true);
    //             shopUIIndex++;
    //         }
    //     }
    //     StartCoroutine(TimerCloseShop(shopTimer));
    // }

    private void Update()
    {
        if (shopping)
            internalShopTimer -= Time.unscaledDeltaTime;
    }

    public void Shopping()
    {
        shopping = true;
        shopUI.SetActive(true);
        crowAnim.Play("Hello");
        //PlayerInput[] playerInputs = FindObjectsOfType<PlayerInput>();
        defaultButtonIndex = 0;
        shopUIIndex = 0;
        ResetButtons();
        richestEventSystem = Leaderboard.singleton.FindLeaderEventSystem();
        //richestPI = Leaderboard.singleton.FindLeaderInput();
        foreach (PlayerInput player in PlayerManager.instance.activePlayers)
        {
            MultiplayerEventSystem playerEventSystem = player.GetComponentInChildren<MultiplayerEventSystem>();
            if (playerEventSystem != null)
            {
                //playerEventSystem.SetSelectedGameObject(null);
                //playerEventSystem.SetSelectedGameObject(defaultButtons[defaultButtonIndex]);
                player.actions["UI/Submit"].performed += ctx => OnSubmit(player);
                //defaultButtonIndex++;
                CharacterControl characterControl = player.GetComponent<CharacterControl>();
                playerShopUI[shopUIIndex].name = characterControl.HeadGFX.name;
                playerShopUI[shopUIIndex].GetComponent<PlayerShopUI>().coinUI.text = Leaderboard.singleton.GetMoney(characterControl.PlayerID).ToString();
                playerShopUI[shopUIIndex].SetActive(true);
                shopUIIndex++;
                if (playerEventSystem != richestEventSystem)
                {
                    poorPlayers.Add(playerEventSystem);
                }
            }
        }
            StartCoroutine(RichShopFirst());
    }

    private IEnumerator RichShopFirst()
    {
        richShop = true;
        CrowSign();
        yield return new WaitUntil(() => crowAnim.GetCurrentAnimatorStateInfo(0).IsName("Idle"));
        //yield return new WaitUntil(() => crowAnim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f && !anim.IsInTransition(0));
        internalShopTimer = shopTimer;
        richestEventSystem.SetSelectedGameObject(null);
        richestEventSystem.SetSelectedGameObject(defaultButtons[defaultButtonIndex]);
        while (internalShopTimer > 0)
        {
            if (shortenTimer && internalShopTimer >= 2)
            {
                internalShopTimer = 2;
                shortenTimer = false;
            }

            timerText.text = $"Timer: {internalShopTimer.ToString("F1")}";

            if (internalShopTimer <= 5f)
            {
                animator.Play("ShopTimer");
            }
            yield return null;
        }
        richShop = false;
        shortenTimer = false;
        richestEventSystem.SetSelectedGameObject(null);
        StartCoroutine(PoorShopSecond());
        //richestPI.actions["UI/Submit"].performed -= ctx => OnSubmit(richestPI);
        //shopUI.SetActive(false);
        //PlayerInput[] playerInputs = FindObjectsOfType<PlayerInput>();
        // foreach (PlayerInput player in playerInputs)
        // {
        //     player.actions["UI/Submit"].performed -= ctx => OnSubmit(player);
        //     MultiplayerEventSystem playerEventSystem = player.GetComponentInChildren<MultiplayerEventSystem>();
        //     if (playerEventSystem != null)
        //     {
        //         playerEventSystem.SetSelectedGameObject(null);
        //     }
        // }
    }
    private IEnumerator PoorShopSecond()
    {
        poorShop = true;
        CrowSign();
        crowAnim.SetTrigger("IdleToPoor");
        yield return new WaitUntil(() => crowAnim.GetCurrentAnimatorStateInfo(0).IsName("Idle 2"));
        internalShopTimer = shopTimer;
        //Leaderboard.singleton.AnnounceText("Now the poor");
        foreach (MultiplayerEventSystem player in poorPlayers)
        {
            player.SetSelectedGameObject(null);
            player.SetSelectedGameObject(defaultButtons[defaultButtonIndex]);
            defaultButtonIndex++;
        }
        while (internalShopTimer > 0)
        {
            timerText.text = $"Timer: {internalShopTimer.ToString("F1")}";

            if (internalShopTimer <= 5f)
            {
                animator.Play("ShopTimer");
            }
            if (shortenTimer && internalShopTimer >= 2)
            {
                internalShopTimer = 2;
                shortenTimer = false;
            }
            yield return null;
        }
        PlayerInput[] playerInputs = Leaderboard.singleton.GetPlayerInputs();
        foreach (PlayerInput player in playerInputs)
        {
            player.actions["UI/Submit"].performed -= ctx => OnSubmit(player);
            MultiplayerEventSystem playerEventSystem = player.GetComponentInChildren<MultiplayerEventSystem>();
            if (playerEventSystem != null)
            {
                playerEventSystem.SetSelectedGameObject(null);
            }
        }
        poorShop = false;
        shortenTimer = false;
        //shopUI.SetActive(false);
        poorPlayers.Clear();
        shopping = false;
        crowAnim.SetTrigger("PoorToBye");
        crowSignPortraits[0].gameObject.SetActive(false);
        crowSignPortraits[1].gameObject.SetActive(false);
        crowSignPortraits[2].gameObject.SetActive(false);
    }

    private void ResetButtons()
    {
        foreach (GameObject button in defaultButtons)
        {
            ButtonSelectionTracker tracker = button.GetComponent<ButtonSelectionTracker>();
            button.GetComponent<Button>().interactable = true;
            tracker.soldUI.SetActive(false);
            tracker.redSelectionIcon.SetActive(false);
            tracker.blueSelectionIcon.SetActive(false);
            tracker.greenSelectionIcon.SetActive(false);
            tracker.yellowSelectionIcon.SetActive(false);
        }
    }
    public void WantedPosters()
    {
        //richestPlayer = Leaderboard.singleton.FindLeaderName();
        wantedPosterPortrait.sprite = Leaderboard.singleton.SetPlayerPortrait(Leaderboard.singleton.FindLeaderName());
        wantedPosterTransition.sprite = Leaderboard.singleton.SetPlayerPortrait(Leaderboard.singleton.FindLeaderName());

    }

    public void CrowSign()
    {
        if (richShop)
        {
            crowSignLayoutGroup.spacing = 0f;
            firstLoserOrRichGuy.sprite = Leaderboard.singleton.SetPlayerPortrait(Leaderboard.singleton.FindLeaderName());
            crowSignPortraits[0].gameObject.SetActive(true);
            crowSignText.text = "Rich buys 1st";
        }
        else
        {
            crowSignText.text = "poor buys 2nd";
            if (poorPlayers.Count == 1)
            {
                crowSignLayoutGroup.spacing = 0f;
                firstLoserOrRichGuy.sprite = Leaderboard.singleton.SetPlayerPortrait(Leaderboard.singleton.DetermineLosers(0));
            }
            else if (poorPlayers.Count == 2)
            {
                crowSignLayoutGroup.spacing = 3f;
                firstLoserOrRichGuy.sprite = Leaderboard.singleton.SetPlayerPortrait(Leaderboard.singleton.DetermineLosers(0));
                crowSignPortraits[1].sprite = Leaderboard.singleton.SetPlayerPortrait(Leaderboard.singleton.DetermineLosers(1));
                crowSignPortraits[1].gameObject.SetActive(true);
            }
            else if (poorPlayers.Count == 3)
            {
                crowSignLayoutGroup.spacing = -13f;
                firstLoserOrRichGuy.sprite = Leaderboard.singleton.SetPlayerPortrait(Leaderboard.singleton.DetermineLosers(0));
                crowSignPortraits[1].sprite = Leaderboard.singleton.SetPlayerPortrait(Leaderboard.singleton.DetermineLosers(1));
                crowSignPortraits[2].sprite = Leaderboard.singleton.SetPlayerPortrait(Leaderboard.singleton.DetermineLosers(2));
                crowSignPortraits[1].gameObject.SetActive(true);
                crowSignPortraits[2].gameObject.SetActive(true);
            }
        }
    }
    private void OnSubmit(PlayerInput player)
    {
        MultiplayerEventSystem playerEventSystem = player.GetComponentInChildren<MultiplayerEventSystem>();
        if (playerEventSystem != null)
        {
            GameObject selectedButton = playerEventSystem.currentSelectedGameObject;
            if (selectedButton != null)
            {
                int itemPrice = selectedButton.GetComponent<ButtonSelectionTracker>().itemPrice;
                if (Leaderboard.singleton.GetMoney(PlayerManager.instance.playerList[player.playerIndex].GetComponent<CharacterControl>().PlayerID) >= itemPrice)
                {
                    Leaderboard.singleton.ModifyMoney(PlayerManager.instance.playerList[player.playerIndex].GetComponent<CharacterControl>().PlayerID, -itemPrice);
                    if (selectedButton.GetComponent<ButtonSelectionTracker>().type == ButtonSelectionTracker.shopItemType.Weapon)
                    {
                        PlayerManager.instance.playerList[player.playerIndex].GetComponent<CharacterControl>().BuyWeapon(selectedButton.name);
                        playerEventSystem.SetSelectedGameObject(null);
                    }
                    else if (selectedButton.GetComponent<ButtonSelectionTracker>().type == ButtonSelectionTracker.shopItemType.Consumable)
                    {
                        PlayerManager.instance.playerList[player.playerIndex].GetComponent<CharacterControl>().BuyConsumeable(selectedButton.name);
                    }
                    shopUIIndex = player.playerIndex;
                    playerShopUI[shopUIIndex].GetComponent<PlayerShopUI>().coinUI.text = Leaderboard.singleton.GetMoney(PlayerManager.instance.playerList[player.playerIndex].GetComponent<CharacterControl>().PlayerID).ToString();
                    selectedButton.GetComponent<Button>().interactable = false;
                    selectedButton.GetComponent<ButtonSelectionTracker>().soldUI.SetActive(true);
                    selectedButton.GetComponent<ButtonSelectionTracker>().itemPrice += 3;
                    selectedButton.GetComponent<ButtonSelectionTracker>().priceText.text = selectedButton.GetComponent<ButtonSelectionTracker>().itemPrice.ToString();
                    SoundManager.singleton.PlayClip($"Pay", transform.position, 0.4f, false, false); //return with sound
                    WantedPosters();
                    if (richShop)
                    {
                        shortenTimer = true;
                    }
                    if (poorShop)
                    {
                        poorplayersbought++;
                        if (poorplayersbought == poorPlayers.Count)
                        {
                            shortenTimer = true;
                            poorplayersbought = 0;
                        }
                    }
                    //playerEventSystem.SetSelectedGameObject(null);
                }
            }
            else
            {
                Debug.Log($"Player {player.playerIndex} pressed submit, but no button is currently selected.");
            }
        }
    }
}