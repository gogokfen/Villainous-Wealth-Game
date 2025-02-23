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
    public static MenuManager instance;
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
    [SerializeField] Image backButtonMenu;
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

    [SerializeField] Animator readyAnim;

    public float holdDuration = 1.5f;
    private float holdTime = 0f;
    public Image holdImage;

    public CanvasGroup menuButtons;
    public GameObject characterSelectionScreen;
    public bool weReadyCheck;
    public Button startButton;
    public TextMeshProUGUI requirementText;
    [SerializeField] Animator curtain;

    public Sprite[] showcaseImages;
    public Sprite joinShowcaseImage;
    public Image[] playerShowcases;
    public TextMeshProUGUI[] selectedCharacterName;
    public EventSystem eventSystem;

    [Button("Go To Main Scene")]
    public void MainScene()
    {
        StartCoroutine(LoadYourAsyncScene());
    }


    public IEnumerator LoadYourAsyncScene()
    {
        curtain.Play("CurtainClose");
        yield return new WaitForSeconds(1);
        AsyncOperation operation = SceneManager.LoadSceneAsync("MainScene");
        operation.allowSceneActivation = false;
        while (!operation.isDone)
        {
            if (operation.progress >= 0.9f)
            {
                operation.allowSceneActivation = true;
            }
            yield return null;
        }
    }

    

    private void Awake()
    {
        instance = this;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
    void Update()
    {
        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.JoystickButton7)) && weReadyCheck == true)
        {
            MainScene();
        }

        if (characterSelectionScreen.activeInHierarchy == true)
        {
            menuButtons.interactable = false;
            if ((Input.GetKey(KeyCode.Escape) || Input.GetButton("Cancel")) && characterSelectionScreen.activeInHierarchy == true)
            {
                holdTime += Time.deltaTime;
                holdImage.fillAmount = holdTime / holdDuration;
                if (holdTime >= holdDuration)
                {
                    holdImage.fillAmount = 0f;
                    menuButtons.interactable = true;
                    playButton.SetActive(true);
                    optionsButton.SetActive(true);
                    quitButton.SetActive(true);
                    ResetShowcases();
                    PlayerManager.KillMenuPlayers();
                    eventSystem.SetSelectedGameObject(null);
                    eventSystem.SetSelectedGameObject(playButton);
                    characterSelectionScreen.SetActive(false);
                    eventSystem.gameObject.SetActive(true);
                }
            }
            else
            {
                holdTime = 0f;
                holdImage.fillAmount = 0f;
            }
        }
        // if (Mouse.current.delta.ReadValue() != Vector2.zero)
        // {
        //     if (!usingMouse && usingInput)
        //     {
        //         usingMouse = true;
        //         usingInput = false;
        //         //EventSystem.current.SetSelectedGameObject(null);
        //         KNMUI();
        //     }
        // }
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
            backButtonMenu.gameObject.SetActive(false);
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
        backButtonMenu.sprite = bButton;
    }
    private void KNMUI()
    {
        confirmButton.sprite = enterButton;
        backButton.sprite = escButton;
        backButtonMenu.sprite = escButton;
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
        //roundMenu.SetActive(true);
        //backButtonMenu.gameObject.SetActive(true);
        playButton.SetActive(false);
        optionsButton.SetActive(false);
        quitButton.SetActive(false);
        eventSystem.SetSelectedGameObject(null);
        eventSystem.gameObject.SetActive(false);
        characterSelectionScreen.SetActive(true);
        //EventSystem.current.SetSelectedGameObject(defaultRoundMenuButton);
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

    public void WeReady()
    {
        if (weReadyCheck)
        {
            startButton.interactable = true;
            requirementText.text = "Lets do this!";
            readyAnim.gameObject.SetActive(true);
            readyAnim.Play("ShopTimer");
        }
        else
        {
            startButton.interactable = false;
            requirementText.text = "Not enough players are ready";
            readyAnim.StopPlayback();
            readyAnim.gameObject.SetActive(false);
        }
    }

    public void ResetShowcases()
    {
        foreach (Image image in playerShowcases)
        {
            image.sprite = joinShowcaseImage;
        }
    }
}