using UnityEngine;
using System.Collections;
using UnityEditor.Experimental.GraphView;
using UnityEngine.InputSystem;

public class BlockInputAndFade : MonoBehaviour
{
    public CanvasGroup group;
    public float fadeDuration = 1f;
    public float visibleTime = 2f;

    private Coroutine currentFade;
    private void Awake()
    {
        if (group == null) 
            group = GetComponent<CanvasGroup>();
        
        group.alpha = 0f;
        group.interactable = false;
        group.blocksRaycasts = false;
    }

    public void FadeIn()
    {
        StartCoroutine(Fade(0f, 1f));
    }

    public void FadeOut()
    {
        StartCoroutine(Fade(1f, 0f));
    }
    
    public void FadeInThenFadeOut()
    {
        if (currentFade != null)
            StopCoroutine(currentFade);
        
        currentFade = StartCoroutine(FadeInOutSequence());
    }

    private IEnumerator FadeInOutSequence()
    {
        
        
        yield return StartCoroutine(Fade(0f, 1f));
        
        yield return new WaitForSeconds(visibleTime);
        
        yield return StartCoroutine(Fade(1f, 0f));

        currentFade = null;
    }
    private IEnumerator Fade(float start, float end)
    {
        float t = 0;

        if (start < end)
        {
            group.interactable = false;
            group.blocksRaycasts = false;
        }
        
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            group.alpha = Mathf.Lerp(start, end, t / fadeDuration);
            yield return null;
        }
        group.alpha = end;

        if (end > 0.99f)
        {
            group.interactable = true;
            group.blocksRaycasts = true;
        }
        else
        {
            group.interactable = false;
            group.blocksRaycasts = false;
        }
        
        
    }
}
