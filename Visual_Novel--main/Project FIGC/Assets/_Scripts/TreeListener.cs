using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TreeSpriteChanger : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;

    // Sprites para cada estado
    [SerializeField] private Sprite macetaSprite;
    [SerializeField] private Sprite broteSprite;
    [SerializeField] private Sprite plantaSprite;
    [SerializeField] private Sprite arbolSprite;

    [SerializeField] private Sprite bgSprite,bgSprite2;
    [SerializeField] private SpriteRenderer bgSpriteRenderer;

    [SerializeField] private float _duration;

    // Método para conectar al evento OnChange
    public void ChangeSprite(TreeStateMachine.State newState)
    {
        switch (newState)
        {
            case TreeStateMachine.State.Maceta:
                spriteRenderer.sprite = macetaSprite;
                break;

            case TreeStateMachine.State.Brote:
                spriteRenderer.sprite = broteSprite;
                break;

            case TreeStateMachine.State.Planta:
                CrossfadeSprites(bgSprite, bgSprite2, _duration);
                spriteRenderer.sprite = plantaSprite;
                break;

            case TreeStateMachine.State.Arbol:
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
                break;

            default:
                Debug.LogWarning("Estado no reconocido");
                break;
        }

        Debug.Log($"Sprite cambiado al estado: {newState}");
    }


    private void CrossfadeSprites(Sprite currentSprite, Sprite newSprite, float duration)
    {
        StartCoroutine(CrossfadeCoroutine(currentSprite, newSprite, duration));
    }

    private IEnumerator CrossfadeCoroutine(Sprite currentSprite, Sprite newSprite, float duration)
    {
        Sprite oldSprite = currentSprite;
        float elapsedTime = 0f;

        // Set the new sprite immediately
        bgSpriteRenderer.sprite = newSprite;

        // Fade in the new sprite and fade out the old sprite
        Color startColor = new Color(1f, 1f, 1f, 0f);  // Start with fully transparent
        Color endColor = new Color(1f, 1f, 1f, 1f);    // End with fully opaque

        bgSpriteRenderer.color = startColor;

        // Fade from transparent to opaque over duration
        while (elapsedTime < duration)
        {
            bgSpriteRenderer.color = Color.Lerp(startColor, endColor, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure it is fully opaque at the end
        bgSpriteRenderer.color = endColor;
    }
}
