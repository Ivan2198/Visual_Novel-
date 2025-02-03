using System;
using System.Collections;
using UnityEngine;

public class SpriteChanger : MonoBehaviour
{
    public SpriteRenderer spriteRenderer; // Reference to the SpriteRenderer
    public Sprite[] sprites; // Array of sprites to switch between


    

    private void OnEnable()
    {
        SpritesController.OnSpriteChange += ChangeSprite;
    }

    private void OnDisable()
    {
        SpritesController.OnSpriteChange -= ChangeSprite;
    }




    private void ChangeSprite(int spriteIndex)
    {
        if (spriteIndex >= 0 && spriteIndex < sprites.Length)
        {
            spriteRenderer.sprite = sprites[spriteIndex];
        }
        else
        {
            Debug.LogWarning("Invalid sprite index received!");
        }
    }

}