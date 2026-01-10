using UnityEngine;

public class DialogueCaller : MonoBehaviour
{
    [TextArea(2, 15)]
    public string textToShow;
    public string textToShow2;

    public Vector2 dialoguePosition;
    public bool hasBeenShowed = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            if (hasBeenShowed) return; 
            FindFirstObjectByType<DialogueManager>().StartDialogue(textToShow, dialoguePosition);
            hasBeenShowed = true;

            if(textToShow2 != "")
                FindFirstObjectByType<DialogueManager>().StartDialogue(textToShow2, dialoguePosition);
        }
    }
}
