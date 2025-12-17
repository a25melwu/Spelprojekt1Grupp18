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

    [SerializeField] private Color available;
    [SerializeField] private Color usedUp;

    private int instanceCount = 0;
    public List<GameObject> feathers = new();

    public int usedUpFeathers = 0;
    
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
            feathers.Add(instance);
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


    private void SetAllFeathersToColor(Color colorToSetTo)
    {
        foreach (GameObject feather in feathers)
        {
            SetFeatherToColor(feather, colorToSetTo);
        }
    }

    //Called when we double jump, from the player
    public void SetFeatherToUsedUpColor()
    {
        if (feathers.Count <= 0) return;

        SetFeatherToColor(feathers[usedUpFeathers], usedUp);
        usedUpFeathers++;
    }

    private void SetFeatherToColor(GameObject feather, Color colorToSetTo)
    {
        feather.GetComponent<Image>().color = colorToSetTo;
    }

    //Called when the player touches the ground
    public void SetAllFeatherColorToAvailable()
    {
        SetAllFeathersToColor(available);
        usedUpFeathers = 0;
    }







}
