using UnityEngine;
using System;

public class SpritesController : MonoBehaviour
{
    public static event Action<int> OnSpriteChange; // Event for sprite change

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            OnSpriteChange?.Invoke(0); // Trigger event for sprite 0
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            OnSpriteChange?.Invoke(1); // Trigger event for sprite 1
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            OnSpriteChange?.Invoke(2); // Trigger event for sprite 2
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            OnSpriteChange?.Invoke(3); // Trigger event for sprite 3
        }
    }
}