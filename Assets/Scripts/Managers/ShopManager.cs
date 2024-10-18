using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
public class ShopManager : MonoBehaviour
{
    public GameObject defaultButton;
    public GameObject shopUI;
    public float shopTimer;
    public void Shopping()
    {
        Debug.Log("we shopping");
        shopUI.SetActive(true);
        PlayerInput[] playerInputs = FindObjectsOfType<PlayerInput>();
        foreach (PlayerInput player in playerInputs)
        {
            EventSystem playerEventSystem = player.GetComponentInChildren<MultiplayerEventSystem>();
            if (playerEventSystem != null)
            {
                playerEventSystem.SetSelectedGameObject(null);
                playerEventSystem.SetSelectedGameObject(defaultButton);
            }
        }
        StartCoroutine(TimerCloseShop(shopTimer));
    }
    private IEnumerator TimerCloseShop(float delay)
    {
        yield return new WaitForSeconds(delay);
        shopUI.SetActive(false);
        Debug.Log("Shop UI closed after 5 seconds.");
    }
}
