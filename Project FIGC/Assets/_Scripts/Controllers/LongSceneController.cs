using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class LongSceneController : MonoBehaviour
{
    public GameController gameController;
    [SerializeField] private GameObject longSceneGO;

    [Header("Paneo")]
    [SerializeField] private RectTransform imageRectTransform;
    [SerializeField] private float scrollDuration = 10f;
    [SerializeField] private float pauseDuration = 2f;
    [SerializeField] private Ease scrollEase = Ease.InOutQuad;

    private float startPosition;
    private float endPosition;
    private Sequence scrollSequence;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            CreateScrollSequence();
        }
    }
    public void CreateScrollSequence()
    {
        // Kill any existing sequence
        scrollSequence?.Kill();

        // Create a new sequence
        scrollSequence = DOTween.Sequence();

        // Reset position
        imageRectTransform.anchoredPosition = new Vector2(startPosition, imageRectTransform.anchoredPosition.y);

        // Add the scroll animation
        scrollSequence.Append(imageRectTransform.DOAnchorPosX(endPosition, scrollDuration).SetEase(scrollEase))
                     .AppendInterval(pauseDuration)
                     .Append(imageRectTransform.DOAnchorPosX(startPosition, scrollDuration).SetEase(scrollEase))
                     .AppendInterval(pauseDuration)
                     .SetLoops(-1); // -1 means infinite loops
    }

    private void OnDisable()
    {
        // Clean up the sequence when the object is disabled
        scrollSequence?.Kill();
    }

    // Public method to pause the scrolling
    public void PauseScrolling()
    {
        scrollSequence?.Pause();
    }

    // Public method to resume the scrolling
    public void ResumeScrolling()
    {
        scrollSequence?.Play();
    }

    // Public method to restart the scrolling from the beginning
    public void RestartScrolling()
    {
        CreateScrollSequence();
    }
    public void SetupChoose(LongScene scene)
    {
        if (imageRectTransform == null)
        {
            imageRectTransform = GetComponent<RectTransform>();
        }

        // Calculate start and end positions
        // For a 3840px wide image in 1920px viewport, we need to move -1920px
        startPosition = 0f;
        endPosition = -1920f;

        longSceneGO.SetActive(true);
        Debug.Log("Game Sequence");
    }

    public void PerformChoose(StoryScene scene)
    {
        gameController.PlayScene(scene);
        longSceneGO.SetActive(false);
    }
}
