using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using VInspector;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Unity.VisualScripting;
using TMPro;
public class MenuManager : MonoBehaviour
{
    private bool usingMouse = true;
    private bool usingInput = false;
    [Foldout("Menu Buttons")]
    [SerializeField] GameObject playButton;
    [SerializeField] GameObject optionsButton;
    [SerializeField] GameObject quitButton;
    [EndFoldout]
    [SerializeField] GameObject defaultOptionsButton;
    [SerializeField] GameObject roundMenu;
    [SerializeField] GameObject defaultRoundMenuButton;
    [Foldout("UI Input Images")]
    [SerializeField] Image confirmButton;
    [SerializeField] Image backButton;
    [EndFoldout]
    [Foldout("Xbox UI Elements")]
    [SerializeField] Sprite aButton;
    [SerializeField] Sprite bButton;
    [EndFoldout]
    [Foldout("KNM UI Elements")]
    [SerializeField] Sprite enterButton;
    [SerializeField] Sprite escButton;
    [EndFoldout]
    [SerializeField] GameObject optionsMenu;
    [SerializeField] TextMeshProUGUI roundText;
    void Update()
    {
        if (Mouse.current.delta.ReadValue() != Vector2.zero)
        {
            if (!usingMouse && usingInput)
            {
                usingMouse = true;
                usingInput = false;
                EventSystem.current.SetSelectedGameObject(null);
                KNMUI();
            }
        }
        if ((Keyboard.current.anyKey.wasPressedThisFrame || (Gamepad.current != null && IsGamepadPressed())) && usingMouse && !usingInput)
        {
            if (usingMouse && !usingInput)
            {
                EventSystem.current.SetSelectedGameObject(null);
                EventSystem.current.SetSelectedGameObject(playButton);
                usingMouse = false;
                usingInput = true;
            }
        }
        if (Keyboard.current.anyKey.wasPressedThisFrame)
        {
            KNMUI();
        }
        if (optionsMenu.activeSelf && (Input.GetKeyDown(KeyCode.Escape) || Input.GetButtonDown("Cancel")))
        {
            optionsMenu.SetActive(false);
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(playButton);
            playButton.SetActive(true);
            optionsButton.SetActive(true);
            quitButton.SetActive(true);
        }
        if (roundMenu.activeSelf && (Input.GetKeyDown(KeyCode.Escape) || Input.GetButtonDown("Cancel")))
        {
            roundMenu.SetActive(false);
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(playButton);
            playButton.SetActive(true);
            optionsButton.SetActive(true);
            quitButton.SetActive(true);
        }
    }
    private bool IsGamepadPressed()
    {
        var gamepad = Gamepad.current;
        if (gamepad == null) return false;
        foreach (var control in gamepad.allControls)
        {
            if (control.IsPressed())
            {
                XboxUI();
                return true;
            }
        }
        return false;
    }
    private void XboxUI()
    {
        confirmButton.sprite = aButton;
        backButton.sprite = bButton;
    }
    private void KNMUI()
    {
        confirmButton.sprite = enterButton;
        backButton.sprite = escButton;
    }
    public void AddTotalRounds()
    {
        if (CustomizationManager.instance.roundAmount != 7)
        {
            CustomizationManager.instance.roundAmount++;
            roundText.text = CustomizationManager.instance.roundAmount.ToString();
        }
        else Debug.Log("Maximum amount of rounds reach");
    }
    public void SubTotalRounds()
    {
        if (CustomizationManager.instance.roundAmount != 2)
        {
            CustomizationManager.instance.roundAmount--;
            roundText.text = CustomizationManager.instance.roundAmount.ToString();
        }
        else Debug.Log("Minimum amount of rounds reach");
    }
    public void QuitButton()
    {
        Application.Quit();
    }
    public void PlayButton()
    {
        //SceneManager.LoadScene("MainScene");
        roundMenu.SetActive(true);
        playButton.SetActive(false);
        optionsButton.SetActive(false);
        quitButton.SetActive(false);
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(defaultRoundMenuButton);
    }
    public void OptionsButton()
    {
        //SceneManager.LoadScene("MainScene");
        optionsMenu.SetActive(true);
        playButton.SetActive(false);
        optionsButton.SetActive(false);
        quitButton.SetActive(false);
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(defaultOptionsButton);
    }
    public void StartGameButton()
    {
        SceneManager.LoadScene("MainScene");
    }
}