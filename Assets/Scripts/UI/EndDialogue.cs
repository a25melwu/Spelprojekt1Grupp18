using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class EndDialogue : MonoBehaviour
{
    private PlayerInput playerInput;
    private MainMenu mainMenu;
    
    //used together with e.g. UnityEventOnTrigger, UI-button-events to decide when a scene should be changed or the game should be closed
    [Tooltip("Enter next scene or leave empty")]
    public string scene;

    void Awake()
    {
        playerInput = FindFirstObjectByType<PlayerInput>();
        mainMenu = FindFirstObjectByType<MainMenu>();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SwitchToUI()
    {
        if (playerInput != null)
        {
            playerInput.SwitchCurrentActionMap("UI");
            Debug.Log($"End dialogue - switched to {playerInput.currentActionMap?.name}");
        }
    }

    public void OpenFinishMenu()
    {
        if (mainMenu != null)
        {
            mainMenu.OnOpenFinishMenu();
        }
        
    }
    
    public void ChangeScene()
    {
        string scene = this.scene.ToString();
        if (string.IsNullOrEmpty(scene)) return;
        
        SceneManager.LoadSceneAsync(scene, LoadSceneMode.Single);
        
    }
}
