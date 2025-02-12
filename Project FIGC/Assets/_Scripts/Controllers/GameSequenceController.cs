using UnityEngine;

public class GameSequenceController : MonoBehaviour
{
    public GameController gameController;
    [SerializeField] private GameObject gameSequenceGO;

    [Header("Timer")]
    public float timeRemaining = 5f;
    private bool isTimerRunning = false;

    public enum GameStates
    {
        SHOW,
        PLAY,
        HIDE,
    }

    public GameStates state;

    private void Update()
    {
        if (isTimerRunning)
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
            }
            else
            {
                timeRemaining = 0;
                isTimerRunning = false;
                TimerCompleted(); // Handle state transition here
            }
        }

        switch (state)
        {
            case GameStates.SHOW:
                Debug.Log("Game state: SHOW");
                if (!isTimerRunning) // Start timer only if it's not running
                {
                    StartTimer();
                }
                break;
            case GameStates.PLAY:
                Debug.Log("Game state: PLAY");
                gameSequenceGO.SetActive(true);
                break;
            case GameStates.HIDE:
                Debug.Log("Game state: HIDE");
                gameSequenceGO.SetActive(false);
                break;
        }
    }

    public void SetupChoose(GameSequence scene)
    {
        state = GameStates.SHOW;
        Debug.Log("Game Sequence Started");
    }

    public void PerformChoose(StoryScene scene)
    {
        gameController.PlayScene(scene);
        gameSequenceGO.SetActive(false);
        state = GameStates.HIDE;
    }

    public void StartTimer()
    {
        timeRemaining = 5f;
        isTimerRunning = true;
    }

    private void TimerCompleted()
    {
        Debug.Log("Timer Completed!");
        state = GameStates.PLAY; // Transition to PLAY state after timer ends
    }
}
