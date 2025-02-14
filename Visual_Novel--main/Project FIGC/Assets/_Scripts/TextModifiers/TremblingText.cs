using UnityEngine;
using TMPro;

public class TremblingText : MonoBehaviour
{
    public TMP_Animated textComponent;
    public float tremblingMagnitude = 1.0f;

    private bool isTrembling = false;

    private void Start()
    {
        StartTrembling();
    }
    public void StartTrembling()
    {
        isTrembling = true;
    }

    public void StopTrembling()
    {
        isTrembling = false;
    }

    private void Update()
    {
        if (!isTrembling) return;

        textComponent.ForceMeshUpdate();
        var textInfo = textComponent.textInfo;

        for (int i = 0; i < textInfo.characterCount; ++i)
        {
            var charInfo = textInfo.characterInfo[i];
            if (!charInfo.isVisible) continue;

            var verts = textInfo.meshInfo[charInfo.materialReferenceIndex].vertices;
            for (int j = 0; j < 4; ++j)
            {
                var offset = Random.insideUnitCircle * tremblingMagnitude;
                verts[charInfo.vertexIndex + j] += new Vector3(offset.x, offset.y, 0);
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
