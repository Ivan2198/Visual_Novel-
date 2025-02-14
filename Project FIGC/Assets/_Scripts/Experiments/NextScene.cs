using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class NextScene : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    private bool sceneLoading = false; // Prevent multiple scene loads

    private void Start()
    {
        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached += OnVideoEnd; // Subscribe to event when video ends
        }
    }

    private void Update()
    {
        if (!sceneLoading && Input.GetMouseButtonDown(0))
        {
            LoadNextScene();
        }
    }

    private void OnVideoEnd(VideoPlayer vp)
    {
        LoadNextScene();
    }

    private void LoadNextScene()
    {
        if (!sceneLoading)
        {
            sceneLoading = true; // Prevent duplicate calls
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
}
