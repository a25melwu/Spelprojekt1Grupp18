using System;
using UnityEngine;

public class PlayMusicWhenColliding : MonoBehaviour
{
    public int trackToPlay;
    private void OnTriggerEnter2D(Collider2D other)
    {
        SaveManager.instance.gameObject.GetComponentInChildren<AudioFade>().StartFadeIn(trackToPlay);
    }
}
