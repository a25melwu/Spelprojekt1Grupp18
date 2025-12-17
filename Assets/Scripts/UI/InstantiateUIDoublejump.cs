using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InstantiateUIDoublejump : MonoBehaviour
{
    [SerializeField] private GameObject imagePrefab;
    [SerializeField] private Transform parentImageHolder;

    private int instanceCount = 0;
    
    public static InstantiateUIDoublejump Instance { get; private set; } 
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // if we haven't filled in a specific position, set parentImageHolder to this GameObjects transform
        if (parentImageHolder == null)
        {
            parentImageHolder = transform;
        }
        if (Instance == null)
        {
            Instance = this; //saves reference to this first instance
        }
        else
        {
            Destroy(gameObject);
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnDestroy() //called when gameobject is destroyed. cleans up static reference
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
    
    public void AddUIDoubleJump(int value)
    {
        if (imagePrefab == null || value <= 0) return;

        for (int i = 0; i < value; i++)
        {
            GameObject instance = Instantiate(imagePrefab, parentImageHolder);
            instanceCount += value;
        }
        
    }

    public void ClearUIDoubleJump()
    {
        foreach (Transform child in parentImageHolder)
        {
            Destroy(child.gameObject);
        }
        instanceCount = 0;
    }

    public int GetUIDoubleJump()
    {
        return instanceCount;
    }
}
