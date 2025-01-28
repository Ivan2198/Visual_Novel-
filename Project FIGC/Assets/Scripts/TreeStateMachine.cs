using UnityEngine;
using System.Collections;
using System;
using UnityEngine.Events;

public class TreeStateMachine : MonoBehaviour
{
    [SerializeField] private DragAndDrop dragAndDrop;

    [SerializeField] private float _duration;
    private float _timer = 0;
    public enum State
    {
        Maceta,
        Brote,
        Planta,
        Arbol
    }

    public State currentState = State.Brote;

    public static event Action OnProcessComplete;

    [Serializable]
    public class StateChangeEvent : UnityEvent<State> { }
    public StateChangeEvent OnChange;

    private void Start()
    {
        currentState = State.Maceta;
    }

    private void Update()
    {
        if (dragAndDrop.isActive) 
        {
            switch(currentState){
                case State.Maceta:
                    
                    _timer += Time.deltaTime;

                    if(_timer > _duration)
                    {
                        _timer = 0;

                        currentState = State.Brote;
                        OnProcessComplete?.Invoke();
                        OnChange?.Invoke(currentState);
                    }
                break;
                case State.Brote:
                    _timer += Time.deltaTime;

                    if (_timer > _duration)
                    {
                        _timer = 0;

                        currentState = State.Planta;
                        OnProcessComplete?.Invoke();
                        OnChange?.Invoke(currentState);
                    }

                    break;
                case State.Planta:
                    _timer += Time.deltaTime;

                    if (_timer > _duration)
                    {
                        _timer = 0;

                        currentState = State.Arbol;
                        OnProcessComplete?.Invoke();
                        OnChange?.Invoke(currentState);
                    }
                    break;
                case State.Arbol:

                    break;
                default:

                    break;
            }
        }
    }


    public float GetCooldownTimer()
    {
        return 1 - (_timer / _duration);
    }
}
