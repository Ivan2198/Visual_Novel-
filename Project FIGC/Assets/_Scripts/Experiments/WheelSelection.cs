using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Events;

[System.Serializable]
public class WheelOption
{
    public string itemName;
    public Sprite icon;
    public UnityEvent onSelect;
}
public class WheelSelection : MonoBehaviour
{
    [Header("Wheel Settings")]
    [SerializeField] private float wheelRadius = 200f;
    [SerializeField] private List<WheelOption> menuItems = new List<WheelOption>();
    [SerializeField] private GameObject wheelItemPrefab;
    [SerializeField] private float activationThreshold = 0.3f;

    [Header("Visual Settings")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color highlightColor = Color.yellow;

    private List<GameObject> wheelButtons = new List<GameObject>();
    private bool isWheelActive;
    private int currentSelection = -1;
    private RectTransform rectTransform;
    private Canvas parentCanvas;
    private Vector2 initialMousePosition;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        parentCanvas = GetComponentInParent<Canvas>();
        CreateWheel();
        gameObject.SetActive(false);
    }

    private void CreateWheel()
    {
        float angleStep = 360f / menuItems.Count;

        for (int i = 0; i < menuItems.Count; i++)
        {
            float angle = i * angleStep;
            Vector2 position = GetPositionOnCircle(angle);

            GameObject buttonObj = Instantiate(wheelItemPrefab, transform);
            RectTransform buttonRect = buttonObj.GetComponent<RectTransform>();
            buttonRect.anchoredPosition = position;

            Image buttonImage = buttonObj.GetComponent<Image>();
            buttonImage.sprite = menuItems[i].icon;

            wheelButtons.Add(buttonObj);
            buttonRect.rotation = Quaternion.Euler(0, 0, angle);
        }
    }

    private Vector2 GetPositionOnCircle(float angleDegrees)
    {
        float angleRadians = angleDegrees * Mathf.Deg2Rad;
        float x = Mathf.Sin(angleRadians) * wheelRadius;
        float y = Mathf.Cos(angleRadians) * wheelRadius;
        return new Vector2(x, y);
    }

    private void Update()
    {
        if (!isWheelActive && (Input.GetKeyDown(KeyCode.Tab)))
        {
            ShowWheel();
        }
        else if (isWheelActive && (Input.GetKeyUp(KeyCode.Tab)))
        {
            HideWheel();
            if (currentSelection >= 0 && currentSelection < menuItems.Count)
            {
                menuItems[currentSelection].onSelect?.Invoke();
            }
        }

        if (isWheelActive)
        {
            UpdateSelection();
        }
    }

    private void UpdateSelection()
    {
        Vector2 currentMousePos = Input.mousePosition;
        Vector2 direction = currentMousePos - initialMousePosition;

        if (direction.magnitude > activationThreshold)
        {
            float angle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;
            if (angle < 0) angle += 360f;

            int newSelection = Mathf.RoundToInt(angle / (360f / menuItems.Count)) % menuItems.Count;

            if (newSelection != currentSelection)
            {
                UpdateHighlight(newSelection);
            }
        }
    }

    private void UpdateHighlight(int newSelection)
    {
        if (currentSelection >= 0 && currentSelection < wheelButtons.Count)
        {
            wheelButtons[currentSelection].GetComponent<Image>().color = normalColor;
        }

        currentSelection = newSelection;
        wheelButtons[currentSelection].GetComponent<Image>().color = highlightColor;
    }

    private void ShowWheel()
    {
        isWheelActive = true;
        gameObject.SetActive(true);
        currentSelection = -1;

        // Store initial mouse position
        initialMousePosition = Input.mousePosition;

        // Position wheel at mouse
        if (parentCanvas.renderMode == RenderMode.ScreenSpaceOverlay)
        {
            rectTransform.position = initialMousePosition;
        }
        else if (parentCanvas.renderMode == RenderMode.ScreenSpaceCamera)
        {
            Vector2 pos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                parentCanvas.transform as RectTransform,
                initialMousePosition,
                parentCanvas.worldCamera,
                out pos);
            rectTransform.localPosition = pos;
        }

        Time.timeScale = 0.3f;
    }

    private void HideWheel()
    {
        isWheelActive = false;
        gameObject.SetActive(false);
        Time.timeScale = 1f;
    }
}
