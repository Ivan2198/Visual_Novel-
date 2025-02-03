using UnityEngine;
using UnityEngine.Video;

public class GameSequenceController : MonoBehaviour
{
    public GameController gameController;
    [SerializeField] private GameObject gameSequenceGO;

    public void SetupChoose(GameSequence scene)
    {
        gameSequenceGO.SetActive(true);
        Debug.Log("Game Sequence");
    }

    public void PerformChoose(StoryScene scene)
    {
        gameController.PlayScene(scene);
        gameSequenceGO.SetActive(false);
    }
}
