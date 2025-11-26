using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

// The Rigidbody2D component should (probably) have some constraints: Freeze Rotation Z, and added mass e.g. '5' /REMOVED
// The Circle Collider 2D should be set to "is trigger", resized and moved to a proper position for ground check.
// The following componenets are needed for headbutt check: BoxCollider2D
// The following components are also needed: Player Input
// Gravity for the project is set in Unity at Edit -> Project Settings -> Physics2D-> Gravity Y

//[RequireComponent(typeof(Rigidbody2D), typeof(CircleCollider2D), typeof(CapsuleCollider2D))]
class PlatformerMovement : MonoBehaviour
{
    [SerializeField] private BoxCollider2D groundCheckCollider;
    [SerializeField] private BoxCollider2D headCheckCollider;
    [SerializeField] private float maxSpeed;
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
    //private AudioPlayRandom jumpAudioPlay; //jumping sound

    private bool isHeadbutt; //for checking if head is colliding

    private bool faceRight = true;
    private bool faceLeft;

    private float jumpChargeTime = 0f;
    private float maxChargeTime = 1.0f;
    private float minJumpForce = 1f;
    private float maxJumpForce = 7f;
    
    private float landingSlowdown = 0.7f; // 1 - harder landing, 0.1 softer landing
    private float jumpGravityScale = 0.6f; // 1 - heavier jump, 0.1 floatier jump
    private float fallGravityScale = 0.8f; // 2 - fast falling, 0.5 slower falling
    
    //[SerializeField] private bool canDoubleJump;
    //private int maxJumps = 2;
    //private int currentJumps = 0;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        groundCheckCollider.isTrigger = true;
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
        
        if (jumpInput && wasGrounded)
        {
            //add visual feedback here 
            
            jumpChargeTime += Time.deltaTime;
            
            if (jumpChargeTime >= maxChargeTime)
            {
                velocity.y = maxJumpForce;
                if (faceLeft) moveInput = Vector2.left.normalized;
                else if (faceRight) moveInput = Vector2.right.normalized;
                
                jumpInput = false;
            }
            
        }
        
        velocity = TranslateInputToVelocity(moveInput);
        
        //Debug.Log($"{jumpInput} , {canDoubleJump}, {currentJumps}");
        
        /*if (jumpInput && canDoubleJump && currentJumps < maxJumps)
        {
            velocity.y = jumpForce;
            jumpInput = false;
            currentJumps++;
            Debug.Log("should doublejump");
        }*/
        
        
        if (isHeadbutt == true) //added bounce if head is colliding
        {
            velocity.y = -maxSpeed;
        }

        //check if character gained contact with ground this frame
        if (wasGrounded == false && isGrounded == true)
        {
            moveInput = Vector2.zero;
            
            //canDoubleJump = false;
            //currentJumps = 0;
            
            //has landed, play landing sound and trigger landing animation
            
        }
        //Debug.Log(currentJumps);
        wasGrounded = isGrounded;
        
        //flip sprite according to direction (if a sprite renderer has been assigned)
        if (spriteRenderer)
        {
            if (faceRight)
            {
                spriteRenderer.flipX = false;
            }
            else if (faceLeft)
            {
                spriteRenderer.flipX = true;
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
        if (context.started && controlEnabled && wasGrounded)
        {
            jumpChargeTime = 0f;
            jumpInput = true;
            
        }

        if (context.canceled && jumpInput)
        {
            //Debug.Log($"OnJump - moveInput {moveInput}, velocity before: {velocity}");
            float charge = Mathf.Clamp01(jumpChargeTime / maxChargeTime); 
            velocity.y = Mathf.Lerp(minJumpForce, maxJumpForce, charge);
            
            if (faceLeft) moveInput = Vector2.left.normalized;
            else if (faceRight) moveInput = Vector2.right.normalized;
            
            //Debug.Log($"OnJump - velocity after : {velocity}");
            
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
    
    
}
