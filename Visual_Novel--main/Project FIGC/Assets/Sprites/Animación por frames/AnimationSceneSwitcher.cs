using UnityEngine;
using UnityEngine.SceneManagement;

public class AnimationSceneSwitcher : MonoBehaviour
{
    public string sceneToLoad;
    [SerializeField] private float timeTotal;
    private float timer = 0f; // Keeps track of elapsed time

    private void Update()
    {
        timer += Time.deltaTime; // Accumulate time

        if (timer >= timeTotal) // Check if timer exceeds threshold
        {
            SceneManager.LoadScene(sceneToLoad);
        }
    }
}
