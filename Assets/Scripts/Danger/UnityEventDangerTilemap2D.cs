using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UnityEventDangerTilemap2D : MonoBehaviour
{
    public string tagToActivate = "Player";

    public UnityEvent onTriggerEnter;
    public UnityEvent onTriggerExit;

    private bool triggeredThisFrame = false; //flag to limit collisions to 1 per frame
    private PlayerSFX playerSFX;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    private void Awake()
    {
        if (GetComponent<Collider2D>() == null)
        {
            Debug.Log($"{gameObject} is missing a collider2D");
        }
        
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(tagToActivate) && !triggeredThisFrame)
        {
            playerSFX = other.GetComponentInChildren<PlayerSFX>();
            playerSFX?.PlayHurtSound();
            
            onTriggerEnter.Invoke();
            triggeredThisFrame = true;
            //Debug.Log("Unity Event Trigger (enter) activated on " + gameObject.name);
        }
    }
    
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(tagToActivate))
        {
            onTriggerExit.Invoke();
            //Debug.Log("Unity Event Trigger (exit) activated on " + gameObject.name);
        }
    }

    private void LateUpdate() 
    {
        triggeredThisFrame = false; //resets flag every frame
    }
}
