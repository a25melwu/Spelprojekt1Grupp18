using UnityEngine;

public class FlyingSound : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;

    public void StartFlyingSound()
    {
        if (audioSource == null) return;
        
        audioSource.Play();
    }

    public void StopFlyingSound()
    {
        if (audioSource == null) return;
        
        audioSource.Stop();
    }
}
