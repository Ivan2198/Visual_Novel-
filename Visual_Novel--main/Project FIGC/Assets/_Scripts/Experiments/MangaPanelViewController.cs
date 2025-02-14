using UnityEngine;
using UnityEngine.UI;

public class MangaPanelViewController : MonoBehaviour
{
    [System.Serializable]
    public class PanelLayer
    {
        public RectTransform panelFrame;      // The frame/border of the manga panel
        public RectTransform contentLayer;     // The actual content/image inside
        [Range(0f, 1f)]
        public float parallaxStrength = 0.1f;  // How much this layer moves
        public Vector2 maxOffset = new Vector2(20f, 20f); // Maximum pixel offset
    }

    [Header("Panel Settings")]
    [SerializeField] private PanelLayer[] panels;
    [SerializeField] private float smoothSpeed = 10f;
    [SerializeField] private bool useScreenSpace = true;

    [Header("Optional Mask Settings")]
    [SerializeField] private bool useMask = true;
    [SerializeField] private RectTransform maskRect;

    private Vector2[] initialPositions;
    private Vector2 targetMousePos;
    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
        InitializePanels();
    }

    private void InitializePanels()
    {
        initialPositions = new Vector2[panels.Length];

        for (int i = 0; i < panels.Length; i++)
        {
            // Store initial positions
            initialPositions[i] = panels[i].contentLayer.anchoredPosition;

            // Setup mask if enabled
            if (useMask && maskRect != null)
            {
                // Add mask component to panel frame
                var mask = panels[i].panelFrame.gameObject.AddComponent<Mask>();
                mask.showMaskGraphic = true;
            }

            // Ensure content is larger than frame to allow for movement
            var contentRect = panels[i].contentLayer as RectTransform;
            var frameRect = panels[i].panelFrame as RectTransform;

            // Make content slightly larger than frame to prevent edge revealing
            contentRect.sizeDelta = frameRect.sizeDelta + panels[i].maxOffset * 2;
        }
    }

    private void Update()
    {
        // Get mouse position in correct space
        Vector2 mousePos;
        if (useScreenSpace)
        {
            mousePos = Input.mousePosition;
            targetMousePos = new Vector2(
                (mousePos.x / Screen.width) * 2 - 1,
                (mousePos.y / Screen.height) * 2 - 1
            );
        }
        else
        {
            // Use world space if needed
            mousePos = mainCamera.ScreenToViewportPoint(Input.mousePosition);
            targetMousePos = new Vector2(
                mousePos.x * 2 - 1,
                mousePos.y * 2 - 1
            );
        }

        UpdatePanels(targetMousePos);
    }

    private void UpdatePanels(Vector2 normalizedMousePos)
    {
        for (int i = 0; i < panels.Length; i++)
        {
            PanelLayer panel = panels[i];

            // Calculate offset based on mouse position and parallax strength
            Vector2 offset = new Vector2(
                normalizedMousePos.x * panel.maxOffset.x * panel.parallaxStrength,
                normalizedMousePos.y * panel.maxOffset.y * panel.parallaxStrength
            );

            // Calculate target position
            Vector2 targetPos = initialPositions[i] + offset;

            // Smoothly move to target position
            panel.contentLayer.anchoredPosition = Vector2.Lerp(
                panel.contentLayer.anchoredPosition,
                targetPos,
                Time.deltaTime * smoothSpeed
            );
        }
    }

    // Add or remove a panel at runtime
    public void AddPanel(RectTransform frame, RectTransform content, float parallaxStrength = 0.1f)
    {
        System.Array.Resize(ref panels, panels.Length + 1);
        System.Array.Resize(ref initialPositions, initialPositions.Length + 1);

        panels[panels.Length - 1] = new PanelLayer
        {
            panelFrame = frame,
            contentLayer = content,
            parallaxStrength = parallaxStrength,
            maxOffset = new Vector2(20f, 20f)
        };

        initialPositions[initialPositions.Length - 1] = content.anchoredPosition;
    }
}