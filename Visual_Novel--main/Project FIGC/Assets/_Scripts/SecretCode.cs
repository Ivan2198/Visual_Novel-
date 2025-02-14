using UnityEngine;
using UnityEngine.Video;

public class SecretCode : MonoBehaviour
{
    [SerializeField] private GameObject targetGameObject; // The GameObject to activate
    [SerializeField] private GameObject targetGameObject2; // The GameObject to activate
    [SerializeField] private GameObject targetGameObject3; // The GameObject to activate

    [SerializeField] private VideoPlayer videoPlayer;     // The VideoPlayer component
    [SerializeField] private string secretCode = "fofe";  // The secret code to trigger activation
    private string currentInput = "";                     // Stores the current user input

    void Start()
    {
        // Ensure the target GameObject is initially inactive
        if (targetGameObject != null)
            targetGameObject.SetActive(false);
    }

    void Update()
    {
        // Listen for user keyboard input
        foreach (char c in Input.inputString)
        {
            // Add the character to the current input
            currentInput += c;

            // Check if the current input matches the secret code
            if (currentInput.Equals(secretCode))
            {
                ActivateSecret();
                currentInput = ""; // Reset the input after activation
            }
            else if (currentInput.Length > secretCode.Length)
            {
                // Reset input if it exceeds the secret code length
                currentInput = c.ToString();
            }
        }
    }

    void ActivateSecret()
    {
        if (targetGameObject != null)
        {
            targetGameObject.SetActive(true); // Activate the GameObject
            targetGameObject2.SetActive(false); // Activate the GameObject
            targetGameObject3.SetActive(false);
        }

        if (videoPlayer != null)
        {
            videoPlayer.Play(); // Start playing the video
        }

        Debug.Log("Secret code entered! GameObject activated and video started.");
    }
}
