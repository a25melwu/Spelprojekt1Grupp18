using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class CrumblingPlatform : MonoBehaviour
{
    private string tagToActivate = "Player";
    public UnityEvent onTriggerEnter;
    public UnityEvent onTriggerExit;
    public UnityEvent onTimerCompleted;

    private BoxCollider2D playerGroundCheck;
    private bool timerRunning = false;

    [Header("Platform settings")]
    [Tooltip("Platform duration in seconds")]
    [SerializeField] private float timerLength = 1f;
    private float defaultTime;
    [Tooltip("Platform respawn time in seconds")]
    [SerializeField] private float timerRespawn = 8f;
    [SerializeField] private bool autoRespawn = true;
    private Collider2D platformCollider;

    [Header("Visual feedback")] 
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private bool enableAlphaFade = true;

    [Header("Animation")]
    [SerializeField] private Animator animator;
    private string progressParameter = "TimerProgress";
    
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        defaultTime = timerLength;

        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
        
        platformCollider = GetComponent<Collider2D>();
        
        playerGroundCheck = GetPlayerGroundCheckCollider();

    }
    

    // Update is called once per frame
    void Update()
    {
        if (timerRunning)
        {
            timerLength -= Time.deltaTime;
            
            UpdateVisualFeedback(); //update alpha fade effect
            
            float progress = 1f - (timerLength / defaultTime); //send progress to animator
            animator.SetFloat(progressParameter, progress);
        }

        if (timerLength <= 0f)
        {
            OnTimerComplete();
        }
        
    }
    

    public void StartTimer()
    {
        timerRunning = true;
    }

    public void StopTimer()
    {
        timerRunning = false;
    }

    private void OnTimerComplete()
    {
        SetPlatformVisible(false);

        if (autoRespawn)
        {
            StartCoroutine(RespawnAfterDelay(timerRespawn));
        }
        
        onTimerCompleted.Invoke();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!spriteRenderer.enabled) return;
        
        //only invokes if triggered by player groundcheck collider
        if (other.CompareTag(tagToActivate) && playerGroundCheck != null && other == playerGroundCheck) 
        {
            onTriggerEnter.Invoke();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        //only invokes if triggered by player groundcheck collider
        if (other.CompareTag(tagToActivate) && playerGroundCheck != null && other == playerGroundCheck)
        {
            onTriggerExit.Invoke();
        }
    }

    private void UpdateVisualFeedback()
    {
        if (!enableAlphaFade || spriteRenderer == null) return;
        
        float progress = 1f - (timerLength / defaultTime);
        Color newColor = spriteRenderer.color;
        newColor.a = Mathf.Lerp(1f, 0f, progress);
        spriteRenderer.color = newColor;
    }

    private IEnumerator RespawnAfterDelay(float time)
    {
        yield return new WaitForSeconds(time);
        SetPlatformVisible(true);
        ResetPlatform();
    }

    private void SetPlatformVisible(bool visible)
    {
        if (visible)
        {
            if (spriteRenderer != null)
            {
                spriteRenderer.enabled = true;
                Color color = spriteRenderer.color;
                color.a = 1f;
                spriteRenderer.color = color;
                
            }
            
            if (platformCollider != null)
            {
                platformCollider.enabled = true;
            }

        }
        else
        {
            spriteRenderer.enabled = false;
            platformCollider.enabled = false;
        }
        

        if (visible && animator != null)
        {
            animator.SetFloat(progressParameter, 0f);
            animator.Rebind(); //reset animation state
        }
        
    }

    private void ResetPlatform()
    {
        timerLength = defaultTime;
        timerRunning = false;
    }

    private BoxCollider2D GetPlayerGroundCheckCollider()
    {
       
        PlatformerMovement playerMovement = FindFirstObjectByType<PlatformerMovement>();
        if (playerMovement != null)
        {
            return playerMovement.GroundCheckCollider;
        }
        
        return null;

    }
    
}
