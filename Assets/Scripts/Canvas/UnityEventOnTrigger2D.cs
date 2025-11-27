using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UnityEventOnTrigger2D : MonoBehaviour
{
    //public string tagToActivate = "Player";

    public UnityEvent onTriggerEnter;
    public UnityEvent onTriggerExit;
    
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

    /*private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(tagToActivate))
        {
            onTriggerEnter.Invoke();
            Debug.Log("Unity Event Trigger (enter) activated on " + gameObject.name);
        }
    }*/
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        onTriggerEnter.Invoke();
        
    }

    /*private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(tagToActivate))
        {
            onTriggerExit.Invoke();
            Debug.Log("Unity Event Trigger (exit) activated on " + gameObject.name);
        }
    }*/
    
    private void OnTriggerExit2D(Collider2D other)
    {
        onTriggerExit.Invoke();
        
    }
}
