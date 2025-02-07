using UnityEngine;

public class PlayerController : MonoBehaviour
{

    [SerializeField] private GameSequenceController gameSequenceController;
    public bool followOnX = true;
    public bool followOnY = true;
    public float zPosition = 0f;

    [SerializeField] private StoryScene scene;

    

    public float smoothSpeed = 5f;

    private Camera mainCamera;

    void Start()
    {

        mainCamera = Camera.main;
    }

    void Update()
    {
        // Get the mouse position in screen coordinates
        Vector3 mousePosition = Input.mousePosition;

  
        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(new Vector3(
            mousePosition.x,
            mousePosition.y,
            -mainCamera.transform.position.z
        ));


        Vector3 newPosition = new Vector3(
            followOnX ? worldPosition.x : transform.position.x,
            followOnY ? worldPosition.y : transform.position.y,
            zPosition
        );

        transform.position = Vector3.Lerp(transform.position, newPosition, smoothSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        gameSequenceController.PerformChoose(scene);
    }
}
