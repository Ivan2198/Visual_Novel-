using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class BottomBarController : MonoBehaviour
{
    public TextMeshProUGUI barText;
    public TextMeshProUGUI personNameText;
    public AudioSource voicePlayer;
    public RectTransform rectTransform;
    public Image nameBubbleImage;

    public Image boxtext;



    private int sentenceIndex = -1;
    private StoryScene currentScene;
    private State state = State.COMPLETED;
    private Animator animator;
    private bool isHidden = false;

    public Dictionary<Speaker, SpriteController> sprites;
    public GameObject spritesPrefab;

    private Coroutine typingCoroutine;
    private float speedFactor = 1f;

    [SerializeField] private DialogueAudio dialogueAudio;
    public TMP_Animated animatedText;

    private enum State
    {
        PLAYING, SPEEDED_UP, COMPLETED
    }

    private void Start()
    {
        sprites = new Dictionary<Speaker, SpriteController>();
        animator = GetComponent<Animator>();
    }

    public int GetSentenceIndex()
    {
        return sentenceIndex;
    }

    public void SetSentenceIndex(int sentenceIndex)
    {
        this.sentenceIndex = sentenceIndex;
    }

    public void Hide()
    {
        if (!isHidden)
        {
            animator.SetTrigger("Hide");
            isHidden = true;
        }
    }

    public void Show()
    {
        animator.SetTrigger("Show");
        isHidden = false;
    }

    public void ClearText()
    {
        barText.text = "";
        personNameText.text = "";
    }

    public void PlayScene(StoryScene scene, int sentenceIndex = -1, bool isAnimated = true)
    {
        currentScene = scene;
        this.sentenceIndex = sentenceIndex;
        PlayNextSentence(isAnimated);
    }

    public void PlayNextSentence(bool isAnimated = true)
    {
        StopTyping(); // Stop any current audio before moving to next sentence
        sentenceIndex++;
        PlaySentence(isAnimated);
    }

    public void GoBack()
    {
        sentenceIndex--;
        StopTyping();
        HideSprites();
        PlaySentence(false);
    }

    public bool IsCompleted()
    {
        return state == State.COMPLETED || state == State.SPEEDED_UP;
    }

    public bool IsLastSentence()
    {
        return sentenceIndex + 1 == currentScene.sentences.Count;
    }
    public bool IsLastScene()
    {
        return currentScene.escenaFinal && sentenceIndex + 1 == currentScene.sentences.Count;
    }
    public int SceneTransition()
    {
        return currentScene.sceneTransition;
    }

    public bool IsFirstSentence()
    {
        return sentenceIndex == 0;
    }

    public void SpeedUp()
    {
        state = State.SPEEDED_UP;
        speedFactor = 0.25f;
    }

    public void StopTyping()
    {
        state = State.COMPLETED;
        if (voicePlayer != null)
        {
            voicePlayer.Stop();
        }
        if (dialogueAudio != null)
        {
            dialogueAudio.StopAllSounds();
        }
        // Removemos todos los listeners para evitar que se acumulen
        if (animatedText != null)
        {
            animatedText.onTextReveal.RemoveAllListeners();
            animatedText.onDialogueFinish.RemoveAllListeners();
        }
    }

    public void HideSprites()
    {
        while(spritesPrefab.transform.childCount > 0)
        {
            DestroyImmediate(spritesPrefab.transform.GetChild(0).gameObject);
        }
        sprites.Clear();
    }

    private void PlaySentence(bool isAnimated = true)
    {
        StoryScene.Sentence sentence = currentScene.sentences[sentenceIndex];

        // Configuración del hablante y visuales
        // Update only the x value
        Vector2 newPosition = rectTransform.anchoredPosition;
        newPosition.x = sentence.speaker.namePositionX;  // Set your desired x value
        rectTransform.anchoredPosition = newPosition;
        nameBubbleImage.color = sentence.speaker.color;

        personNameText.text = sentence.speaker.speakerName;
        personNameText.color = sentence.speaker.textColor;
        boxtext.sprite = sentence.speaker.boxtext;

        // Configuración del audio
        if (sentence.audio)
        {
            voicePlayer.clip = sentence.audio;
            voicePlayer.Play();
        }
        else
        {
            voicePlayer.Stop();
        }

        // Limpiamos los listeners anteriores antes de añadir nuevos
        animatedText.onTextReveal.RemoveAllListeners();
        animatedText.onDialogueFinish.RemoveAllListeners();

        // Añadimos los nuevos listeners
        animatedText.onTextReveal.AddListener((char c) => {
            dialogueAudio.ReproduceSound(c);
        });

        animatedText.onDialogueFinish.AddListener(() => {
            state = State.COMPLETED;
            dialogueAudio.StopAllSounds();
        });

        // Animamos el texto
        animatedText.ReadText(sentence.text);

        // Ejecutamos las acciones de los hablantes
        ActSpeakers(isAnimated);
    }

    private IEnumerator TypeText(string text)
    {
        barText.text = "";
        state = State.PLAYING;
        int wordIndex = 0;

        while (state != State.COMPLETED)
        {
            barText.text += text[wordIndex];
            yield return new WaitForSeconds(speedFactor * 0.05f);
            if(++wordIndex == text.Length)
            {
                state = State.COMPLETED;
                break;
            }
        }
    }

    private void ActSpeakers(bool isAnimated = true)
    {
        List<StoryScene.Sentence.Action> actions = currentScene.sentences[sentenceIndex].actions;
        for(int i = 0; i < actions.Count; i++)
        {
            ActSpeaker(actions[i], isAnimated);
        }
    }
    

    private void ActSpeaker(StoryScene.Sentence.Action action, bool isAnimated = true)
    {
        SpriteController controller;
        if (!sprites.ContainsKey(action.speaker))
        {
            controller = Instantiate(action.speaker.prefab.gameObject, spritesPrefab.transform)
                .GetComponent<SpriteController>();
            sprites.Add(action.speaker, controller);
        }
        else
        {
            controller = sprites[action.speaker];
        }
        switch (action.actionType)
        {
            case StoryScene.Sentence.Action.Type.APPEAR:
                controller.Setup(action.speaker.sprites[action.spriteIndex]);
                controller.Show(action.coords, isAnimated);
                return;
            case StoryScene.Sentence.Action.Type.MOVE:
                controller.Move(action.coords, action.moveSpeed, isAnimated);
                break;
            case StoryScene.Sentence.Action.Type.DISAPPEAR:
                controller.Hide(isAnimated);
                break;
        }
        controller.SwitchSprite(action.speaker.sprites[action.spriteIndex], isAnimated);
    }
}
