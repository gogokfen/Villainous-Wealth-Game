using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using VInspector;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
public class MenuManager : MonoBehaviour
{
    private bool usingMouse = true;
    private bool usingInput = false;
    [SerializeField] GameObject defaultButton;
    [SerializeField] GameObject defaultOptionsButton;
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
    void Start()
    {
    }
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
                EventSystem.current.SetSelectedGameObject(defaultButton);
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
            EventSystem.current.SetSelectedGameObject(defaultButton);
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
    public void QuitGame()
    {
        Application.Quit();
    }
    public void LoadGame()
    {
        SceneManager.LoadScene("MainScene");
    }
}