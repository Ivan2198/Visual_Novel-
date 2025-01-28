using UnityEngine;
using UnityEngine.Rendering.Universal; // Required for Light2D

public class DayNightCycle : MonoBehaviour
{
    [SerializeField] private SpriteRenderer dayNightOverlay; // Assign the SpriteRenderer in the Inspector
    [SerializeField] private float cycleDuration = 10f; // Full cycle duration in seconds

    private float cycleTimer = 0f;
    private bool isNight = false;

    [SerializeField] private GameObject sun; // GameObject representing the Sun
    [SerializeField] private GameObject moon; // GameObject representing the Moon

    [SerializeField] private float orbitRadius = 5f; // Radius of the orbit around the screen center

    [SerializeField] private DragAndDrop dragAndDrop;

    [SerializeField] private Light2D globalLight; // Assign your Global Light 2D component here
    [SerializeField] private float minIntensity = 0.5f; // Minimum intensity during the night
    [SerializeField] private float maxIntensity = 1f; // Maximum intensity during the day

    [SerializeField] private Transform _center;
    private void Update()
    {
        // Increment the cycle timer based on time
        if (dragAndDrop.isActive)
        {
            cycleTimer += Time.deltaTime;
        }

        // Calculate the normalized progress of the cycle (0 to 1)
        float normalizedCycle = cycleTimer / cycleDuration;

        // Calculate the Sun and Moon positions
        float angleSun = normalizedCycle * 360f; // Full rotation for Sun
        float angleMoon = angleSun + 180f; // Moon is always opposite to the Sun

        Vector3 center = _center.position;

        sun.transform.position = center + new Vector3(Mathf.Cos(angleSun * Mathf.Deg2Rad), Mathf.Sin(angleSun * Mathf.Deg2Rad), 0) * orbitRadius;
        moon.transform.position = center + new Vector3(Mathf.Cos(angleMoon * Mathf.Deg2Rad), Mathf.Sin(angleMoon * Mathf.Deg2Rad), 0) * orbitRadius;

        // Calculate progress in the current half of the cycle (0 to 5 or 5 to 10)
        float halfCycleProgress = (cycleTimer % (cycleDuration / 2)) / (cycleDuration / 2);

        // Adjust opacity for day-to-night and night-to-day transitions
        if (cycleTimer <= cycleDuration / 2) // Day to Night
        {
            isNight = false;
            SetOpacity(halfCycleProgress); // From 0 to 1
            AdjustLightIntensity(1 - halfCycleProgress); // Reduce light intensity
        }
        else // Night to Day
        {
            isNight = true;
            SetOpacity(1 - halfCycleProgress); // From 1 to 0
            AdjustLightIntensity(halfCycleProgress); // Increase light intensity
        }

        // Reset the timer when it completes a full cycle
        if (cycleTimer >= cycleDuration)
        {
            cycleTimer = 0f;
        }
    }

    private void SetOpacity(float alpha)
    {
        Color color = dayNightOverlay.color;
        color.a = alpha;
        dayNightOverlay.color = color;
    }

    private void AdjustLightIntensity(float progress)
    {
        // Lerp between maxIntensity and minIntensity based on progress
        globalLight.intensity = Mathf.Lerp(minIntensity, maxIntensity, progress);
    }
}