using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class DialogueManager : MonoBehaviour
{
    [Header("References")]
    public TextMeshProUGUI mainText;
    public GameObject canvas; //Turn on and off and move around

    [Header("Variables")]
    [SerializeField] private float textSpeed = 0.085f;
    private float originalTextSpeed = 0.085f;

    [SerializeField] private float timeToWaitBeforeHidingAfterDialogieFinished = 1f;
    public bool whenTrueAndTheTextIsOverEndGame;

    [Header("Others")]
    [TextArea(2, 15)]
    public string textToShow;

    private bool isShowingDialogue = false;
    public List<string> dialogueQueue = new();
    public List<Vector2> positionQueue = new();

    private void AddTextToQueue(string textToQueue, Vector2 position)
    {
        dialogueQueue.Add(textToQueue);
        positionQueue.Add(position);

        if(dialogueQueue.Count > 0)
        {
            textSpeed = originalTextSpeed - 0.01f * dialogueQueue.Count;
        }
    }

    private void Start()
    {
        ShowCanvas(false);
        originalTextSpeed = textSpeed;
    }

    public void StartDialogue(string showThisText, Vector2 position)
    {
        if (isShowingDialogue)
        {
            AddTextToQueue(showThisText, position);
        }
        else
        {
            StartCoroutine(RevealText(showThisText, position));
        }
    }

    IEnumerator RevealText(string textToReveal, Vector2 position)
    {
        ShowCanvas(true);
        isShowingDialogue = true;
        canvas.transform.localPosition = position; 

        mainText.text = textToReveal;
        int totalCharactersToShow = textToReveal.Length + 1;

        for (int i = 0; i < totalCharactersToShow; i++)
        {
            mainText.maxVisibleCharacters = i;
            yield return new WaitForSeconds(textSpeed);
        }

        yield return new WaitForSeconds(timeToWaitBeforeHidingAfterDialogieFinished);

        ShowCanvas(false);
        isShowingDialogue = false;

        if(dialogueQueue.Count > 0)
        {
            string nextTextToShow = dialogueQueue[0];
            dialogueQueue.RemoveAt(0);
            StartDialogue(nextTextToShow, positionQueue[0]);
            positionQueue.RemoveAt(0);
        }
        else
        {
            textSpeed = originalTextSpeed;

            if (whenTrueAndTheTextIsOverEndGame)
            {
                MainMenu mainMenu = FindFirstObjectByType<MainMenu>();
                mainMenu.OnOpenFinishMenu();
            }
        }
    }

    private void ShowCanvas(bool show)
    {
        canvas.SetActive(show);
    }


}
