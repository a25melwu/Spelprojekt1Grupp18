using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using TMPro;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class StartMenu : MonoBehaviour
{
    [Header("Start Scene")] 
    [SerializeField] private string startScene;
    
    [Header("Scene Options")]
    [SerializeField] private GameObject optionsPanel;
    private int toggleCount = 0;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if (optionsPanel != null)
        {
            optionsPanel.SetActive(false);
        }
        else
        {
            Debug.Log("Options panel object not assigned!");
        }
    }
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartGame()
    {
        string scene = this.startScene.ToString();
        if (string.IsNullOrEmpty(scene))
        {
            Debug.Log("Start scene is empty in inspector!");
            return;
        }
        
        SceneManager.LoadSceneAsync(scene, LoadSceneMode.Single);
    }

    public void OnToggleOptionsButton()
    {
        toggleCount++;
        Debug.Log($"toggled: {toggleCount}");
        ToggleOptionsMenu();
    }
    
    public void QuitGame()
    {
        Application.Quit();
    }

    private void ToggleOptionsMenu()
    {
        if (toggleCount % 2 == 1)
        {
            optionsPanel?.SetActive(true);
        }
        else
        {
            optionsPanel?.SetActive(false);
        }
    }
}
