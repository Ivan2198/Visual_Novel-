using DG.Tweening;
using UnityEngine;

public class ParallaxController : MonoBehaviour
{
    [System.Serializable]
    public class ParallaxLayer
    {
        public Transform objectTransform;
        [Range(0f, 1f)]
        public float parallaxEffect = 0.5f;
        public bool infiniteHorizontal = false;
        public float textureWidth = 0f; // Only needed for infinite scrolling
    }

    [SerializeField] private ParallaxLayer[] parallaxLayers;
    [SerializeField] private float smoothing = 1f;

    private Transform cameraTransform;
    private Vector3 previousCameraPosition;
    private float[] startPositions;

    private void Start()
    {
        cameraTransform = Camera.main.transform;
        previousCameraPosition = cameraTransform.position;
        startPositions = new float[parallaxLayers.Length];

        // Store the starting x position of each layer
        for (int i = 0; i < parallaxLayers.Length; i++)
        {
            if (parallaxLayers[i].objectTransform != null)
            {
                startPositions[i] = parallaxLayers[i].objectTransform.position.x;
            }
        }
    }

    private void LateUpdate()
    {
        float deltaX = (cameraTransform.position.x - previousCameraPosition.x);
        float deltaY = (cameraTransform.position.y - previousCameraPosition.y);

        for (int i = 0; i < parallaxLayers.Length; i++)
        {
            ParallaxLayer layer = parallaxLayers[i];
            if (layer.objectTransform == null) continue;

            // Calculate parallax movement
            float parallaxX = -deltaX * layer.parallaxEffect;
            float parallaxY = -deltaY * layer.parallaxEffect;

            // Get the new position
            Vector3 newPos = layer.objectTransform.position;
            newPos.x += parallaxX;
            newPos.y += parallaxY;

            // Apply the new position with smoothing
            layer.objectTransform.position = Vector3.Lerp(
                layer.objectTransform.position,
                newPos,
                smoothing * Time.deltaTime
            );

            // Handle infinite scrolling if enabled
            if (layer.infiniteHorizontal && layer.textureWidth > 0)
            {
                float relativeDistance = cameraTransform.position.x * (1 - layer.parallaxEffect);
                if (relativeDistance > startPositions[i] + layer.textureWidth)
                {
                    startPositions[i] += layer.textureWidth;
                    layer.objectTransform.position = new Vector3(startPositions[i], layer.objectTransform.position.y, layer.objectTransform.position.z);
                }
                else if (relativeDistance < startPositions[i] - layer.textureWidth)
                {
                    startPositions[i] -= layer.textureWidth;
                    layer.objectTransform.position = new Vector3(startPositions[i], layer.objectTransform.position.y, layer.objectTransform.position.z);
                }
            }
        }

        previousCameraPosition = cameraTransform.position;
    }

    // Method to move camera with DOTween and maintain parallax
    public void MoveCameraTo(Vector3 targetPosition, float duration, Ease easeType = Ease.InOutQuad)
    {
        cameraTransform.DOMove(targetPosition, duration)
            .SetEase(easeType)
            .OnUpdate(() => {
                // The LateUpdate will handle the parallax effect automatically
            })
            .OnComplete(() => Debug.Log("Camera movement complete"));
    }
}
