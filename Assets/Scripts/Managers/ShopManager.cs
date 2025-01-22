using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.EventSystems;
using System.Collections;
using UnityEngine.UI;
using TMPro;
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
    private void Start()
    {
        PlayerInput[] playerInputs = FindObjectsOfType<PlayerInput>();
        foreach (PlayerInput player in playerInputs)
        {
        }
    }
    private void Update()
    {
    }
    public void Shopping()
    {
        shopUI.SetActive(true);
        PlayerInput[] playerInputs = FindObjectsOfType<PlayerInput>();
        defaultButtonIndex = 0;
        shopUIIndex = 0;
        ResetButtons();
        foreach (PlayerInput player in PlayerManager.instance.activePlayers)
        {
            MultiplayerEventSystem playerEventSystem = player.GetComponentInChildren<MultiplayerEventSystem>();
            if (playerEventSystem != null)
            {
                playerEventSystem.SetSelectedGameObject(null);
                playerEventSystem.SetSelectedGameObject(defaultButtons[defaultButtonIndex]);
                player.actions["UI/Submit"].performed += ctx => OnSubmit(player);
                defaultButtonIndex++;
                CharacterControl characterControl = player.GetComponent<CharacterControl>();
                playerShopUI[shopUIIndex].name = characterControl.HeadGFX.name;
                playerShopUI[shopUIIndex].GetComponent<PlayerShopUI>().coinUI.text = Leaderboard.singleton.GetMoney(characterControl.PlayerID).ToString();
                playerShopUI[shopUIIndex].SetActive(true);
                shopUIIndex++;
            }
        }
        StartCoroutine(TimerCloseShop(shopTimer));
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
                    //playerEventSystem.SetSelectedGameObject(null);
                }
            }
            else
            {
                Debug.Log($"Player {player.playerIndex} pressed submit, but no button is currently selected.");
            }
        }
    }
    private IEnumerator TimerCloseShop(float delay)
    {
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
        shopUI.SetActive(false);
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
}