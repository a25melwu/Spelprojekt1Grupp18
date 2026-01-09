using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class HeightManager : MonoBehaviour
{
    [SerializeField] private string tagToFindY = "Finish";
    [SerializeField] private string playerTag = "Player";
    private float currentHeight;
    private float highestPoint;
    private float playerStartY = 0f;
    private float metersPerUnit = 0.25f; // 0.15*5.78 = 0.9m per jump
    private Transform playerTransform;

    public float DistanceToTop { get; private set; }

    public float DistanceToTopMeters
    {
        get {return DistanceToTop * metersPerUnit;}
    }
    
    //FIXED: required as global access point to the heightmanager instance
    //otherwise other classes cant find it when using findfirstobjectbytype when it is under dontdestroyonload
    public static HeightManager Instance { get; private set; } 
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this; //saves reference to this first instance
        }
        else
        {
            Destroy(gameObject);
        }
        
        FindInitialHighestPoint();
        GetPlayerStartPositionY();
        UpdatePlayerHeight();
    }
    public void Update()
    {
        
        if (playerTransform != null && (Time.frameCount % 60 == 0)) //log once per second at 60 fps
        {
            UpdatePlayerHeight();
            Debug.Log($"Update: playertransform {playerTransform.position.y}");
        }
    }

    void OnDestroy() //called when gameobject is destroyed. cleans up static reference
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    private void FindInitialHighestPoint()
    {
        highestPoint = 0f;
        
        GameObject[] objectsWithTag = GameObject.FindGameObjectsWithTag(tagToFindY);
        if (objectsWithTag.Length == 0)
        {
            Debug.Log("No objects with tag Finish found");
        }
        
        foreach (GameObject objects in objectsWithTag)
        {
            float height = GetObjectHeight(objects);
            Debug.Log($"Height:  {height}, object:  {objects.name}");
            
            if (height > highestPoint)
            {   
                
                highestPoint = height;
            }
        }
        Debug.Log($"FindInitialHighestPoint: {highestPoint}");
    }

    private float GetObjectHeight(GameObject objects)
    {
        
        Collider2D collider2D = objects.GetComponent<Collider2D>();
        if (collider2D != null)
        {
            return collider2D.bounds.max.y;
        }
        
        return objects.transform.position.y;
    }

    private void GetPlayerStartPositionY()
    {
        GameObject player = GameObject.FindGameObjectWithTag(playerTag);
        playerTransform = player.transform.GetChild(0);
        playerStartY = playerTransform.position.y;
    }

    private void UpdatePlayerHeight()
    {
        float currentPlayerY =  playerTransform.position.y;
        
        DistanceToTop = Mathf.Max(0f, highestPoint - currentPlayerY); 
        
        
    }
    
    
}
