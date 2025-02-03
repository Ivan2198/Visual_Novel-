using UnityEngine;

public class AudioClipPlay : MonoBehaviour
{
    public AudioSource audioSource;
   
    // Play a given audio clip
    public void PlayAudioClip(AudioClip clip)
    {
        if (audioSource == null)
        {
            Debug.LogError("AudioSource is not assigned.");
            return;
        }

        audioSource.clip = clip;
        audioSource.loop = false;
        audioSource.volume = .8f;
        audioSource.Play();
    }
}
