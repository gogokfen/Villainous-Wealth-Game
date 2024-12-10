using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using VInspector;
using Cinemachine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
public class PlayerManager : MonoBehaviour
{
    public static PlayerManager instance;
    public bool singleplayerTesting;
    [SerializeField] Transform[] startPositions;
    [SerializeField] GameObject[] playerPrefabs;
    [SerializeField] GameObject gamepadDisconnectedUI;

    [Foldout("Camera")]
    [SerializeField] GameObject cameraParent;
    [SerializeField] CinemachineTargetGroup cameraGroup;
    [EndFoldout]



    private List<CharacterControl.PlayerTypes> availablePlayerID;
    private List<PlayerInput> activePlayers;
    public static bool roundOver;
    public static int playerCount;
    int playerIndex = 0;
    int disconnectedPlayers = 0;
    public GameObject[] playerList;

    [SerializeField] GameObject characterSelectionScreen;
    private List<InputDevice> joinedDevices = new List<InputDevice>();
    [SerializeField] GameObject menuPlayerPrefab;
    [SerializeField] GameObject defaultCharacterButton;
    private int characterSelection;
    private int playerNumber = 1;

    public int[] characterPicks;
    public int pickingPlayer = 1;

    [Foldout("Player Materials")]
    public Material redMaterial;
    public Material greenMaterial;
    public Material blueMaterial;
    public Material yellowMaterial;
    [EndFoldout]

    public float holdDuration = 1.5f;
    private float holdTime = 0f;
    public Image holdImage;
    

    [Button("Go To Main Scene")]
    public void MainScene()
    {
        SceneManager.LoadScene("MainScene");
    }

    private void Awake()
    {
        instance = this;
        roundOver = false;
        DontDestroyOnLoad(gameObject);
        characterPicks = new int[4];
    }
    private void Update()
    {
        if (Input.GetKey(KeyCode.Escape) && characterSelectionScreen.activeInHierarchy == true)
        {
            holdTime += Time.deltaTime;
            holdImage.fillAmount = holdTime / holdDuration;
            Debug.Log(holdTime);
            if (holdTime >= holdDuration)
            {
                characterSelectionScreen.SetActive(false);
            }
        }
        else 
        {
            holdTime = 0f;
            holdImage.fillAmount = 0f;
        }

        if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            if (characterSelectionScreen.activeInHierarchy == true)
            {
                //characterScreenOn = true;
                
                DetectPlayerJoin();
            }
            if (characterSelectionScreen.activeInHierarchy == false)
            {
                //kill all fake players
                holdImage.fillAmount = 0f;
                //characterScreenOn = false;
            }
        }
        InputSystem.onDeviceChange += (device, change) =>
        {
            switch (change)
            {
                case InputDeviceChange.Disconnected:
                    if (!Application.isPlaying) return;
                    Debug.Log($"Device {device.name} has been disconnected.");
                    gamepadDisconnectedUI.SetActive(true);
                    disconnectedPlayers++;
                    Time.timeScale = 0f;
                    break;
                case InputDeviceChange.Reconnected:
                    Debug.Log($"Device {device.name} has been reconnected.");
                    disconnectedPlayers--;
                    if (disconnectedPlayers == 0)
                    {
                        gamepadDisconnectedUI.SetActive(false);
                        Time.timeScale = 1f;
                    }
                    break;
            }
        };
    }
    private void DetectPlayerJoin()
    {
        foreach (var device in InputSystem.devices)
        {
            if ((device is Gamepad || device is Keyboard) && !joinedDevices.Contains(device))
            {
                if (device is Gamepad gamepad && gamepad.buttonSouth.wasPressedThisFrame
                || device is Keyboard keyboard && keyboard.enterKey.wasPressedThisFrame)
                {
                    PlayerInput menuPlayer = PlayerInput.Instantiate(menuPlayerPrefab, controlScheme: null, pairWithDevice: device);
                    MultiplayerEventSystem player = menuPlayer.GetComponent<MultiplayerEventSystem>();
                    player.SetSelectedGameObject(null);
                    player.SetSelectedGameObject(defaultCharacterButton);
                    joinedDevices.Add(device);
                    player.GetComponent<MenuPlayer>().playerNum = playerNumber;
                    playerNumber++;
                    menuPlayer.actions["UI/Submit"].performed += ctx => CharacterPick(menuPlayer);
                    menuPlayer.actions["UI/Cancel"].performed += ctx => CancelCharacterPick(menuPlayer);
                    Debug.Log(joinedDevices);
                }
            }
        }
    }

    private void CharacterPick(PlayerInput player)
    {
        MultiplayerEventSystem playerEventSystem = player.GetComponent<MultiplayerEventSystem>();
        GameObject selectedButton = playerEventSystem.currentSelectedGameObject;
        if (selectedButton.GetComponent<CustomizeCharacter>().picked == false)
        {
        MenuPlayer menuPlayer = playerEventSystem.GetComponent<MenuPlayer>();
        menuPlayer.selectedChar = selectedButton.GetComponent<CustomizeCharacter>().characterNum;
        menuPlayer.pickedCharButton = selectedButton;
        playerEventSystem.GetComponent<MenuPlayer>().selectedChar = selectedButton.GetComponent<CustomizeCharacter>().characterNum;
        characterPicks[playerEventSystem.GetComponent<MenuPlayer>().selectedChar] = playerEventSystem.GetComponent<MenuPlayer>().playerNum;
        selectedButton.GetComponent<Button>().interactable = false;
        selectedButton.GetComponent<CustomizeCharacter>().pickedUI.SetActive(true);
        playerEventSystem.SetSelectedGameObject(null);
        selectedButton.GetComponent<CustomizeCharacter>().picked = true;
        if (selectedButton.name == "Dragon")
        {
            SoundManager.singleton.AnnounceDragon();
        }
        else if (selectedButton.name == "Boxhead")
        {
            SoundManager.singleton.AnnounceBoxhead();
        }
        else if (selectedButton.name == "TestDummy")
        {
            SoundManager.singleton.AnnounceTestDummy();
        }
        else if (selectedButton.name == "MonopolyDude")
        {
            SoundManager.singleton.AnnounceMonopolyDude();
        }
        }
    }

    private void CancelCharacterPick(PlayerInput player)
{
    MenuPlayer menuPlayer = player.GetComponent<MenuPlayer>();
    if (menuPlayer.selectedChar >= 0)
    {
        GameObject selectedButton = menuPlayer.pickedCharButton;
        characterPicks[menuPlayer.selectedChar] = 0;
        menuPlayer.selectedChar = 0;
        selectedButton.GetComponent<Button>().interactable = true;
        selectedButton.GetComponent<CustomizeCharacter>().pickedUI.SetActive(false);
        selectedButton.GetComponent<CustomizeCharacter>().picked = false;
        MultiplayerEventSystem playerEventSystem = player.GetComponent<MultiplayerEventSystem>();
        playerEventSystem.SetSelectedGameObject(null);
        playerEventSystem.SetSelectedGameObject(defaultCharacterButton);
        menuPlayer.pickedCharButton = null;
    }
}

    public void StartRound()
    {
        if (activePlayers == null)
        {
            activePlayers = new List<PlayerInput>();
        }
        else
        {
            activePlayers.Clear();
        }
        AvailablePlayerIDs();
        AssignPlayers();
        cameraParent.SetActive(true);
    }
    private void AvailablePlayerIDs()
    {
        availablePlayerID = new List<CharacterControl.PlayerTypes>
    {
        CharacterControl.PlayerTypes.Red,
        CharacterControl.PlayerTypes.Green,
        CharacterControl.PlayerTypes.Blue,
        CharacterControl.PlayerTypes.Yellow
    };
    }
    public void AssignPlayers()
    {
        playerList = new GameObject[InputSystem.devices.Count];
        foreach (var device in joinedDevices)
        {
            if (device is Gamepad || device is Keyboard)
            {
                for (int i = 0; i < characterPicks.Length; i++)
                {
                    if (characterPicks[i] == pickingPlayer)
                    {
                        var playerInput = PlayerInput.Instantiate(playerPrefabs[i], controlScheme: null, pairWithDevice: device);

                        playerList[playerInput.playerIndex] = playerInput.gameObject;
                        playerInput.transform.position = startPositions[playerIndex].position;
                        CharacterControl characterControl = playerInput.GetComponent<CharacterControl>();
                        if (availablePlayerID.Count > 0)
                        {
                            CharacterControl.PlayerTypes assignedPlayerID = availablePlayerID[0];
                            characterControl.PlayerID = assignedPlayerID;
                            availablePlayerID.Remove(assignedPlayerID);
                            playerInput.gameObject.name = $"Player_{assignedPlayerID}";
                            cameraGroup.AddMember(playerInput.gameObject.transform, 1f, 0f);
                            ChangePlayerMaterial(playerInput.gameObject, assignedPlayerID);
                            activePlayers.Add(playerInput);
                            playerIndex++;
                            playerCount++;
                        }
                    }
                }
                pickingPlayer++;
            }
        }
    }

    public void PlayersNextRound()
    {
        playerCount = playerIndex;
        playerIndex = 0;
        foreach (PlayerInput playerInput in activePlayers)
        {
            playerInput.transform.position = startPositions[playerIndex].position;
            CharacterControl characterControl = playerInput.GetComponent<CharacterControl>();
            if (characterControl != null)
            {
                characterControl.NextRound();
            }

            playerIndex++;
        }
    }

    private void ChangePlayerMaterial(GameObject player, CharacterControl.PlayerTypes playerType)
    {
        Transform bodyGFXTransform = player.transform.Find("charParent/Body/BodyGFX");
        if (bodyGFXTransform != null)
        {
            Renderer renderer = bodyGFXTransform.GetComponent<Renderer>();
            Material newMaterial = null;
            switch (playerType)
            {
                case CharacterControl.PlayerTypes.Red:
                    newMaterial = redMaterial;
                    break;
                case CharacterControl.PlayerTypes.Green:
                    newMaterial = greenMaterial;
                    break;
                case CharacterControl.PlayerTypes.Blue:
                    newMaterial = blueMaterial;
                    break;
                case CharacterControl.PlayerTypes.Yellow:
                    newMaterial = yellowMaterial;
                    break;
            }
            renderer.material = newMaterial;
        }
    }
    public static void PlayerCheck()
    {
        playerCount--;

        if (playerCount == 1)
        {
            roundOver = true;
        }
    }
}
