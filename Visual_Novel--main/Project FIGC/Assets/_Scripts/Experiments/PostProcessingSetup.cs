using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class PostProcessingSetup : MonoBehaviour
{
    [SerializeField] private LayerMask layerMask; // Assign in Inspector

    void Start()
    {
        //// Get the Post Process Layer component
        //var postProcessLayer = GetComponent<PostProcessLayer>();

        //if (postProcessLayer == null)
        //{
        //    Debug.LogError("PostProcessLayer component not found on this GameObject.");
        //    return;
        //}

        //// Remove the specified layers from volumeLayer using bitwise NOT
        //postProcessLayer.volumeLayer &= ~layerMask.value;
    }
}