using UnityEngine;

public class RainbowLineRenderer : MonoBehaviour
{
    private LineRenderer lineRenderer;
    [SerializeField] private float colorSpeed = 0.5f;
    [SerializeField] private float saturation = 1f;
    [SerializeField] private float brightness = 1f;

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        if (lineRenderer == null)
        {
            Debug.LogError("No LineRenderer component found!");
            enabled = false;
            return;
        }
    }

    private void Update()
    {
        UpdateRainbowGradient();
    }

    private void UpdateRainbowGradient()
    {
        Gradient gradient = new Gradient();
        GradientColorKey[] colorKeys = new GradientColorKey[6];
        GradientAlphaKey[] alphaKeys = new GradientAlphaKey[2];

        // Calculate time-based offset for color cycling
        float timeOffset = Time.time * colorSpeed;

        // Create six color keys for smooth rainbow transition
        for (int i = 0; i < 6; i++)
        {
            float hue = (i / 6f + timeOffset) % 1f;
            Color color = Color.HSVToRGB(hue, saturation, brightness);
            colorKeys[i] = new GradientColorKey(color, i / 5f);
        }

        // Set alpha keys (fully opaque)
        alphaKeys[0] = new GradientAlphaKey(1.0f, 0.0f);
        alphaKeys[1] = new GradientAlphaKey(1.0f, 1.0f);

        gradient.SetKeys(colorKeys, alphaKeys);
        lineRenderer.colorGradient = gradient;
    }
}