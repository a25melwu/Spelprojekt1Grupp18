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
    
    [Header("Scene Panels")]
    [SerializeField] private GameObject optionsPanel;
    [SerializeField] private GameObject creditsPanel;
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

        if (creditsPanel != null)
        {
            creditsPanel.SetActive(true);
        }
        else
        {
            Debug.Log("Credits panel object not assigned!");
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
        toggleCount = 0;
    }

    public void OnToggleOptionsButton()
    {
        toggleCount++;
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
            creditsPanel?.SetActive(false);
            optionsPanel?.SetActive(true);
            
        }
        else
        {
            optionsPanel?.SetActive(false);
            creditsPanel?.SetActive(true);
        }
    }
}
