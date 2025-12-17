using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using TMPro;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject mainMenuCanvas;
    private GameObject timerPanel;
    private PlayerInput playerInput;
    private InstantiateUIDoublejump instantiateUIDoublejump;
    public bool IsMenuOpen { get; private set; }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if (mainMenuCanvas != null)
        {
            mainMenuCanvas.SetActive(false);
        }
        else
        {
            Debug.Log("Main menu canvas object not assigned!");
        }
        
        playerInput = FindFirstObjectByType<PlayerInput>();
        instantiateUIDoublejump = FindFirstObjectByType<InstantiateUIDoublejump>();
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            ToggleMenu();
        }
    }

    /*public void ChangeScene(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName)) return;
        
        SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
        
    }*/

    public void GoToStartScene(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName)) return;
        
        SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
        
        instantiateUIDoublejump.ClearUIDoubleJump(); //clears UI doublejumps when changing scene
        
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void OnToggleMenuButton() //for UI buttons
    {
        //Debug.Log("OnToggleMenu called");
        ToggleMenu();
    }

    private void ToggleMenu()
    {
        //Debug.Log("Togglemenu called");
        if (mainMenuCanvas == null) return; 
        //Debug.Log($"Before toggle: isMenuOpen = {IsMenuOpen}");
        
        IsMenuOpen = !IsMenuOpen;
        mainMenuCanvas?.SetActive(IsMenuOpen);
        //Debug.Log($"After toggle: isMenuOpen = {IsMenuOpen}");
        
        if (playerInput != null) //switches actionmap to block player input and jump sounds
        {
            playerInput.SwitchCurrentActionMap(IsMenuOpen ? "UI" : "Player");
            Debug.Log($"Switched to: {playerInput.currentActionMap?.name}");
        }
        
        Time.timeScale = IsMenuOpen ? 0f : 1f; //pauses game when menu is open, blocks squash and stretch script here
        
    }
    
}
