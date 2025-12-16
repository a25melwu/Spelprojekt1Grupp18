using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;


public class ParallaxColorFade : MonoBehaviour
{
    public enum CurrentColor
    {
        Default = 0,
        Color1 = 1,
        Color2 = 2,
        Color3 = 3,
        Color4 = 4,
        Color5 = 5,
    }
    
    [Header("Color Selection")] 
    [SerializeField] private CurrentColor currentColor = CurrentColor.Default;

    [Header("Custom Colors (Set these in Inspector)")]
    [SerializeField] private Color defaultColor = Color.white;
    [SerializeField] private Color color1 = Color.white;
    [SerializeField] private Color color2 = Color.white;
    [SerializeField] private Color color3 = Color.white;
    [SerializeField] private Color color4 = Color.white;
    [SerializeField] private Color color5 = Color.white;

    [Header("Fade Settings")] 
    [SerializeField] private float fadeDuration = 1f;
    
    private SpriteRenderer[] spriteRenderers;
    private Coroutine fadeRoutine;

    public void FadeToDefault() => StartFade(CurrentColor.Default);
    public void FadeToColor1() => StartFade(CurrentColor.Color1);
    public void FadeToColor2() => StartFade(CurrentColor.Color2);
    public void FadeToColor3() => StartFade(CurrentColor.Color3);
    public void FadeToColor4() => StartFade(CurrentColor.Color4);
    public void FadeToColor5() => StartFade(CurrentColor.Color5);
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
   
    void Awake()
    {
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>(); //get ALL children SpriteRenderers
        ApplyColor(GetColorFromSelection(currentColor));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void FadeToColor(int colorIndex)
    {
        if (colorIndex >= 0 && colorIndex <= 5)
        {
            StartFade((CurrentColor)colorIndex);
        }
    }
    
    private void StartFade(CurrentColor targetColor)
    {
        if (fadeRoutine != null)
        {
            StopCoroutine(fadeRoutine);
        }

        fadeRoutine = StartCoroutine(FadeRoutine(targetColor, fadeDuration));
    }

    private IEnumerator FadeRoutine(CurrentColor targetPreset, float duration)
    {
        Color startColor = GetColorFromSelection(currentColor);
        Color targetColor = GetColorFromSelection(targetPreset);

        float timer = 0f;

        while (timer < duration)
        {
            Color color = Color.Lerp(startColor, targetColor, timer / duration);
            ApplyColor(color);
            
            timer += Time.deltaTime;
            yield return null;
        }
        
        currentColor = targetPreset;
        ApplyColor(targetColor);
    }
    
    private void ApplyColor(Color color)
    {
        foreach (SpriteRenderer sr in spriteRenderers)
        {
            sr.color = color;
        }
    }

    private Color GetColorFromSelection(CurrentColor colorSelection)
    {
        switch (colorSelection)
        {
            case CurrentColor.Default:
                return defaultColor;
            case CurrentColor.Color1:
                return color1;
            case CurrentColor.Color2:
                return color2;
            case CurrentColor.Color3:
                return color3;
            case CurrentColor.Color4:
                return color4;
            case CurrentColor.Color5:
                return color5;
            default:
                return defaultColor;
        }
    }
}
