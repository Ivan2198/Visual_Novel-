using System.Collections;
using UnityEngine;

public class MaterialLerp : MonoBehaviour
{
    public Material targetMaterial;
    public Color colorA = Color.red;
    public Color colorB = Color.blue;
    public float intensityA = 1f;
    public float intensityB = 2f;
    public float duration = 2f;

    private void Start()
    {
        //if (targetMaterial != null)
        //{
        //    StartCoroutine(LerpMaterial());
        //}
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            DoTheLerp();
        } 
    }
    private IEnumerator LerpMaterial()
    {
        yield return StartCoroutine(LerpColor(colorA, colorB, intensityA, intensityB));
        yield return StartCoroutine(LerpColor(colorB, colorA, intensityB, intensityA));
    }

    private IEnumerator LerpColor(Color startColor, Color endColor, float startIntensity, float endIntensity)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            Color lerpedColor = Color.Lerp(startColor, endColor, t);
            float lerpedIntensity = Mathf.Lerp(startIntensity, endIntensity, t);

            targetMaterial.SetColor("_Color", lerpedColor * lerpedIntensity);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        targetMaterial.SetColor("_Color", endColor * endIntensity);
    }

    public void DoTheLerp()
    {
        StartCoroutine(LerpMaterial());
    }
}