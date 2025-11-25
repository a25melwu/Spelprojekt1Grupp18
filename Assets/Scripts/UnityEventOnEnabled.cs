using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class UnityEventOnEnabled : MonoBehaviour
{
    public UnityEvent customEvent;

    private bool toggle = true;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    
    private void OnEnable()
    {
        if (toggle) customEvent.Invoke();
        toggle = false;

    }

    private void OnDisable()
    {
        toggle = true;
    }
}
