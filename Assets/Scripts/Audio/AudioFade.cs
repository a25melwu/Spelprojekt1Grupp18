using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioFade : MonoBehaviour
{
    //private AudioSource source;
    public AudioSource[] audioSources;
    public float fadeInTargetVolume;
    public float fadeOutTargetVolume;
    public float fadeSpeed;
    public int audioTrackPlaying = 1;

    void Awake()
    {
        //source = GetComponent<AudioSource>();
        double startTime = AudioSettings.dspTime + 0.1;

        //audioSource1.PlayScheduled(startTime);
        //audioSource2.PlayScheduled(startTime);
        foreach (AudioSource audioSource in audioSources)
        {
            audioSource.PlayScheduled(startTime);
        }
        StartFadeIn(0);
        
    }

    public void StartFadeIn(int audioTrackToFadeIn)
    {
        //StopAllCoroutines();
        StartCoroutine("DoFadeIn", audioTrackToFadeIn);
        StartCoroutine("DoFadeOut", audioTrackPlaying);
    }

    public void StartFadeOut(int audioTrackToFadeOut)
    {
        //StopAllCoroutines();
        StartCoroutine("DoFadeOut", audioTrackToFadeOut);
    }

    private IEnumerator DoFadeIn(int audioTrackToFadeIn)
    {
        while(audioSources[audioTrackToFadeIn].volume != fadeInTargetVolume)
        {
            audioSources[audioTrackToFadeIn].volume = Mathf.MoveTowards(audioSources[audioTrackToFadeIn].volume, fadeInTargetVolume, fadeSpeed * Time.deltaTime);
            yield return null;
        }
        StopAllCoroutines();
        audioTrackPlaying = audioTrackToFadeIn;
    }

    private IEnumerator DoFadeOut(int audioTrackToFadeOut)
    {
        while (audioSources[audioTrackToFadeOut].volume != fadeOutTargetVolume)
        {
            audioSources[audioTrackToFadeOut].volume = Mathf.MoveTowards(audioSources[audioTrackToFadeOut].volume, fadeOutTargetVolume, fadeSpeed * Time.deltaTime);
            yield return null;
        }
        //StopAllCoroutines();
    }

}
