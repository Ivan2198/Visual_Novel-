using TMPro;
using UnityEngine;

public class WobblyTextHover : MonoBehaviour
{
    public TMP_Text textComponent;
    private bool isWobbling = false;
    private void Update()
    {
        if (isWobbling)
        {
            textComponent.ForceMeshUpdate();

            var textInfo = textComponent.textInfo;

            for (int i = 0; i < textInfo.characterCount; ++i)
            {
                var charInfo = textInfo.characterInfo[i];

                if (!charInfo.isVisible)
                {
                    continue;
                }

                var verts = textInfo.meshInfo[charInfo.materialReferenceIndex].vertices;

                for (int j = 0; j < 4; ++j)
                {
                    var orig = verts[charInfo.vertexIndex + j];
                    verts[charInfo.vertexIndex + j] = orig + new Vector3(0, Mathf.Sin(Time.time * 2f + orig.x * 0.01f) * 10f, 0);
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
    public void StartWobbling(bool a_isWobbling)
    {
        isWobbling = a_isWobbling;
    }
}
