using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections.Generic;

public class LongSceneController : MonoBehaviour
{
    public GameController gameController;
    [SerializeField] private GameObject longSceneGO;

    [Header("Paneo")]
    [SerializeField] private Image bgImage;
    [SerializeField] private List<RectTransform> parallaxLayers; // List of images for parallax
    [SerializeField] private List<float> parallaxSpeeds; // Speed multipliers for each layer
    [SerializeField] private float scrollDuration = 10f;
    [SerializeField] private float pauseDuration = 2f;
    [SerializeField] private Ease scrollEase = Ease.InOutQuad;

    private float startPosition;
    private float endPosition;
    private List<Sequence> scrollSequences = new List<Sequence>();
    private LongScene longScene;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            CreateScrollSequence();
        }
    }

    public void CreateScrollSequence()
    {
        // Kill existing sequences
        foreach (var seq in scrollSequences)
        {
            seq?.Kill();
        }
        scrollSequences.Clear();

        // Calculate positions
        startPosition = 0f;
        endPosition = -1920f;

        for (int i = 0; i < parallaxLayers.Count; i++)
        {
            RectTransform layer = parallaxLayers[i];
            float speedMultiplier = parallaxSpeeds[i];

            Sequence layerSequence = DOTween.Sequence();

            // Reset position
            layer.anchoredPosition = new Vector2(startPosition, layer.anchoredPosition.y);

            // Add parallax scrolling effect (plays only once)
            layerSequence.Append(layer.DOAnchorPosX(endPosition * speedMultiplier, scrollDuration).SetEase(scrollEase))
                         .AppendInterval(pauseDuration)
                         .AppendCallback(() => Debug.Log($"Layer {i} animation completed"));

            scrollSequences.Add(layerSequence);
        }
    }

    private void OnDisable()
    {
        // Kill all parallax sequences when disabled
        foreach (var seq in scrollSequences)
        {
            seq?.Kill();
        }
    }

    public void PauseScrolling()
    {
        foreach (var seq in scrollSequences)
        {
            seq?.Pause();
        }
    }

    public void ResumeScrolling()
    {
        foreach (var seq in scrollSequences)
        {
            seq?.Play();
        }
    }

    public void RestartScrolling()
    {
        CreateScrollSequence();
    }

    public void SetupChoose(LongScene scene)
    {
        bgImage.sprite = scene.sprite;
        longSceneGO.SetActive(true);
        //CreateScrollSequence();
        Debug.Log("Game Sequence Started");
    }

    public void PerformChoose(StoryScene scene)
    {
        gameController.PlayScene(scene);
        longSceneGO.SetActive(false);
    }
}
