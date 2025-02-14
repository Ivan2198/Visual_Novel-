using UnityEngine;
using System.Collections;

public class MaterialPropertyLerp : MonoBehaviour
{
    public Material targetMaterial;
    public string propertyName = "_Area";
    public float valueA;
    public float valueB;
    public float duration = 2f; 

    private void Start()
    {
       targetMaterial.SetFloat(propertyName, -100);
    }

    private IEnumerator ChangeMaterialProperty()
    {
        while (true)
        {
            yield return LerpMaterialProperty(valueA, valueB, duration);
            yield return LerpMaterialProperty(valueB, valueA, duration);
        }
    }

    private IEnumerator LerpMaterialProperty(float startValue, float endValue, float time)
    {
        float elapsedTime = 0f;
        while (elapsedTime < time)
        {
            elapsedTime += Time.deltaTime;
            float newValue = Mathf.Lerp(startValue, endValue, elapsedTime / time);
            targetMaterial.SetFloat(propertyName, newValue);
            yield return null;
        }
    }

    public void DoMaterialLerp()
    {
        if (targetMaterial != null)
        {
            StartCoroutine(LerpMaterialProperty(valueA, valueB, duration));
        }
        else
        {
            Debug.LogError("No material assigned!");
        }
    }
}
