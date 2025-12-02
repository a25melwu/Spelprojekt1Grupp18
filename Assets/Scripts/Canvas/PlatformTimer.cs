using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class PlatformTimer : MonoBehaviour
{
    private string tagToActivate = "Player";
    public UnityEvent onTriggerEnter;
    public UnityEvent onTriggerExit;
    public UnityEvent onTimerCompleted;
    
    private bool timerRunning = false;

    [SerializeField] private float timerLength = 1f;
    private float defaultTime;


    [Header("Visual feedback")] 
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private bool enableAlphaFade = true;
    
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        defaultTime = timerLength;

        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        if (timerRunning)
        {
            timerLength -= Time.deltaTime;
            
            //update alpha fade effect
            UpdateVisualFeedback();
        }

        if (timerLength <= 0f)
        {
            onTimerCompleted.Invoke();
            timerRunning = false;
            timerLength = defaultTime;
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

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(tagToActivate))
        {
            onTriggerEnter.Invoke();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(tagToActivate))
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
   
}
