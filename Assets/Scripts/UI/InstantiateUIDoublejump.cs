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
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // if we haven't filled in a specific position, set parentImageHolder to this GameObjects transform
        if (parentImageHolder == null)
        {
            parentImageHolder = transform;
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
