using UnityEngine;
using TMPro;

public class SwingingText : MonoBehaviour
{
    public TMP_Text textComponent;
    public float swingMagnitude = 5.0f; // Maximum horizontal swing
    public float swingSpeed = 2.0f; // Speed of swinging motion

    private void Update()
    {
        textComponent.ForceMeshUpdate();
        var textInfo = textComponent.textInfo;
        float time = Time.time * swingSpeed;

        for (int i = 0; i < textInfo.characterCount; ++i)
        {
            var charInfo = textInfo.characterInfo[i];
            if (!charInfo.isVisible) continue;

            var verts = textInfo.meshInfo[charInfo.materialReferenceIndex].vertices;
            float swingOffset = Mathf.Sin(time + i * 0.2f) * swingMagnitude;

            for (int j = 0; j < 4; ++j)
            {
                verts[charInfo.vertexIndex + j] += new Vector3(swingOffset, 0, 0);
            }
        }

        for (int i = 0; i < textInfo.meshInfo.Length; ++i)
        {
            var meshInfo = textInfo.meshInfo[i];
            meshInfo.mesh.vertices = meshInfo.vertices;
            textComponent.UpdateGeometry(meshInfo.mesh, i);
        }
    }
}