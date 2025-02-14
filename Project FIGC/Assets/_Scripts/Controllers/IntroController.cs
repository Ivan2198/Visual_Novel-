using UnityEngine;

public class IntroController : MonoBehaviour
{
    public GameController gameController;
    public void SetupChoose(Intro scene)
    {
    
        Debug.Log("Intro Started");
    }

    public void PerformChoose(StoryScene scene)
    {
        gameController.PlayScene(scene);
    }
}
