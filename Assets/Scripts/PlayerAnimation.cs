using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class PlayerAnimation : MonoBehaviour
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
        if (actionAsset.FindAction("Move").IsPressed())
        {
            animator.SetBool("Walk", true);
        }
        else animator.SetBool("Walk", false);
        
        if (actionAsset.FindAction("Jump").IsPressed())
        {
            animator.SetBool("Jump", true);
        }
        else animator.SetBool("Jump", false);

        
        
    }
}