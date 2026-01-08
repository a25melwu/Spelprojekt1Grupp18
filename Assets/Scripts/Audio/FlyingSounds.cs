using UnityEngine;

public class FlyingSounds : MonoBehaviour
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
