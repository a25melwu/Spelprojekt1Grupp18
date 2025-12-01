using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

// The Rigidbody2D component should (probably) have some constraints: Freeze Rotation Z, and added mass e.g. '5' 
// The BoxCollider 2D should be set to "is trigger", resized and moved to a proper position for ground check.
// The following componenets are needed for headbutt check: BoxCollider2D
// The following components are also needed: Player Input
// Gravity for the project is set in Unity at Edit -> Project Settings -> Physics2D-> Gravity Y

//[RequireComponent(typeof(Rigidbody2D), typeof(CircleCollider2D), typeof(CapsuleCollider2D))]
class PlatformerMovement : MonoBehaviour
{
    [SerializeField] private BoxCollider2D groundCheckCollider;
    [SerializeField] private BoxCollider2D headCheckCollider;
    [SerializeField] private SpriteRenderer spriteRenderer;
    public bool controlEnabled { get; set; } = true;

    private Vector2 moveInput;
    private Rigidbody2D rb;
    
    //platformer specific variables
    
    private LayerMask groundLayer = ~0; //~0 is referring to EVERY layer. Serialize the variable and assign the layer of your choice if you want a specific layer.

    private Vector2 velocity;
    private bool jumpInput = false;
    private bool wasGrounded;
    private bool isGrounded;
    //private Animator animator;
    
    public InputActionAsset actionAsset;
    private PlayerSFX playerSFX; //jumping sounds
    private SquahAndStretch squashAndStretchManager;

    private bool isHeadbutt; //for checking if head is colliding

    private bool faceRight = true;
    
    [Header("X Movement")]
    [Tooltip("maxSpeed is a constant for x-axis")]
    [SerializeField] private float xMaxSpeed;

    [Tooltip("How the speed changes when charging Jump in air")]
    [SerializeField] private float xSpeedAirJumpSlowMultiplier = 1f;

    [Header("Y Movement")]
    [Tooltip("Jump force adjusts in y-axis")]
    [SerializeField] private float maxJumpForce = 7f;

    [Header("Fall Movement")]
    [Tooltip("1 - harder landing, 0.1 - softer landing")]
    [SerializeField] private float landingSlowdown = 0.7f; // 1 - harder landing, 0.1 softer landing
    [Tooltip("1 - heavier jump, 0.1 - floatier jump")]
    [SerializeField] private float jumpGravityScale = 0.6f; // 1 - heavier jump, 0.1 floatier jump

    [Tooltip("2 - fast falling, 0.5 - slower falling")]
    [SerializeField] private float fallGravityScale = 0.8f; // 2 - fast falling, 0.5 slower falling

    [Tooltip("How the fall gravity changes when charging Jump in air")]
    [SerializeField] private float airFallGravityScaleSlowMultiplier = 1;

    private float jumpChargeTime = 0f;
    private float maxChargeTime = 1.0f;
    private float minJumpForce = 1f;

    private float startFallGravityScale;
    private float startXMaxSpeed;

    private int maxJumps = 1;
    private int currentJumps = 0;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        groundCheckCollider.isTrigger = true;
        headCheckCollider.isTrigger = true;
        
        //Set gravity scale to 0 so player wont "fall"
        rb.gravityScale = 0;
        
        playerSFX = GetComponentInChildren<PlayerSFX>(); //soundgroup playerSFX
        squashAndStretchManager = GetComponentInParent<SquahAndStretch>();

        startFallGravityScale = fallGravityScale;
        startXMaxSpeed = xMaxSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        if (isGrounded)
        {
            if (actionAsset.FindAction("Right").IsPressed())
            {
                faceRight = true;
            }

            if (actionAsset.FindAction("Left").IsPressed())
            {
                faceRight = false;
            }
        }
        
        if (jumpInput && HasJumpsLeft()) 
        {
            //add visual feedback here 
            
            jumpChargeTime += Time.deltaTime;

            //The player slows when we double jump
            fallGravityScale = startFallGravityScale * airFallGravityScaleSlowMultiplier;
            xMaxSpeed = startXMaxSpeed * xSpeedAirJumpSlowMultiplier;

            if (jumpChargeTime >= maxChargeTime) //auto-releases at max
            {
                Jump(maxJumpForce);
            }
            
        }

        velocity = TranslateInputToVelocity(moveInput);
        
        
        if (isHeadbutt == true) //added bounce if head is colliding
        {
            velocity.y = -xMaxSpeed;
        }

        
        if (wasGrounded == false && isGrounded == true) //check if character gained contact with ground this frame
        {
            moveInput = Vector2.zero;
            
            currentJumps = 0; //reset jump counter when landing
            
            //has landed, play landing sound and trigger landing animation
            playerSFX?.PlayLandingSound();
        }
       
        wasGrounded = isGrounded;
        
        
        if (spriteRenderer) //flip sprite according to direction (if a sprite renderer has been assigned)
        {
            switch (faceRight)
            {
                case true:
                    spriteRenderer.flipX = false;
                    break;
                case false:
                    spriteRenderer.flipX = true;
                    break;
            }
        }
        
    }

    private void Jump(float jumpForce)
    {
        if (isGrounded && currentJumps == 0) playerSFX?.PlayJumpSound(); //play jump sound here
        
        //Set movement to player
        velocity.y = jumpForce;

        //Remove squash animation
        if (squashAndStretchManager != null)
            squashAndStretchManager.SetSquashState(false);

        //Change moveInput direction
        switch (faceRight)
        {
            case true:
                moveInput = Vector2.right.normalized;
                break;
            case false:
                moveInput = Vector2.left.normalized;
                break;
        }
        
        jumpInput = false;
        currentJumps++;

        fallGravityScale = startFallGravityScale;
        xMaxSpeed = startXMaxSpeed;

    }


    private void FixedUpdate()
    {
        isGrounded = IsGrounded();
        isHeadbutt = IsHeadbutt();
        ApplyGravity();
        rb.linearVelocity = velocity;
    }

    public bool HasJumpsLeft()
    {
        if (currentJumps < maxJumps)
            return true;
        else
        {
            return false;
        }
    }

    private bool IsGrounded()
    {
        if (groundCheckCollider.IsTouchingLayers(groundLayer))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private bool IsHeadbutt() //added to check if head is colliding
    {
        if (headCheckCollider.IsTouchingLayers(groundLayer))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void ApplyGravity()
    {
        if (isGrounded && velocity.y < 0.0f)
        {
            velocity.y = -1f * landingSlowdown; //soft landing
        }
        
        //updates fall speed with gravity if object isnt grounded
        else
        {
            float gravityScale;
            
            if (velocity.y > 0)  //is jumping
            {
                gravityScale = jumpGravityScale; //floaty ascent
               
            }
           
            else //is falling
            {
                gravityScale = fallGravityScale; //slow descent
            }
            velocity.y += Physics2D.gravity.y * gravityScale * Time.deltaTime;
        }
    }

    Vector2 TranslateInputToVelocity(Vector2 input)
    {
        return new Vector2(input.x * xMaxSpeed, velocity.y);
    }

    /*public void OnMove(InputAction.CallbackContext context)
    {
        if (controlEnabled)
        {
            moveInput = context.ReadValue<Vector2>().normalized;
        }
        else
        {
            moveInput = Vector2.zero;
        }
    }*/

    //When space is pressed
    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started && controlEnabled) 
        {
            if (wasGrounded || (!wasGrounded && currentJumps < maxJumps)) //jump either from ground OR (in air AND have jumps left)
            {
                jumpChargeTime = 0f;
                jumpInput = true;
            }
        }

        //When space is released when you have started to jump
        if (context.canceled && jumpInput && currentJumps < maxJumps)
        {
            //Debug.Log($"OnJump - moveInput {moveInput}, velocity before: {velocity}");
            float charge = Mathf.Clamp01(jumpChargeTime / maxChargeTime); 
            
            float jumpForce = Mathf.Lerp(minJumpForce, maxJumpForce, charge);

            Jump(jumpForce);
            
            if (currentJumps > 1) playerSFX?.PlayDoubleJumpSound(); //play doublejump sound here
            
        }
    }
    
    public void AddDoubleJump(int add)
    {
        maxJumps += add;
        Debug.Log($"Current Doublejumps: {maxJumps-1}");
    }
    
    public void DisableDoubleJump()
    {
        maxJumps = 1;
        Debug.Log("Double Jump disabled");
    }
}
