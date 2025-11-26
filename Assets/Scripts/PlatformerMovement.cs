using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// This script automatically adds a Rigidbody2D, CapsuleCollider2D and CircleCollider2D component in the inspector.
// The Rigidbody2D component should (probably) have some constraints: Freeze Rotation Z, and added mass e.g. '5'
// The Circle Collider 2D should be set to "is trigger", resized and moved to a proper position for ground check.
// The following componenets are needed for headbutt check: BoxCollider2D
// The following components are also needed: Player Input
// Gravity for the project is set in Unity at Edit -> Project Settings -> Physics2D-> Gravity Y

[RequireComponent(typeof(Rigidbody2D), typeof(CircleCollider2D),
    typeof(CapsuleCollider2D))]
class PlatformerMovement : MonoBehaviour
{
    [SerializeField] private float maxSpeed;
    [SerializeField] private float jumpForce;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [Tooltip("Started as 1")]
    [SerializeField] private float jumpInputDeceleration;
    [Tooltip("Started as 5")]
    [SerializeField] private float jumpReleasedDeceleration;
    
    public bool controlEnabled { get; set; } = true;

    private Vector2 moveInput;
    private Rigidbody2D rb;
    
    //platformer specific variables
    private CircleCollider2D groundCheckCollider;
    private LayerMask groundLayer = ~0; //~0 is referring to EVERY layer. Serialize the variable and assign the layer of your choice if you want a specific layer.

    private Vector2 velocity;
    private bool jumpInput;
    private bool jumpReleased;
    //private bool duckInput; //added for duck
    //private bool duckReleased; //added for duck
    private bool wasGrounded;
    private bool isGrounded;
    //private Animator animator;
    
    public InputActionAsset actionAsset;
    //private AudioPlayRandom jumpAudioPlay; //jumping sound
    
    private BoxCollider2D headCheckCollider; //for checking if head is colliding
    private bool isHeadbutt;

    private bool faceRight = true;
    private bool faceLeft;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        groundCheckCollider = GetComponent<CircleCollider2D>();
        groundCheckCollider.isTrigger = true;

        headCheckCollider = GetComponent<BoxCollider2D>(); //for checking if head is colliding
        headCheckCollider.isTrigger = true;
        
        //Set gravity scale to 0 so player wont "fall"
        rb.gravityScale = 0;
        
        //animator = GetComponent<Animator>();
        
        //jumpAudioPlay = GetComponentInChildren<AudioPlayRandom>(); //jumping sound
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //JumpSound(); //call jumping sound function
        if (actionAsset.FindAction("Left").IsPressed() && isGrounded)
        {
            faceLeft = true;
            faceRight = false;
        }
        else if (actionAsset.FindAction("Right").IsPressed() && isGrounded)
        {
            faceRight = true;
            faceLeft = false;
        }
        
        if (faceLeft && jumpInput)
        {
            moveInput = Vector2.left.normalized;
        }
        if (faceRight && jumpInput)
        {
            moveInput = Vector2.right.normalized;
        }
        
        
        velocity = TranslateInputToVelocity(moveInput);
        if (jumpInput && wasGrounded)
        {
            //maxSpeed = 2.5f;
            velocity.y = jumpForce;
            jumpInput = false;
            //duckInput = false;
            
        }
        if (isHeadbutt == true) //added bounce if head is colliding
        {
            velocity.y = -maxSpeed;
        }

        //check if character gained contact with ground this frame
        if (wasGrounded == false && isGrounded == true)
        {
            jumpReleased = false;
            //maxSpeed = 5f;
            //duckReleased = true;
            
            //has landed, play landing sound and trigger landing animation
            moveInput = Vector2.zero;
        }
        
        wasGrounded = isGrounded;
        
        //flip sprite according to direction (if a sprite renderer has been assigned)

        /*if (spriteRenderer)
        {
            if (moveInput.x > 0.01f)
            {
                spriteRenderer.flipX = true;
            }
            else if (moveInput.x < -0.01f)
            {
                spriteRenderer.flipX = false;
            }
        }*/
        
        if (spriteRenderer)
        {
            if (faceRight)
            {
                spriteRenderer.flipX = true;
            }
            else if (faceLeft)
            {
                spriteRenderer.flipX = false;
            }
        }
        
    }

    private void FixedUpdate()
    {
        isGrounded = IsGrounded();
        isHeadbutt = IsHeadbutt();
        ApplyGravity();
        rb.linearVelocity = velocity;
        
        
        //write movement animation code here. Suggestion: send your current velocity into the Animator for both the x- and y-axis
        /*if (MathF.Abs(rb.linearVelocity.x) > 0.01f && isGrounded)
        {
            animator.SetBool("Walk", true);
        }
        else animator.SetBool("Walk", false);*/

        /*if (rb.linearVelocity.y > 0.01f && isGrounded == false)
        {
            animator.SetBool("Jump", true);
        }
        else animator.SetBool("Jump", false);*/
        
        /*if (rb.linearVelocity.y < 0f && isGrounded && duckInput)
        {
            animator.SetBool("Duck", true);
        }
        else animator.SetBool("Duck", false);*/
        
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
            velocity.y = -1f;
        }
        
        //updates fall speed with gravity if object isnt grounded
        else
        {
            //is jumping
            if (velocity.y > 0)
            {
                float deceleration = jumpInputDeceleration; //started as 1, local variable
                if (jumpReleased) //shorter jump height when releasing the jump
                {
                    deceleration = jumpReleasedDeceleration; //started as 5, local variable
                }
                //add gravity multiplier here
                velocity.y += Physics2D.gravity.y * deceleration * Time.deltaTime;
            }
           
            //is falling
            else
            {
                //add gravity multiplier here
                velocity.y += Physics2D.gravity.y * Time.deltaTime;
            }
        }
    }

    Vector2 TranslateInputToVelocity(Vector2 input)
    {
        return new Vector2(input.x * maxSpeed, velocity.y);
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

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started && controlEnabled)
        {
            jumpInput = true;
            jumpReleased = false;
        }

        if (context.canceled && controlEnabled)
        {
            jumpReleased = true;
            jumpInput = false;
            
        }
    }

    /*private void JumpSound()
    {
        if (jumpInput && wasGrounded)
        {
            
            jumpAudioPlay.PlayAudio();
            
            
        }
    }*/
    
    /*public void OnDuck(InputAction.CallbackContext context) //added for duck
    {
        if (context.started && controlEnabled)
        {
            duckInput = true;
            duckReleased = false;
            
        }

        if (context.canceled && controlEnabled)
        {
            
            duckInput = false;
            duckReleased = true;
        }
        
    }*/
    
    
}
