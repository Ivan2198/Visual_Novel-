using TMPro;
using UnityEngine;

public class DialogueAudio : MonoBehaviour
{
    private TMP_Animated animatedText;


    public AudioClip[] voices;
    public AudioClip[] punctuations;
    [Space]
    public AudioSource voiceSource;
    public AudioSource punctuationSource;
    public AudioSource effectSource;

    [Space]
    public AudioClip sparkleClip;
    public AudioClip rainClip;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void ReproduceSound(char c)
    {

        if (char.IsPunctuation(c) && !punctuationSource.isPlaying)
        {
            voiceSource.Stop();
            if (c == '!')
            {
                //effectSource.PlayOneShot(rainClip);
            }
            else
            {
                punctuationSource.clip = punctuations[Random.Range(0, punctuations.Length)];
                punctuationSource.Play();
            }
        }

        if (char.IsLetter(c) && !voiceSource.isPlaying)
        {
            punctuationSource.Stop();
            voiceSource.clip = voices[Random.Range(0, voices.Length)];
            voiceSource.Play();
        }

    }

    public void StopAllSounds()
    {
        Debug.Log("Stop sounds");
        // Stop all audio sources
        if (voiceSource != null)
        {
            voiceSource.Stop();
        }
        if (punctuationSource != null)
        {
            punctuationSource.Stop();
        }
        if (effectSource != null)
        {
            effectSource.Stop();
        }
    }
}