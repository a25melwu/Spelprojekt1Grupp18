using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;
using UnityEngine.UI;

public class Dialogue : MonoBehaviour
{
    [SerializeField] private Canvas dialogueCanvas;
    [SerializeField] private TMPro.TextMeshProUGUI textBox;
    //[SerializeField] private Button nextButton;
    [SerializeField] List<string> dialogue;

    [SerializeField] private UnityEvent onStartDialogue;
    [SerializeField] private UnityEvent onEndDialogue;
    
    [Header("Fade Settings")]
    [SerializeField] private float fadeOutDelay = 5f;
    [SerializeField] private float fadeTime = 1f;
    
    private int currentIndex = -1;
    private Coroutine fadeCoroutine;

    void Awake()
    {
        if (dialogueCanvas != null)
        {
            dialogueCanvas.enabled = false;
        }
    }

    public void NextDialogue()
    {
        if (fadeCoroutine != null) //stop any existing fade coroutine
        {
            StopCoroutine(fadeCoroutine);
            fadeCoroutine = null;
        }
        
        if (currentIndex < 0) // If dialogue wasn't started: run the onStartDialogue-event
        {
            onStartDialogue?.Invoke();
            dialogueCanvas.enabled = true;
            //nextButton?.onClick.AddListener(NextDialogue);
        }
        
        
        currentIndex++; // Move to the next item in the list of dialogue-lines
        
        
        if (currentIndex < dialogue.Count) // If we're not at the end of the list yet: update text in text-box
        {
            textBox.text = dialogue[currentIndex];

            ResetTextAlpha(); //reset text alpha to fully visible
            fadeCoroutine = StartCoroutine(FadeOutTextAfterDelay()); //start fade out after delay
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

    private IEnumerator FadeOutTextAfterDelay()
    {
        yield return new WaitForSeconds(fadeOutDelay); //wait for the delay

        yield return StartCoroutine(FadeOutText()); //then fade out the text
    }

    private IEnumerator FadeOutText()
    {
        Color textColor = textBox.color;
        float elapsedTime = 0f;

        while (elapsedTime < fadeTime)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeTime);
            textBox.color = new Color(textColor.r, textColor.g, textColor.b, alpha);
            yield return null;
        }
        
        textBox.color = new Color(textColor.r, textColor.g, textColor.b, 0f); //ensures invisibility
    }

    private void ResetTextAlpha()
    {
        Color textColor = textBox.color;
        textBox.color = new Color(textColor.r, textColor.g, textColor.b, 1f);
    }
}