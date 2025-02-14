using System.Collections;
using UnityEngine;

public class GameSequenceController : MonoBehaviour
{
    public GameController gameController;
    [SerializeField] private GameObject gameSequenceGO;
    [SerializeField] private GameObject gameSequenceCanvas;

    [SerializeField] private Animator animator;

    [Header("Timer")]
    public float timeRemaining = 2f;
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
                gameSequenceCanvas.SetActive(true);
                animator.CrossFade("Show", 0, 0);
                state = GameStates.PLAY;
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
        StartCoroutine(ChangeScene());
        gameController.PlayScene(scene);
    }

    public void StartTimer()
    {
        timeRemaining = 5f;
        isTimerRunning = true;
    }

    private void TimerCompleted()
    {
        Debug.Log("Timer Completed!");
    }
    private  IEnumerator ChangeScene()
    {
        state = GameStates.HIDE;
        yield return new WaitForSeconds(2f);
        animator.CrossFade("Hide", 10, 0);
        gameSequenceCanvas.SetActive(false);
    }

}
