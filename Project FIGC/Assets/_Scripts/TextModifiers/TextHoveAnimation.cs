using UnityEngine;
using TMPro;
using DG.Tweening;

public class TextHoverAnimation : MonoBehaviour
{
    [Header("Animation Settings")]
    [SerializeField] private float hoverHeight = 10f;
    [SerializeField] private float animationDuration = 1f;
    [SerializeField] private Ease easeType = Ease.InOutSine;
    [SerializeField] private bool playOnStart = true;

    private TextMeshProUGUI tmpText;
    private Vector3 startPosition;
    private Tween hoverTween;

    void Start()
    {
        // Get the TextMeshPro component
        tmpText = GetComponent<TextMeshProUGUI>();
        if (tmpText == null)
        {
            Debug.LogError("No TextMeshPro component found!");
            return;
        }

        // Store the initial position
        startPosition = tmpText.transform.position;

        if (playOnStart)
        {
            StartHoverAnimation();
        }
    }

    public void StartHoverAnimation()
    {
        // Kill any existing animation
        if (hoverTween != null && hoverTween.IsPlaying())
        {
            hoverTween.Kill();
        }

        // Create the hovering animation
        hoverTween = tmpText.transform
            .DOMoveY(startPosition.y + hoverHeight, animationDuration)
            .SetEase(easeType)
            .SetLoops(-1, LoopType.Yoyo) // -1 means infinite loops
            .SetUpdate(true); // Make it continue during timescale changes
    }

    public void StopAnimation()
    {
        if (hoverTween != null && hoverTween.IsPlaying())
        {
            hoverTween.Kill();
            // Return to starting position
            tmpText.transform.position = startPosition;
        }
    }

    public void PauseAnimation()
    {
        hoverTween?.Pause();
    }

    public void ResumeAnimation()
    {
        hoverTween?.Play();
    }

    private void OnDisable()
    {
        StopAnimation();
    }

    private void OnDestroy()
    {
        StopAnimation();
    }
}