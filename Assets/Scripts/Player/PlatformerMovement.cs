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
    [SerializeField] private HeadbuttChecker headChecker;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Animator anim;

    public bool controlEnabled { get; set; } = true;

    private Vector2 moveInput;
    private Rigidbody2D rb;
    
    private Vector2 velocity;
    private bool jumpInput = false;
    
    public InputActionAsset actionAsset;
    private PlayerSFX playerSFX; //jumping sounds
    private SquashAndStretch squashAndStretchManager;
    private SaveManager saveManager;

    [SerializeField] private bool isHeadbutt; //for checking if head is colliding

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

    public int maxJumps = 1;
    
    private float jumpChargeTime = 0f;
    public float maxChargeTime = 1.0f;
    private float minJumpForce = 1f;
    private float minChargeTime = 0.1f;
    
    [Header("Jump Buffer")]
    [Tooltip("Allowed time before landing to continue charging a jump. Increase time to increase distance to ground")]
    [SerializeField] private float maxJumpBuffer = 0.2f;
    private float jumpBufferTime = 0f;
    private bool startedChargingInAir = false; //important for distinguishing between ground jumps, air jumps due to crumbling platforms and doublejump-air-to-ground-jumps
    private bool startedChargingOnGround = false;
    [Tooltip("Jump cooldown time between taps. Decrease time to increase jumps/sec")]
    [SerializeField] private float jumpCooldown = 0.2f; //to prevent spam and to limit buffer timer increment in update
    private float lastJump = 0f;
    private bool shouldJump = false;
    
    private float startFallGravityScale;
    private float startXMaxSpeed;

    [Header("Raycast Objects")]
    [SerializeField] private GameObject rightWallChecker;
    [SerializeField] private GameObject leftWallChecker;
    [SerializeField] private GameObject raycastCheck1;
    [SerializeField] private GameObject raycastCheck2;

    [SerializeField] private float groundCheckDistance = 0.01f;

    [Header("Public just for bug checking")]
    [SerializeField] private float distanceToGround;
    [SerializeField] private bool clickedJump;
    [SerializeField] private bool jumpedHighEnough = false;
    [SerializeField] private int currentJumps = 0;


    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        //groundCheckCollider.isTrigger = true;
        
        //Set gravity scale to 0 so player wont "fall"
        rb.gravityScale = 0;
        
        playerSFX = GetComponentInChildren<PlayerSFX>(); //soundgroup playerSFX
        squashAndStretchManager = GetComponentInParent<SquashAndStretch>();

        startFallGravityScale = fallGravityScale;
        startXMaxSpeed = xMaxSpeed;

        //If we have gotten a double jump and then respawned, we get the double jumps back
        saveManager = FindFirstObjectByType<SaveManager>();
        if (saveManager != null)
            SetMaxJumpAmount(saveManager.GetDoubleJumpsPlayerShouldStartWith() + 1); //We should start with 0 double jumps, but we want to be able to jump once

    }

    private void Start()
    {

        SaveManager.instance.gameObject.GetComponentInChildren<InstantiateUIDoublejump>().SetAllFeatherColorToAvailable();
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

        if (jumpInput) //jumpbuffertimer to check how close to ground player is before landing
        {
            if (!IsGrounded()) //only start increment when not grounded
            {
                float before = jumpBufferTime;
                jumpBufferTime += Time.deltaTime;
                Debug.Log($"Update: jumpinput {jumpInput}, !IsGrounded() {!IsGrounded()}, buffertime +{before} = {jumpBufferTime}");
            }
        }
        
        if (jumpInput && HasJumpsLeft())
        {
            if (IsGrounded()) //jump cases; when grounded and space is pressed within max jump buffer time, or when player has doublejump and is charging a jump in air - lands - but should still jump
            {
                shouldJump = ((jumpBufferTime <= maxJumpBuffer) || maxJumps > 1);
            }
            else //jump cases; doublejumps in air or when falling (without doublejump) can still do first jump in air, jump in air after charging on ground then falls
            {
                shouldJump = HasJumpsLeft() || (!startedChargingInAir && currentJumps == 0);
            }

            if (shouldJump)
            {
                ChargingJump();
            }
            
        }
        
        /*if (startedChargingOnGround)
        {
            jumpChargeTime = 0f; //always start charging whenever jump is pressed (ground or air) --new buffer system
            jumpInput = true;
        }*/

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
    
    private void ChargingJump()
    {
        jumpChargeTime += Time.deltaTime;
        Debug.Log($"ChargingJump : jumpchargetime {jumpChargeTime}");

        //The player slows when we double jump
        fallGravityScale = startFallGravityScale * airFallGravityScaleSlowMultiplier;
        xMaxSpeed = startXMaxSpeed * xSpeedAirJumpSlowMultiplier;

            
        if (squashAndStretchManager != null)
            squashAndStretchManager.SetSquashState(true);
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
            jumpChargeTime = 0f; //always start charging whenever jump is pressed (ground or air) --new buffer system
            
            if (!jumpInput) //only reset for each jump, prevents spam?
            {
                jumpBufferTime = 0f;
            }
            
            jumpInput = true;
            
            //If double jump
            if(!IsGrounded())
            {
                startedChargingInAir = true; //tracks if charge is started in air, to be able to jump when charging on ground -falls- and then jump without doublejump
                startedChargingOnGround = false;
                if(SaveManager.instance.playerDoubleJumpsSaved > 0)
                {
                    anim.SetBool("charging", true);
                    anim.SetBool("cancel", false);
                }
                
            }
            else
            {
                startedChargingInAir = false;
                startedChargingOnGround = true;
            }
            
            Debug.Log($"OnJump started: currentjumps {currentJumps}");
            
            
        }

        //When space is released when you have started to jump
        if (context.canceled && jumpInput)
        {
            if ((Time.time - lastJump) < jumpCooldown)
            {
                Debug.Log($"OnJump canceled: jump cooldown");
                jumpInput = false;
                lastJump = Time.time;
                jumpBufferTime = 0f;
                return;
            }
            
            lastJump = Time.time;
            jumpBufferTime = 0f;

            if (!shouldJump)
            {
                jumpInput = false;
                return;
            }
            
            Debug.Log($"OnJump canceled: jumpbuffertime {jumpBufferTime:F3}, maxjumpbuffer {maxJumpBuffer}, comparison {jumpBufferTime < maxJumpBuffer}");
            
            /*if (IsGrounded()) //jump cases; when grounded and space is pressed within max jump buffer time, or when player has doublejump and is charging a jump in air - lands - but should still jump
            {
                shouldJump = ((jumpBufferTime <= maxJumpBuffer) || maxJumps > 1) && HasJumpsLeft();
            }
            else //jump cases; doublejumps in air or when falling (without doublejump) can still do first jump in air, jump in air after charging on ground then falls
            {
                shouldJump = HasJumpsLeft() || (!startedChargingInAir && currentJumps == 0);
            }*/

            StartJump();

        }
    }

    private void StartJump()
    {
        
        if (jumpChargeTime < minChargeTime) //FIXED gliding bug : short taps causes state mismatch
        {
            jumpChargeTime = minChargeTime;
        }
            
        float charge = Mathf.Clamp01(jumpChargeTime / maxChargeTime); 
        float jumpForce = Mathf.Lerp(minJumpForce, maxJumpForce, charge);

        TriggerJump(jumpForce);
            
        if (currentJumps > 1 && !IsGrounded()) playerSFX?.PlayDoubleJumpSound(); //play doublejump sound here
            
        jumpBufferTime = 0f; //force reset of jump buffer timer
        jumpInput = false;
        startedChargingInAir = false;
            
        Debug.Log($"StartJump: currentjumps {currentJumps}");
    }
    
    //Called when we actually Jump
    private void TriggerJump(float jumpForce)
    {
        if (IsGrounded() && currentJumps == 0) playerSFX?.PlayJumpSound(); //play normal jump sound here
        else if(!IsGrounded() && startedChargingInAir) //Double jump
        {
            SaveManager.instance.gameObject.GetComponentInChildren<InstantiateUIDoublejump>().SetFeatherToUsedUpColor();
        }

        if (SaveManager.instance.playerDoubleJumpsSaved > 0)
        {
            anim.SetBool("charging", false);
        }

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
        shouldJump = false;
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

        SaveManager.instance.gameObject.GetComponentInChildren<InstantiateUIDoublejump>().SetAllFeatherColorToAvailable();

        if (SaveManager.instance.playerDoubleJumpsSaved > 0)
        {
            anim.SetBool("cancel", true);
            anim.SetBool("charging", false);
        }
        
        if (squashAndStretchManager != null)
            squashAndStretchManager.SetSquashState(false);
        
        Debug.Log($"Land: isgrounded {IsGrounded()}, jumpbuffer {jumpBufferTime<maxJumpBuffer}, hasjumpsleft {HasJumpsLeft()}");
        
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
                    if (hits[i].collider.CompareTag("Ground") || hits[i].collider.CompareTag("Platform"))
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
        return headChecker.touchingCeiling;
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

        InstantiateUIDoublejump playerUIDoublejump = FindFirstObjectByType<InstantiateUIDoublejump>();
        playerUIDoublejump.AddUIDoubleJump(1);
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
