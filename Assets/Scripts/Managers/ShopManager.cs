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
    [SerializeField] TextMeshProUGUI timerText;
    [SerializeField] GameObject[] playerShopUI;
    int shopUIIndex;
    [SerializeField] Animator animator;
    private string richestPlayer;
    private MultiplayerEventSystem richestEventSystem;
    private PlayerInput richestPI;
    private List<MultiplayerEventSystem> poorPlayers = new List<MultiplayerEventSystem>();
    [SerializeField] Image wantedPosterPortrait;
    [SerializeField] Image wantedPosterTransition;
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

    public void Shopping()
    {
        shopUI.SetActive(true);
        //PlayerInput[] playerInputs = FindObjectsOfType<PlayerInput>();
        defaultButtonIndex = 0;
        shopUIIndex = 0;
        ResetButtons();
        richestEventSystem = Leaderboard.singleton.FindLeaderEventSystem();
        richestPI = Leaderboard.singleton.FindLeaderInput();
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
            StartCoroutine(RichShopFirst(shopTimer));
        }
    }

    private IEnumerator RichShopFirst(float delay)
    {
        richestEventSystem.SetSelectedGameObject(null);
        richestEventSystem.SetSelectedGameObject(defaultButtons[defaultButtonIndex]);
        float remainingTime = delay;
        while (remainingTime > 0)
        {
            timerText.text = $"Timer: {remainingTime.ToString("F1")}";
            remainingTime -= Time.unscaledDeltaTime;
            if (remainingTime <= 5f)
            {
                animator.Play("ShopTimer");
            }
            yield return null;
        }
        richestEventSystem.SetSelectedGameObject(null);
        StartCoroutine(PoorShopSecond(10f));
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
    private IEnumerator PoorShopSecond(float delay)
    {
        Leaderboard.singleton.AnnounceText("Now the poor");
        foreach (MultiplayerEventSystem player in poorPlayers)
        {
            player.SetSelectedGameObject(null);
            player.SetSelectedGameObject(defaultButtons[defaultButtonIndex]);
            defaultButtonIndex++;
        }
        float remainingTime = delay;
        while (remainingTime > 0)
        {
            timerText.text = $"Timer: {remainingTime.ToString("F1")}";
            remainingTime -= Time.unscaledDeltaTime;
            if (remainingTime <= 5f)
            {
                animator.Play("ShopTimer");
            }
            yield return null;
        }
        PlayerInput[] playerInputs = FindObjectsOfType<PlayerInput>();
        foreach (PlayerInput player in playerInputs)
        {
            player.actions["UI/Submit"].performed -= ctx => OnSubmit(player);
            MultiplayerEventSystem playerEventSystem = player.GetComponentInChildren<MultiplayerEventSystem>();
            if (playerEventSystem != null)
            {
                playerEventSystem.SetSelectedGameObject(null);
            }
        }
        shopUI.SetActive(false);
        poorPlayers.Clear();
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
        richestPlayer = Leaderboard.singleton.FindLeaderName();
        wantedPosterPortrait.sprite = CharacterInfoHandler.instance.Portrait(richestPlayer);
        wantedPosterTransition.sprite = CharacterInfoHandler.instance.Portrait(richestPlayer);

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
                    WantedPosters();
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