using DG.Tweening;
using UnityEngine;


public class CameraMovement : MonoBehaviour
{
    [SerializeField] private float moveDuration = 2f;
    [SerializeField] private Vector3 targetPosition;
    [SerializeField] private Ease easeType = Ease.InQuad;

    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            MoveCameraToTarget();
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            ResetCamera();
        }
    }

    public void MoveCameraToTarget()
    {
        mainCamera.transform
            .DOMove(targetPosition, moveDuration)
            .SetEase(easeType)
            .OnComplete(() => Debug.Log("Camera movement complete"));
    }


    public void MoveCameraTo(Vector3 newPosition, float duration)
    {
        mainCamera.transform
            .DOMove(newPosition, duration)
            .SetEase(easeType)
            .OnComplete(() => Debug.Log("Camera movement complete"));
    }

    public void StopCameraMovement()
    {
        mainCamera.transform.DOKill();
    }

    private void ResetCamera()
    {
        mainCamera.transform.position = new Vector3(0, 0, -10);
    }
}
