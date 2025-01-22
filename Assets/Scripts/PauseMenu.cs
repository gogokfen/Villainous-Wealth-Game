using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class PauseMenu : MonoBehaviour
{
    public static PauseMenu instance;
    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameObject resumeButton;
    [SerializeField] Sprite resumeSprite;
    public bool paused;
    private static PlayerInput pausingPlayer;
    private void Awake()
    {
        instance = this;
    }
    public void SubToPause()
    {
        PlayerInput[] playerInputs = FindObjectsOfType<PlayerInput>();
        foreach (PlayerInput player in playerInputs)
        {
            player.actions["UI/Pause"].performed += ctx => OnPause(player);
            Debug.Log("shalom");
        }
    }
    public void OnPause(PlayerInput player)
    {
        Debug.Log("ahalan");
        MultiplayerEventSystem playerEventSystem = player.GetComponentInChildren<MultiplayerEventSystem>();
        {
            if (!paused)
            {
                playerEventSystem.SetSelectedGameObject(null);
                playerEventSystem.SetSelectedGameObject(resumeButton);
                Button button = resumeButton.GetComponent<Button>();
                button.image.sprite = resumeSprite;
                paused = true;
                pausingPlayer = player;
                pauseMenu.SetActive(true);
                Time.timeScale = 0f;
            }
            else if (pausingPlayer == player)
            {
                playerEventSystem.SetSelectedGameObject(null);
                pausingPlayer = null;
                paused = false;
                pauseMenu.SetActive(false);
                Time.timeScale = 1f;
            }
        }
    }
    public void ResumeButton()
    {
        pausingPlayer = null;
        paused = false;
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
    }
    public void MainMenuButton()
    {
        SceneManager.LoadScene("MainMenu");
    }
    public void QuitButton()
    {
        Application.Quit();
    }
}
