using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEditor.Rendering;
using UnityEngine.Rendering.UI;
using UnityEngine.SceneManagement;

public class TimerManager : MonoBehaviour
{
    private float currentTime = 0f;
    private bool isRunning = true;
    private string lastSceneName = ""; //tracks the previous scene for reset logic
    
    //FIXED: required as global access point to the timemanager instance
    //otherwise other classes cant find it when using findfirstobjectbytype when it is under dontdestroyonload
    public static TimerManager Instance { get; private set; } 
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this; //saves reference to this first instance
        }
        else
        {
            //Debug.Log("TimerManager: Duplicate timermanager destroyed");
            Destroy(gameObject);
        }
    }
    void Start()
    {
        lastSceneName = SceneManager.GetActiveScene().name;
    }
    public void Update()
    {
        if (!isRunning) return;
        currentTime += Time.deltaTime;
    }
    
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded; //subscribe to scene change events
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded; //unsubscribe to scene change events
    }

    void OnDestroy() //called when gameobject is destroyed. cleans up static reference
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
    
    public string GetFormattedTime()
    {
        int minutes = Mathf.FloorToInt(currentTime / 60f);
        int seconds = Mathf.FloorToInt(currentTime % 60f);

        return $"{minutes:00}:{seconds:00}"; //always two digits for both minutes and seconds
    }
    
    public void StartTimer()
    {
        isRunning = true;
    }

    public void StopTimer()
    {
        isRunning = false;
    }
    
    public void ResetTimer()
    {
        currentTime = 0f;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        string currentSceneName = scene.name;

        if (currentSceneName != lastSceneName) //reset timer when scene is changed
        {
            ResetTimer();
        }
        
        lastSceneName = currentSceneName;
        StartTimer(); //ensures timer is running in the new scene

    }
    
}
