using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using VInspector;
using Cinemachine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;
public class PlayerManager : MonoBehaviour
{
    public static PlayerManager instance;
    public bool singleplayerTesting;
    // [SerializeField] Transform[] startPositions;
    public int characterAmount;
    [SerializeField] GameObject[] playerPrefabs;
    [SerializeField] GameObject gamepadDisconnectedUI;

    

    private List<CharacterControl.PlayerTypes> availablePlayerID;
    public List<PlayerInput> activePlayers;
    public static bool roundOver;
    public static int playerCount;
    int playerIndex = 0;
    int disconnectedPlayers = 0;
    public GameObject[] playerList;

    [SerializeField] GameObject characterSelectionScreen;
    private List<InputDevice> joinedDevices = new List<InputDevice>();
    [SerializeField] GameObject menuPlayerPrefab;
    [SerializeField] GameObject defaultCharacterButton;
    public int playerNumber = 1;

    private int[] characterPicks;
    private int pickingPlayer = 1;
    private static List<PlayerInput> menuPlayers = new List<PlayerInput>();
    private int readyPlayers = 0;
    [Foldout("Player Materials")]
    public Material redMaterial;
    public Material greenMaterial;
    public Material blueMaterial;
    public Material yellowMaterial;
    [EndFoldout]

    private void Awake()
    {
        instance = this;
        roundOver = false;
        DontDestroyOnLoad(gameObject);
        characterPicks = new int[characterAmount];
    }
    private void Update()
    {
        if (readyPlayers == joinedDevices.Count && readyPlayers >= 2)
        {
            MenuManager.instance.weReadyCheck = true;
            MenuManager.instance.WeReady();
        }    
        else 
        {
            MenuManager.instance.weReadyCheck = false;
            MenuManager.instance.WeReady();
        }

        if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            if (characterSelectionScreen.activeInHierarchy == true)
            {
                DetectPlayerJoin();
            }
            else 
            {
                playerNumber = 1;
                joinedDevices.Clear();
                readyPlayers = 0;
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
                    menuPlayers.Add(menuPlayer);
                    MultiplayerEventSystem player = menuPlayer.GetComponent<MultiplayerEventSystem>();
                    joinedDevices.Add(device);
                    player.GetComponent<MenuPlayer>().playerNum = playerNumber;
                    player.SetSelectedGameObject(null);
                    player.SetSelectedGameObject(defaultCharacterButton);
                    defaultCharacterButton.GetComponent<CustomizeCharacter>().FirstTimeSelection(playerNumber);
                    playerNumber++;
                    menuPlayer.actions["UI/Submit"].performed += ctx => CharacterPick(menuPlayer);
                    menuPlayer.actions["UI/Cancel"].performed += ctx => CancelCharacterPick(menuPlayer);
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
            if (playerEventSystem.GetComponent<MenuPlayer>().ready == false)
            {
                playerEventSystem.GetComponent<MenuPlayer>().ready = true;
                readyPlayers++;
            }
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
            if (playerEventSystem.GetComponent<MenuPlayer>().ready == true)
            {
                playerEventSystem.GetComponent<MenuPlayer>().ready = false;
                readyPlayers--;
            }
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
        //cameraParent.SetActive(true);
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
        playerIndex = 0;
        playerList = new GameObject[InputSystem.devices.Count];
        foreach (var device in joinedDevices)
        {
            if (device is Gamepad || device is Keyboard)
            {
                for (int i = 0; i < characterPicks.Length; i++)
                {
                    if (characterPicks[i] == pickingPlayer)
                    {
                        PlayerInput playerInput = PlayerInput.Instantiate(playerPrefabs[i], controlScheme: null, pairWithDevice: device);
                        playerList[playerInput.playerIndex] = playerInput.gameObject;
                        playerInput.transform.position = MapManager.instance.startPositions[playerIndex].position;
                        CharacterControl characterControl = playerInput.GetComponent<CharacterControl>();
                        if (availablePlayerID.Count > 0)
                        {
                            CharacterControl.PlayerTypes assignedPlayerID = availablePlayerID[0];
                            characterControl.PlayerID = assignedPlayerID;
                            availablePlayerID.Remove(assignedPlayerID);
                            playerInput.gameObject.name = $"Player_{assignedPlayerID}";
                            //cameraGroup.AddMember(playerInput.gameObject.transform, 1f, 0f);
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
            playerInput.transform.position = MapManager.instance.startPositions[playerIndex].position;
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

    public static void KillMenuPlayers()
    {
        for (int i = menuPlayers.Count - 1; i >= 0; i--)
        {
            if (menuPlayers[i] != null)
            {
                Destroy(menuPlayers[i].gameObject);
            }
        }
        menuPlayers.Clear();
        
    }
}
