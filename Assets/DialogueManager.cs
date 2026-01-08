using UnityEngine;
using TMPro;
using System.Collections;

public class DialogueManager : MonoBehaviour
{
    public TextMeshProUGUI mainText;
    [TextArea(2, 15)]
    public string textToShow;

    private void Start()
    {
        StartCoroutine(nameof(RevealText), textToShow);
    }

    IEnumerator RevealText(string textToReveal)
    {
        mainText.text = textToReveal;
        int totalCharactersToShow = textToReveal.Length + 1;

        for (int i = 0; i < totalCharactersToShow; i++)
        {
            mainText.maxVisibleCharacters = i;
            yield return new WaitForSeconds(0.1f);
        }

    }


}
