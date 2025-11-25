using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class PlayerTriggeredAnimation : MonoBehaviour
{
    public InputActionAsset actionAsset;
    public Animator animator;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (actionAsset.FindAction("Interact").IsPressed())
        {
            animator.SetBool("Swing", true);
        }
        else animator.SetBool("Swing", false);
        
        
    }
}
