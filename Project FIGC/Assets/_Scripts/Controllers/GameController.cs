using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public GameScene currentScene, skip;
    public BottomBarController bottomBar;
    public SpriteSwitcher backgroundController;
    public ChooseController chooseController;
    public AudioController audioController;
    public PauseMenu pauseMenu;

    //public DestinyController destinyController;
    
    public VideoController videoController;
    public GameSequenceController gameSequenceController;
    public LongSceneController longSceneController;

    [SerializeField] private GameObject dialogueAudioGO;

    public DataHolder data;

    public string menuScene;
    [SerializeField] string FinalScene;
    private bool estaOculto = false;

    private State state = State.IDLE;

    private List<StoryScene> history = new List<StoryScene>();

    private enum State
    {
        IDLE, ANIMATE, CHOOSE, VIDEO, GAME, LONGSCENE
    }

    void Start()
    {
        if (SaveManager.IsGameSaved())
        {
            SaveData data = SaveManager.LoadGame();
            data.prevScenes.ForEach(scene =>
            {
                history.Add(this.data.scenes[scene] as StoryScene);
            });
            currentScene = history[history.Count - 1];
            history.RemoveAt(history.Count - 1);
            bottomBar.SetSentenceIndex(data.sentence - 1);
        }
        if (currentScene is StoryScene)
        {
            StoryScene storyScene = currentScene as StoryScene;
            history.Add(storyScene);
            bottomBar.PlayScene(storyScene, bottomBar.GetSentenceIndex());
            backgroundController.SetImage(storyScene.background);
            PlayAudio(storyScene.sentences[bottomBar.GetSentenceIndex()]);
        }
    }

    void Update()
    {
        if (state == State.IDLE) 
        {
            if (Input.GetKeyDown(KeyCode.Space) || (Input.GetMouseButtonDown(0) && !DesactivarClick()))
            {
                if(bottomBar.IsLastScene())
                {
                    SaveManager.ClearSavedGame();
                    SceneManager.LoadScene(FinalScene);
                }else
                NextSentence();
                

            }
            if (Input.GetMouseButtonDown(1))
            {
                RewindSentence();
            }
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Debug.Log("Pausa");
                //SaveGame();
                pauseMenu.Pause();
                //SceneManager.LoadScene(menuScene);
            }

            //Opciones para ocultar el HUD
            if (Input.GetKeyDown(KeyCode.H))
            {
                bottomBar.Hide();
               
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                bottomBar.Show();
                
            }
            if (Input.GetKeyDown(KeyCode.N))
            {
                PlayScene(skip);
            }

            AudioToggle();

        }
    }

    public void PlayScene(GameScene scene, int sentenceIndex = -1, bool isAnimated = true)
    {
        StartCoroutine(SwitchScene(scene, sentenceIndex, isAnimated));
    }

    private IEnumerator SwitchScene(GameScene scene, int sentenceIndex = -1, bool isAnimated = true)
    {
        state = State.ANIMATE;
        currentScene = scene;
        if (isAnimated)
        {
            bottomBar.Hide();
            yield return new WaitForSeconds(1f);
        }
        if (scene is StoryScene)
        {
            StoryScene storyScene = scene as StoryScene;
            history.Add(storyScene);
            PlayAudio(storyScene.sentences[sentenceIndex + 1]);

           
            if (isAnimated)
            {
                backgroundController.SwitchImage(storyScene.background);
                yield return new WaitForSeconds(1f);

                switch (bottomBar.SceneTransition())
                {
                    case 0:
                        
                        break;
                    case 1:
                        backgroundController.ZoomIn();
                        break;
                    default:
                        break;
                }

                bottomBar.ClearText();
                bottomBar.Show();
                yield return new WaitForSeconds(1f);
            }
            else
            {
                backgroundController.SetImage(storyScene.background);
                bottomBar.ClearText();
            }
            bottomBar.PlayScene(storyScene, sentenceIndex, isAnimated);
            
            state = State.IDLE;
        }
        else if (scene is ChooseScene)
        {
            state = State.CHOOSE;
            chooseController.SetupChoose(scene as ChooseScene);
        }
        else if (scene is Videos)
        {
            state = State.VIDEO;
            videoController.SetupChoose(scene as Videos);
        }
        else if (scene is GameSequence) 
        {
            state = State.GAME;
            gameSequenceController.SetupChoose(scene as GameSequence);
        }
        else if (scene is LongScene)
        {
            state = State.LONGSCENE;
            longSceneController.SetupChoose(scene as LongScene);
        }
        
    }

    private void PlayAudio(StoryScene.Sentence sentence)
    {
        audioController.PlayAudio(sentence.music, sentence.sound);
    }
    public void NextSentence()
    {
        if (bottomBar.IsCompleted())
        {
            bottomBar.StopTyping();
            if (bottomBar.IsLastSentence())
            {
                PlayScene((currentScene as StoryScene).nextScene);
            }
            else
            {
                bottomBar.PlayNextSentence();
                PlayAudio((currentScene as StoryScene)
                    .sentences[bottomBar.GetSentenceIndex()]);
            }
        }
        else
        {
            bottomBar.SpeedUp();
        }
    }
    public void RewindSentence()
    {
        if (bottomBar.IsFirstSentence())
        {
            if (history.Count > 1)
            {
                bottomBar.StopTyping();
                bottomBar.HideSprites();
                history.RemoveAt(history.Count - 1);
                StoryScene scene = history[history.Count - 1];
                history.RemoveAt(history.Count - 1);
                PlayScene(scene, scene.sentences.Count - 2, false);
            }
        }
        else
        {
            bottomBar.GoBack();
        }
    }
    private void SaveGame()
    {
        List<int> historyIndicies = new List<int>();
        history.ForEach(scene =>
        {
            historyIndicies.Add(this.data.scenes.IndexOf(scene));
        });
        SaveData data = new SaveData
        {
            sentence = bottomBar.GetSentenceIndex(),
            prevScenes = historyIndicies
        };
        SaveManager.SaveGame(data);
    }

    //--------------------- Metodos Extras------------------------------------

    public void GuardarYSalir()
    {
        SaveGame();
        pauseMenu.LoadMenu(); // Para que despause
        SceneManager.LoadScene(menuScene);
    }
    public bool DesactivarClick()
    {
        return false;
    }

    private void AudioToggle()
    {
        if (Input.GetKeyUp(KeyCode.M) && !dialogueAudioGO.activeInHierarchy)
        {
            dialogueAudioGO.SetActive(true);
        }
        else if (Input.GetKeyUp(KeyCode.M) && dialogueAudioGO.activeInHierarchy)
        {
            dialogueAudioGO.SetActive(false);
        }
    }
}
