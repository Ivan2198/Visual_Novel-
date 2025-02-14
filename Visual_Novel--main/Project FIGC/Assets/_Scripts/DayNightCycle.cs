using UnityEngine;
using UnityEngine.Rendering.Universal; 

public class DayNightCycle : MonoBehaviour
{
    [SerializeField] private SpriteRenderer dayNightOverlay; 
    [SerializeField] private float cycleDuration = 10f; 

    private float cycleTimer = 0f;
    private bool isNight = false;

    [SerializeField] private GameObject sun; 
    [SerializeField] private GameObject moon; 

    [SerializeField] private float orbitRadius = 5f;

    [SerializeField] private DragAndDrop dragAndDrop;

    [SerializeField] private Light2D globalLight; 
    [SerializeField] private float minIntensity = 0.5f; 
    [SerializeField] private float maxIntensity = 1f; 

    [SerializeField] private Transform _center;
    private void Update()
    {
        if (dragAndDrop.isActive)
        {
            cycleTimer += Time.deltaTime;
        }


        float normalizedCycle = cycleTimer / cycleDuration;

        float angleSun = normalizedCycle * 360f; 
        float angleMoon = angleSun + 180f; 

        Vector3 center = _center.position;

        sun.transform.position = center + new Vector3(Mathf.Cos(angleSun * Mathf.Deg2Rad), Mathf.Sin(angleSun * Mathf.Deg2Rad), 0) * orbitRadius;
        moon.transform.position = center + new Vector3(Mathf.Cos(angleMoon * Mathf.Deg2Rad), Mathf.Sin(angleMoon * Mathf.Deg2Rad), 0) * orbitRadius;

  
        float halfCycleProgress = (cycleTimer % (cycleDuration / 2)) / (cycleDuration / 2);

        
        if (cycleTimer <= cycleDuration / 2) 
        {
            isNight = false;
            SetOpacity(halfCycleProgress); 
            AdjustLightIntensity(1 - halfCycleProgress); 
        }
        else 
        {
            isNight = true;
            SetOpacity(1 - halfCycleProgress); 
            AdjustLightIntensity(halfCycleProgress); 
        }

       
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

        globalLight.intensity = Mathf.Lerp(minIntensity, maxIntensity, progress);
    }
}