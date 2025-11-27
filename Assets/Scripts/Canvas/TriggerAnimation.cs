using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class TriggerAnimation : MonoBehaviour
{
    private Animator triggerAnimatorSwing;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        triggerAnimatorSwing = GetComponent<Animator>();
        triggerAnimatorSwing.SetBool("Swing", false);
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("e"))
        {
            triggerAnimatorSwing.SetBool("Swing", true);
        }
        if (Input.GetKeyUp("e"))
        {
            triggerAnimatorSwing.SetBool("Swing", false);
        }
    }
}
