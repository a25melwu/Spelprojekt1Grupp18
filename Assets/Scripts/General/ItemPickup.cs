using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class ItemPickup : MonoBehaviour
{
    [Header("Pickup Settings")]
    public string tagToActivate = "Player";

    [Tooltip("Should this pickup give double jump ability?")]
    public bool giveDoubleJump = false;
    
    [Header("Events")]
    public UnityEvent onTriggerEnter;
    public UnityEvent onTriggerExit;

    private bool triggeredThisFrame = false; //flag to limit collisions to 1 per frame
    
    private PlayerSFX playerSFX;

    [SerializeField] private float pickupID; 


    private void Awake()
    {
        if (GetComponent<Collider2D>() == null)
        {
            Debug.Log($"{gameObject} is missing a collider2D");
        }

        pickupID = transform.position.x;
    }

    private void Start()
    {
        //If we have collected this feather already, destroy it when we reload the scene
        if (SaveManager.instance != null)
        {
            if (SaveManager.instance.collectedFeathersID.Contains(pickupID))
            {
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(tagToActivate) && !triggeredThisFrame)
        {
            playerSFX = other.GetComponentInChildren<PlayerSFX>();
            playerSFX?.PlayPickupSound();

            if (giveDoubleJump)
            {
                //get PlatformerMovement from the object that enters the trigger
                PlatformerMovement playerMovement = other.GetComponent<PlatformerMovement>();
            
                //Give doublejump to player
                if (playerMovement != null)
                {
                    playerMovement.AddDoubleJump(1); //add a doublejump to player
                }
            }

            if (SaveManager.instance != null)
            {
                SaveManager.instance.collectedFeathersID.Add(pickupID);
            }


            gameObject.SetActive(false); //sets object to inactive
            //Destroy(gameObject);
           
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
