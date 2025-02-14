using UnityEngine;

public class ParallaxScript : MonoBehaviour
{
    [System.Serializable]
    public class ParallaxLayer
    {
        public RectTransform layerTransform; // UI Layer
        public float parallaxAmount = 10f;  // Movement intensity
    }

    public ParallaxLayer[] layers;
    public RectTransform canvasRect;

    private Vector2 canvasSize;
    private Vector2 center;

    void Start()
    {
        canvasSize = canvasRect.sizeDelta;
        center = canvasSize / 2;
    }

    void Update()
    {
        Vector2 mousePosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, Input.mousePosition, null, out mousePosition);

        foreach (var layer in layers)
        {
            if (layer.layerTransform != null)
            {
                Vector2 offset = (mousePosition - center) / canvasSize * layer.parallaxAmount;
                layer.layerTransform.anchoredPosition = offset;
            }
        }
    }
}