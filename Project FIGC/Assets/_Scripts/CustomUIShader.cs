// CustomUIShader.cs
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasRenderer))]
public class CustomUIShader : Graphic
{
    [SerializeField] private Material shaderGraphMaterial;
    private Material instanceMaterial;

    protected override void OnEnable()
    {
        base.OnEnable();
        if (shaderGraphMaterial != null)
        {
            instanceMaterial = new Material(shaderGraphMaterial);
            material = instanceMaterial;
        }
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        if (instanceMaterial != null)
        {
            if (Application.isPlaying)
                Destroy(instanceMaterial);
            else
                DestroyImmediate(instanceMaterial);
        }
    }

    public override void SetMaterialDirty()
    {
        base.SetMaterialDirty();
        if (shaderGraphMaterial != null && instanceMaterial != null)
        {
            instanceMaterial.CopyPropertiesFromMaterial(shaderGraphMaterial);
        }
    }

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();

        Vector2 corner1 = Vector2.zero;
        Vector2 corner2 = Vector2.right * rectTransform.rect.width;
        Vector2 corner3 = Vector2.one * new Vector2(rectTransform.rect.width, rectTransform.rect.height);
        Vector2 corner4 = Vector2.up * rectTransform.rect.height;

        Color32 color32 = color;
        vh.AddVert(corner1, color32, new Vector2(0f, 0f));
        vh.AddVert(corner2, color32, new Vector2(1f, 0f));
        vh.AddVert(corner3, color32, new Vector2(1f, 1f));
        vh.AddVert(corner4, color32, new Vector2(0f, 1f));

        vh.AddTriangle(0, 1, 2);
        vh.AddTriangle(2, 3, 0);
    }
}