using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;

public class AudioSlider : MonoBehaviour
{
    public AudioMixer mixer;

    public string audioMixerParameter;

    public Slider slider;

    public TextMeshProUGUI sliderValueText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public enum ScaleType
    {
        Percentage,
        Decimals
    }

    public ScaleType valueScaleType = ScaleType.Percentage;

    void Awake()
    {
        mixer.GetFloat(audioMixerParameter, out float previousValue);
        //logarithmic to linear
        slider.value = MathF.Pow(10, (previousValue / 20));
        SetText(slider.value);
    }

    public void SetLevel(float sliderValue)
    {
        SetText(sliderValue);

        if (sliderValue < 0.0001f)
        {
            mixer.SetFloat(audioMixerParameter, -80f);
            return;
        }
        
        //linear to logarithmic
        mixer.SetFloat(audioMixerParameter, MathF.Log10(sliderValue) * 20);
        
    }

    private void SetText(float value)
    {
        switch (valueScaleType)
        {
            case ScaleType.Percentage:
                float valuePercent = value * 100;
                sliderValueText.SetText($"{(int)valuePercent}");
                break;
            case ScaleType.Decimals:
                sliderValueText.SetText($"{value:N2}");
                break;
        }
    }
}
