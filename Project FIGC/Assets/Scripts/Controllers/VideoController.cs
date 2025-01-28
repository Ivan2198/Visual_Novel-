using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Video;

public class VideoController : MonoBehaviour
{
    public GameController gameController;

    private Animator animator;
    private StoryScene _scene;
    public VideoPlayer videoPlayer;
    private float videoDuration;

    void Start()
    {
        animator = GetComponent<Animator>();
        if (videoPlayer == null)
            videoPlayer = GetComponent<VideoPlayer>();
        // Prepare the video
        videoPlayer.Prepare();

        // Subscribe to the prepareCompleted event
        videoPlayer.prepareCompleted += OnVideoPrepared;
    }

    public void SetupChoose(Videos scene)
    {
        Debug.Log("Playing Video");
        animator.SetTrigger("Show");
        
        videoPlayer.clip = scene.labels[0]._videoClip;
        videoDuration = (float)scene.labels[0]._videoClip.length;
        _scene = scene.labels[0].nextScene;
        videoPlayer.Play();
        StartCoroutine(WaitForTheVideoToEnd());
    }
    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    SkipScene();
        //}
    }

    public void PerformChoose(StoryScene scene)
    {
        gameController.PlayScene(scene);
        animator.SetTrigger("Hide");
    }
    IEnumerator WaitForTheVideoToEnd()
    {
        yield return new WaitForSeconds(videoDuration);
        Debug.Log("Video ended");
        SkipScene();
        
    }
    public void PlayVideo()
    {
        if (videoPlayer.isPrepared)
            videoPlayer.Play();
    }

    public void PauseVideo()
    {
        if (videoPlayer.isPlaying)
            videoPlayer.Pause();
    }

    public void StopVideo()
    {
        videoPlayer.Stop();
    }
    private void OnVideoPrepared(VideoPlayer source)
    {
        // Access the duration of the video once it is prepared
        float durationInSeconds = (float)videoPlayer.length;
        Debug.Log("Video Duration: " + durationInSeconds + " seconds.");

        // Optionally, you can start playing the video
        //videoPlayer.Play();
    }

    void OnDestroy()
    {
        // Unsubscribe from the event when the GameObject is destroyed
        if (videoPlayer != null)
        {
            videoPlayer.prepareCompleted -= OnVideoPrepared;
        }
    }
    public void SkipScene()
    {
        PerformChoose(_scene);
    }
}
