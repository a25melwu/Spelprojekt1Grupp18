using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;


public class PlayerSFX : MonoBehaviour
{
    
    [System.Serializable]
    public struct AudioType
    {
        [Tooltip("Descriptive name for this sound variation (e.g. 'Soft jump', 'Heavy Land' or 'All variations' - helps identify sounds in the inspector")]
        public string variationName;
        [Tooltip("Add soundclips here, can be several clips for one element/variation")]
        public AudioClip[] audioClip;
    }
    [SerializeField] private AudioSource audioSource;
    
    [Header("Player Sounds (One shot)")] 
    [NonReorderable] public AudioType[] jumpSounds;
    [NonReorderable] public AudioType[] doubleJumpSounds;
    //[SerializeField] private SoundGroup fallingSounds;
    [NonReorderable] public AudioType[] landingSounds;
    //[SerializeField] private SoundGroup flyingSounds;
    [NonReorderable] public AudioType[] hurtSounds;
    
    [Header("Item Sounds (One shot)")]
    [NonReorderable] public AudioType[] pickupSounds;

    public void PlayJumpSound() => PlayRandomSound(jumpSounds); //call this function from other classes
    public void PlayDoubleJumpSound() => PlayRandomSound(doubleJumpSounds); //call this function from other classes
    //public void PlayFallingSound() => PlayRandomSound(fallingSounds);
    public void PlayLandingSound() => PlayRandomSound(landingSounds); //call this function from other classes
    //public void PlayFlyingSound() => PlayRandomSound(flyingSounds);
    public void PlayHurtSound() => PlayRandomSound(hurtSounds); //call this function from other classes
    public void PlayPickupSound() => PlayRandomSound(pickupSounds); //call this function from other classes
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void PlayRandomSound(AudioType[] soundGroup)
    {
        if (audioSource == null || soundGroup == null || soundGroup.Length == 0) return;
        
        AudioType soundArray =  soundGroup[Random.Range(0, soundGroup.Length)];
        
        if (soundArray.audioClip != null && soundArray.audioClip.Length > 0)
        {
            AudioClip clip = soundArray.audioClip[Random.Range(0, soundArray.audioClip.Length)];
            audioSource.PlayOneShot(clip);
        }
        
    }
}
