using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using TMPro;
using UnityEngine.UI;

public class ApplicationHandler : MonoBehaviour
{
    //used together with e.g. UnityEventOnTrigger, UI-button-events to decide when a scene should be changed or the game should be closed
    public string scene;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeScene()
    {
        //string scene = this.scene.ToString();
        SceneManager.LoadSceneAsync(scene, LoadSceneMode.Single);
        
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
