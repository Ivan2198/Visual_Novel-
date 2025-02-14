using UnityEngine;

public class CandleFlicker : MonoBehaviour
{
    public Material material;  
    public string colorProperty = "_EmissionColor"; 
    public Color baseColor = Color.yellow; 
    public float minIntensity = 1f;
    public float maxIntensity = 3f;
    public float flickerSpeed = 2f; 

    private float timeOffset;

    void Start()
    {
        if (material == null)
        {
            material = GetComponent<Renderer>().material;
        }
        timeOffset = Random.Range(0f, 100f); // Prevents all candles flickering the same
    }

    void Update()
    {
        float intensity = Mathf.Lerp(minIntensity, maxIntensity, Mathf.PingPong(Time.time * flickerSpeed + timeOffset, 1));
        material.SetColor(colorProperty, baseColor * intensity);
    }
}
