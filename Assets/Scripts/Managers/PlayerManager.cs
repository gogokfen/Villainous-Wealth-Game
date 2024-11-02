using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using VInspector;
using Cinemachine;
using UnityEngine.SceneManagement;
using TMPro;
public class PlayerManager : MonoBehaviour
{
    public static PlayerManager instance;
    public bool singleplayerTesting;
    [SerializeField] Transform[] startPositions;
    [SerializeField] GameObject playerPrefab;
    [SerializeField] GameObject gamepadDisconnectedUI;
    [Foldout("Camera")]
    [SerializeField] GameObject cameraParent;
    [SerializeField] CinemachineTargetGroup cameraGroup;
    [EndFoldout]
    [Foldout("Player Materials")]
    public Material redMaterial;
    public Material greenMaterial;
    public Material blueMaterial;
    public Material yellowMaterial;
    [EndFoldout]
    private List<CharacterControl.PlayerTypes> availablePlayerID;
    private List<PlayerInput> activePlayers;
    private bool playersSpawned;
    public static bool roundOver;
    public static int playerCount;
    int playerIndex = 0;
    public GameObject[] playerList;


    //static bool checkWinner = false;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        roundOver = false;
        instance = this;
    }
    private void Start()
    {
        //StartRound();
    }
    private void Update()
    {
        InputSystem.onDeviceChange += (device, change) =>
        {
            switch (change)
            {
                case InputDeviceChange.Disconnected:
                    if (!Application.isPlaying) return;
                    Debug.Log($"Device {device.name} has been disconnected.");
                    gamepadDisconnectedUI.SetActive(true);
                    Time.timeScale = 0f;
                    break;
                case InputDeviceChange.Reconnected:
                    Debug.Log($"Device {device.name} has been reconnected.");
                    gamepadDisconnectedUI.SetActive(false);
                    Time.timeScale = 1f;
                    break;
            }
        };
        if (SceneManager.GetActiveScene().name == "MainScene")
        {
            if (!playersSpawned)
            {
            }
            if (playersSpawned)
            {
            }
        }

        //osher
        /*
        if (checkWinner)
        {
            if (activePlayers!= null)
            {
                //Debug.Log(activePlayers.Count);
                for (int i = 0; i < activePlayers.Count; i++)
                {
                    if (activePlayers[i].gameObject.GetComponent<CharacterControl>().dead == false)
                    {
                        PickupManager.singleton.SetWinningPlayer(activePlayers[i].gameObject);
                    }
                }
            }
            checkWinner = false;
            roundOver = true;
        }
        */
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
        playersSpawned = true;
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
        //int playerIndex = 0;
        foreach (var device in InputSystem.devices)
        {
            if (device is Gamepad || device is Keyboard)
            {
                var playerInput = PlayerInput.Instantiate(playerPrefab, controlScheme: null, pairWithDevice: device);
                playerList[playerInput.playerIndex] = playerInput.gameObject;
                playerInput.transform.position = startPositions[playerIndex].position;
                CharacterControl characterControl = playerInput.GetComponent<CharacterControl>();
                if (availablePlayerID.Count > 0)
                {
                    CharacterControl.PlayerTypes assignedPlayerID = availablePlayerID[0];
                    characterControl.PlayerID = assignedPlayerID;
                    availablePlayerID.Remove(assignedPlayerID);
                    cameraGroup.AddMember(playerInput.gameObject.transform, 1f, 0f);
                    ChangePlayerMaterial(playerInput.gameObject, assignedPlayerID);
                    activePlayers.Add(playerInput);
                    Debug.Log(activePlayers);
                    playerIndex++;
                    playerCount++;
                }
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
            //checkWinner = true; //osher
            roundOver = true;
        }
    }
}
