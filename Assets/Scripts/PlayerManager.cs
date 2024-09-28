using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using VInspector;
using Cinemachine;
using UnityEngine.SceneManagement;
using TMPro;
public class PlayerManager : MonoBehaviour
{
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
    [Foldout("Winner UI")]
    [SerializeField] GameObject winnerBG;
    [SerializeField] TextMeshProUGUI winnerText;
    [EndFoldout]
    private List<CharacterControl.PlayerTypes> availablePlayerID;
    private List<PlayerInput> activePlayers;
    private bool playersSpawned;
    private bool winnerAnnounced;
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        //activePlayers = new List<PlayerInput>();
        winnerAnnounced = false;
    }
    private void Start()
    {
        availablePlayerID = new List<CharacterControl.PlayerTypes>
        {
            CharacterControl.PlayerTypes.Red,
            CharacterControl.PlayerTypes.Green,
            CharacterControl.PlayerTypes.Blue,
            CharacterControl.PlayerTypes.Yellow
        };
    }
    private void Update()
    {
        if (Application.isPlaying)
        {
            InputSystem.onDeviceChange += (device, change) =>
            {
                switch (change)
                {
                    case InputDeviceChange.Disconnected:
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
        }
        if (SceneManager.GetActiveScene().name == "MainScene" && !playersSpawned)
        {
            activePlayers = new List<PlayerInput>();
            AssignPlayers();
            playersSpawned = true;
            cameraParent.SetActive(true);
        }
        CheckRemainingPlayers();
    }
    private void AssignPlayers()
    {
        int playerIndex = 0;
        foreach (var device in InputSystem.devices)
        {
            if (device is Gamepad || device is Keyboard)
            {
                var playerInput = PlayerInput.Instantiate(playerPrefab, controlScheme: null, pairWithDevice: device);
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
                    playerIndex++;
                }
            }
        }
    }
    private void ChangePlayerMaterial(GameObject player, CharacterControl.PlayerTypes playerType)
    {
        Transform bodyGFXTransform = player.transform.Find("Body/BodyGFX");
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
    private void CheckRemainingPlayers()
    {
        activePlayers.RemoveAll(player => player == null);
        if (activePlayers.Count == 1 && !winnerAnnounced)
        {
            PlayerInput remainingPlayer = activePlayers[0];
            CharacterControl characterControl = remainingPlayer.GetComponent<CharacterControl>();
            winnerText.text = $"Player {characterControl.PlayerID} Wins!";
            winnerText.gameObject.SetActive(true);
            winnerBG.SetActive(true);
            //Debug.Log($"Player {characterControl.PlayerID} Wins!");
            winnerAnnounced = true;
        }
    }
    [Button]
    public void LoadGame()
    {
        SceneManager.LoadScene("MainScene");
    }
}
