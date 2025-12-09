using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

// Gravity for the project is set in Unity at Edit -> Project Settings -> Physics2D-> Gravity Y

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
    
    public InputActionAsset actionAsset;
    private PlayerSFX playerSFX; //jumping sounds
    private SquahAndStretch squashAndStretchManager;
    private SaveManager saveManager;

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
    public float maxChargeTime = 1.0f;
    private float minJumpForce = 1f;
    private float minChargeTime = 0.1f;

    private float startFallGravityScale;
    private float startXMaxSpeed;

    private int maxJumps = 1;
    private int currentJumps = 0;


    [Header("Raycast Objects")]
    [SerializeField] private GameObject rightWallChecker;
    [SerializeField] private GameObject leftWallChecker;
    [SerializeField] private GameObject raycastCheck1;
    [SerializeField] private GameObject raycastCheck2;

    [SerializeField] private float groundCheckDistance = 0.01f;

    private float distanceToGround;
    private bool clickedJump;
    private bool jumpedHighEnough = false;





    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        //groundCheckCollider.isTrigger = true;
        headCheckCollider.isTrigger = true;
        
        //Set gravity scale to 0 so player wont "fall"
        rb.gravityScale = 0;
        
        playerSFX = GetComponentInChildren<PlayerSFX>(); //soundgroup playerSFX
        squashAndStretchManager = GetComponentInParent<SquahAndStretch>();

        startFallGravityScale = fallGravityScale;
        startXMaxSpeed = xMaxSpeed;

        //If we have gotten a double jump and then respawned, we get the double jumps back
        saveManager = FindFirstObjectByType<SaveManager>();
        if (saveManager != null)
            SetMaxJumpAmount(saveManager.GetDoubleJumpsPlayerShouldStartWith() + 1); //We should start with 0 double jumps, but we want to be able to jump once
    }

    private void FixedUpdate()
    {
        isHeadbutt = IsHeadbutt();
        ApplyGravity();
        rb.linearVelocity = velocity;
    }

    // Update is called once per frame
    void Update()
    {
        if (IsGrounded())
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
            jumpChargeTime += Time.deltaTime;

            //The player slows when we double jump
            fallGravityScale = startFallGravityScale * airFallGravityScaleSlowMultiplier;
            xMaxSpeed = startXMaxSpeed * xSpeedAirJumpSlowMultiplier;

            /*if (jumpChargeTime >= maxChargeTime) //auto-releases at max - DO NOT REMOVE
            {
                Jump(maxJumpForce);
                if (currentJumps > 1) playerSFX?.PlayDoubleJumpSound(); //play doublejump for auto-release here
            }*/
        }

        velocity = TranslateXInputToVelocity(moveInput);
        
        if (isHeadbutt == true) //added bounce if head is colliding
        {
            velocity.y = -xMaxSpeed;
        }
        
        //If we jumped high enough and we then is grounded, we Land
        if (JumpedHighEnough() && IsGrounded())
        {
            Land();
        }

        //If we didn't jump high enough, but we did click jump and velocity is 0, we still trigger Land
        if(!JumpedHighEnough() && clickedJump && IsGrounded() && velocity.y <= 0f)
        {
            Land();
        }
        
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

    private void ApplyGravity()
    {
        if (IsGrounded() && velocity.y < 0.0f)
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

    Vector2 TranslateXInputToVelocity(Vector2 input)
    {
        return new Vector2(input.x * xMaxSpeed, velocity.y);
    }




    //When space is pressed
    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started && controlEnabled)
        {
            if (wasGrounded || (!wasGrounded && currentJumps < maxJumps)) //jump either from ground OR (in air AND have jumps left)
            {
                jumpChargeTime = 0f;
                jumpInput = true;
                wasGrounded = false; //FIXED wall bug: set wasgrounded to false here due to state mismatch
            }
        }

        //When space is released when you have started to jump
        if (context.canceled && jumpInput && currentJumps < maxJumps)
        {
            if (jumpChargeTime < minChargeTime) //FIXED gliding bug : short taps causes state mismatch (isgrounded/wasgrounded)
            {
                jumpChargeTime = minChargeTime;
            }
            
            //Debug.Log($"OnJump - moveInput {moveInput}, velocity before: {velocity}");
            float charge = Mathf.Clamp01(jumpChargeTime / maxChargeTime); 
            
            float jumpForce = Mathf.Lerp(minJumpForce, maxJumpForce, charge);

            TriggerJump(jumpForce);
            
            if (currentJumps > 1) playerSFX?.PlayDoubleJumpSound(); //play doublejump sound here
            
        }
    }
    
    //Called when we actually Jump
    private void TriggerJump(float jumpForce)
    {
        if (IsGrounded() && currentJumps == 0) playerSFX?.PlayJumpSound(); //play jump sound here

        clickedJump = true;

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

        if (moveInput == Vector2.right.normalized)
        {
            if (IsAgainstAWall(Vector2.right))
            {
                jumpForce += 1;
            }
        }
        if (moveInput == Vector2.left.normalized)
        {
            if (IsAgainstAWall(Vector2.left))
            {
                jumpForce += 1;
            }
        }

        //Set movement to player
        velocity.y = jumpForce;

        //Remove squash animation
        if (squashAndStretchManager != null)
            squashAndStretchManager.SetSquashState(false);

        jumpInput = false;
        currentJumps++;

        fallGravityScale = startFallGravityScale;
        xMaxSpeed = startXMaxSpeed;
    }

    //Triggered to reset the jump
    private void Land()
    {
        moveInput = Vector2.zero;

        currentJumps = 0; //reset jump counter when landing

        //has landed, play landing sound and trigger landing animation
        playerSFX?.PlayLandingSound();

        jumpedHighEnough = false;
        clickedJump = false;
    }







    //Bool that checks if one of the raycasts hits the ground within the specified distance
    private bool IsGrounded()
    {
        //Create two raycasts
        RaycastHit2D[] hit1 = Physics2D.RaycastAll(raycastCheck1.transform.position, Vector2.down, Mathf.Infinity);
        RaycastHit2D[] hit2 = Physics2D.RaycastAll(raycastCheck2.transform.position, Vector2.down, Mathf.Infinity);

        //Set to an impossibly long distance so at least one of the raycasts always will be shorter
        float shortestDistanceToGround = 10000f;
        bool boolToReturn = false;

        //Method to check both raycast arrays
        void HandleHits(RaycastHit2D[] hits)
        {
            for (int i = 0; i < hits.Length; i++)
            {
                if (hits[i].collider != null)
                {
                    if (hits[i].collider.CompareTag("Ground"))
                    {
                        if (hits[i].distance < shortestDistanceToGround)
                            shortestDistanceToGround = hits[i].distance;

                        if (hits[i].distance < groundCheckDistance)
                            boolToReturn = true;
                    }
                }
            }
        }

        //Call on the method for both of the created raycasts
        HandleHits(hit1);
        HandleHits(hit2);

        distanceToGround = shortestDistanceToGround;

        return boolToReturn;
    }

    //added to check if head is colliding
    private bool IsHeadbutt() 
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

    //Used to fight against "friction" when jumping against walls
    private bool IsAgainstAWall(Vector2 directionToCheckIfWallIsIn)
    {
        float wallNeedsToBeThisCloseToCount = 0.5f;

        //Method to check both raycast arrays
        bool HandleHits(RaycastHit2D[] hits)
        {
            for (int i = 0; i < hits.Length; i++)
            {
                if (hits[i].collider != null)
                {
                    if (hits[i].collider.CompareTag("Ground"))
                    {
                        if (hits[i].distance <= wallNeedsToBeThisCloseToCount)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        if (directionToCheckIfWallIsIn == Vector2.right)
        {
            RaycastHit2D[] rightHits = Physics2D.RaycastAll(rightWallChecker.transform.position, Vector2.right, Mathf.Infinity);
            return HandleHits(rightHits);
        }
        if (directionToCheckIfWallIsIn == Vector2.left)
        {
            RaycastHit2D[] leftHits = Physics2D.RaycastAll(leftWallChecker.transform.position, Vector2.left, Mathf.Infinity);
            return HandleHits(leftHits);
        }

        return false;
    }

    //Used to see if the player jumped high enough, only used to prevent bugs
    public bool JumpedHighEnough()
    {
        if (jumpedHighEnough)
            return true;

        if (!clickedJump)
            return false;
        else
        {
            if (distanceToGround > groundCheckDistance)
            {
                jumpedHighEnough = true;
                return true;
            }
        }

        return false;
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






    public void AddDoubleJump(int add)
    {
        maxJumps += add;
        Debug.Log($"Current Doublejumps: {maxJumps-1}");

        //Save it, so that even if the layer dies it will have the double jumps
        if (saveManager != null)
            saveManager.SavePlayerDoubleJump(maxJumps - 1);
    }
    private void SetMaxJumpAmount(int amountToSetTo)
    {
        maxJumps = amountToSetTo;
    }
    
    public void DisableDoubleJump()
    {
        maxJumps = 1;
        Debug.Log("Double Jump disabled");
    }

    public BoxCollider2D GroundCheckCollider => groundCheckCollider;

}
