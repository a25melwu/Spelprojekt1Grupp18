using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class Dialogue : MonoBehaviour
{
    [SerializeField] private Canvas dialogueCanvas;
    private CanvasGroup canvasGroup;
    [SerializeField] private TMPro.TextMeshProUGUI textBox;
    //[SerializeField] private Button nextButton;
    [TextArea(2,15)]
    [SerializeField] List<string> dialogueText;

    [SerializeField] private UnityEvent onStartDialogue;
    [SerializeField] private UnityEvent onEndDialogue;

    [Header("Fade Settings")] 
    [Tooltip("Fade in duration in seconds")]
    [SerializeField] private float fadeInTime = 1.5f;
    [Tooltip("Display duration in seconds")]
    [SerializeField] private float timeDisplay = 8f;
    [Tooltip("Fade out duration in seconds")]
    [SerializeField] private float fadeOutTime = 1f;
    
    private int currentIndex = -1;
    private Coroutine currentCoroutine;

    void Awake()
    {
        InitializeCanvasGroup();
        
        if (dialogueCanvas != null)
        {
            dialogueCanvas.enabled = true;
            canvasGroup.alpha = 0f;
            //canvasGroup.interactable = false;
            //canvasGroup.blocksRaycasts = false;
        }
    }

    public void NextDialogue()
    {
        if (currentCoroutine != null) //stop any existing fade coroutine
        {
            StopCoroutine(currentCoroutine);
            currentCoroutine = null;
        }
        
        if (currentIndex < 0) // If dialogue wasn't started: run the onStartDialogue-event
        {
            onStartDialogue?.Invoke();
            //nextButton?.onClick.AddListener(NextDialogue);

            currentCoroutine = StartCoroutine(FadeInTextAndCanvas());
        }
        
        
        currentIndex++; // Move to the next item in the list of dialogue-lines
        
        
        if (currentIndex < dialogueText.Count) // If we're not at the end of the list yet: update text in text-box
        {
            textBox.text = dialogueText[currentIndex];

            ResetAlpha(); //reset text alpha to fully visible
            currentCoroutine = StartCoroutine(FadeOutTextAndCanvasAfterDelay()); //start fade out after delay
        }
        
        else // If we are at the end of the list: run the onEndDialogue-event, reset the index (so that the dialogue can be restarted) and remove this dialog from the next-button
        {
            textBox.text = "";
            onEndDialogue?.Invoke();
            currentIndex = -1;
            dialogueCanvas.enabled = false;
            //nextButton?.onClick.RemoveListener(NextDialogue);
        }
    }

    private IEnumerator FadeOutTextAndCanvasAfterDelay()
    {
        yield return new WaitForSeconds(timeDisplay); //wait for the delay

        yield return StartCoroutine(FadeOutTextAndCanvas()); //then fade out the text and canvas
    }

    private IEnumerator FadeOutTextAndCanvas()
    {
        //Color textColor = textBox.color;
        float elapsedTime = 0f;

        while (elapsedTime < fadeOutTime)
        {
            elapsedTime += Time.deltaTime;
            //float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeOutTime);
            //textBox.color = new Color(textColor.r, textColor.g, textColor.b, alpha);
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsedTime /fadeOutTime);
            yield return null;
        }
        
        //textBox.color = new Color(textColor.r, textColor.g, textColor.b, 0f); //ensures invisibility
        canvasGroup.alpha = 0f;
    }

    private IEnumerator FadeInTextAndCanvas()
    {
        //Color textColor = textBox.color;
        float elapsedTime = 0f;

        while (elapsedTime < fadeInTime)
        {
            elapsedTime += Time.deltaTime;
            //float alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeInTime);
            //textBox.color = new Color(textColor.r, textColor.g, textColor.b, alpha);
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsedTime /fadeInTime);
            yield return null;
        }
        
        //textBox.color = new Color(textColor.r, textColor.g, textColor.b, 1f); //ensures fully visible
        canvasGroup.alpha = 1f;
    }

    private void ResetAlpha()
    {
        //Color textColor = textBox.color;
        //textBox.color = new Color(textColor.r, textColor.g, textColor.b, 1f);
        canvasGroup.alpha = 1f;
    }
    
    private void InitializeCanvasGroup()
    {
        if (dialogueCanvas == null)
        {
            Debug.LogError("Dialogue canvas is not assigned!");
            return;
        }
        
        canvasGroup = dialogueCanvas.GetComponent<CanvasGroup>(); //Add canvasgroup to canvas gameobject
        if (canvasGroup == null) //if not found, create one
        {
            canvasGroup = dialogueCanvas.gameObject.AddComponent<CanvasGroup>(); //create
            Debug.Log("Added CanvasGroup component to dialogueCanvas");
        }
        
        canvasGroup.alpha = 0f; //set canvasgroup to invisible when awake
        //canvasGroup.interactable = false;
        //canvasGroup.blocksRaycasts = false;
    }
}