using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class CreditsController : MonoBehaviour
{
    public VideoPlayer videoPlayer;   // The VideoPlayer component
    public UnityEvent onVideoSkip;    // Event triggered when the video is skipped
    public UnityEvent onVideoEnd;     // Event triggered when the video ends
    [SerializeField] private string sceneName;

    private void Start()
    {
        if (videoPlayer == null)
        {
            videoPlayer = GetComponent<VideoPlayer>();
        }

        // Start playing the video if it's not already playing
        if (videoPlayer != null && !videoPlayer.isPlaying)
        {
            videoPlayer.Play();
        }

        // Subscribe to the loopPointReached event to detect when the video ends
        videoPlayer.loopPointReached += OnVideoEndReached;
    }

    private void Update()
    {
        // Check if the Escape key is pressed to skip the video
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SkipVideo();
        }
    }

    // Method to skip the video and trigger an event
    public void SkipVideo()
    {
        if (videoPlayer.isPlaying)
        {
            // Stop the video
            videoPlayer.Stop();

            // Trigger the UnityEvent for skipping
            onVideoSkip.Invoke();
        }
    }

    // This method is called when the video ends
    private void OnVideoEndReached(VideoPlayer vp)
    {
        // Trigger the UnityEvent for when the video ends
        onVideoEnd.Invoke();
    }

    // Make sure to unsubscribe from the event when the object is destroyed
    private void OnDestroy()
    {
        videoPlayer.loopPointReached -= OnVideoEndReached;
    }

    public void ChangeScene()
    {
        SceneManager.LoadScene(sceneName);
    }
}
