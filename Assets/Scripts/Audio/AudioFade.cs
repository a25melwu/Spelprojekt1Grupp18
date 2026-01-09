using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioFade : MonoBehaviour
{
    [NonReorderable] public AudioSource[] audioSources;
    public float fadeInTargetVolume;
    public float fadeOutTargetVolume;
    public float fadeSpeed;
    private int audioTrackPlaying = -1;
    private bool isPlayingLastTrack = false;

    public static AudioFade Instance { get; private set; }
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return; //added to prevent destroyed instances from trying to initialize
        }
        
        double startTime = AudioSettings.dspTime + 0.1;
        
        foreach (AudioSource audioSource in audioSources)
        {
            if (audioSource != null)
            {
                audioSource.PlayScheduled(startTime);
                audioSource.loop = true; //all tracks loop by default for it to work
            }
        }
    }

    void OnDestroy() //called when gameobject is destroyed. cleans up static reference
    {
        if (Instance == this) 
        {
            Instance = null;
        }
    }
    
    public void StartFadeIn(int audioTrackToFadeIn)
    {
        isPlayingLastTrack = (audioTrackToFadeIn == audioSources.Length - 1); //checks if last audio track
        
        StopAllCoroutines();

        if (isPlayingLastTrack && audioSources[audioTrackToFadeIn] != null) //set last track looping to false
        {
            audioSources[audioTrackToFadeIn].loop = false;
        }
        
        StartCoroutine(DoFadeIn(audioTrackToFadeIn));
        if (audioTrackPlaying >= 0 && audioTrackPlaying != audioTrackToFadeIn) //added to prevent outofindex exception when initializing with -1
        {
            StartCoroutine(DoFadeOut(audioTrackPlaying));
        }
        
    }

    private IEnumerator DoFadeIn(int audioTrackToFadeIn)
    {
        if (audioSources[audioTrackToFadeIn] == null)
        {
            Debug.LogWarning($"AudioFade: AudioSource for track {audioTrackToFadeIn} is NULL!");
            yield break;
        }
        if (audioSources[audioTrackToFadeIn].clip == null)
        {
            Debug.LogWarning($"AudioFade: AudioClip for track {audioTrackToFadeIn} is NULL!");
            yield break;
        }
        
        while(audioSources[audioTrackToFadeIn].volume != fadeInTargetVolume)
        {
            //FIXED: used unscaledDeltaTime so that MoveTowards doesnt get interrupted by changing scenes
            audioSources[audioTrackToFadeIn].volume = Mathf.MoveTowards(audioSources[audioTrackToFadeIn].volume, fadeInTargetVolume, fadeSpeed * Time.unscaledDeltaTime); 
            yield return null;
        }
        
        audioTrackPlaying = audioTrackToFadeIn;
        
    }

    private IEnumerator DoFadeOut(int audioTrackToFadeOut)
    {
        if (audioSources[audioTrackToFadeOut] == null)
        {
            Debug.LogWarning($"AudioFade: AudioSource for track {audioTrackToFadeOut} is NULL!");
            yield break;
        }
        if (audioSources[audioTrackToFadeOut].clip == null)
        {
            Debug.LogWarning($"AudioFade: AudioClip for track {audioTrackToFadeOut} is NULL!");
            yield break;
        }
        while (audioSources[audioTrackToFadeOut].volume != fadeOutTargetVolume)
        {
            //FIXED: used unscaledDeltaTime so that MoveTowards doesnt get interrupted by changing scenes
            audioSources[audioTrackToFadeOut].volume = Mathf.MoveTowards(audioSources[audioTrackToFadeOut].volume, fadeOutTargetVolume, fadeSpeed * Time.unscaledDeltaTime);
            yield return null;
        }
        
    }

}
