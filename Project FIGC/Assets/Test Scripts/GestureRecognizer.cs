using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine.Events;

namespace PDollarGestureRecognizer
{
    public class GestureRecognizer : MonoBehaviour
    {
        // Width and height of Draw Area
        private int drawAreaWidth = 250;
        private int drawAreaHeight = 250;

        public Transform gestureOnScreenPrefab;

        private List<Gesture> trainingSet = new List<Gesture>();

        private List<Point> points = new List<Point>();
        private int strokeId = -1;

        private Vector3 virtualKeyPosition = Vector2.zero;
        private Rect drawArea;

        private RuntimePlatform platform;
        private int vertexCount = 0;

        private List<LineRenderer> gestureLinesRenderer = new List<LineRenderer>();
        private LineRenderer currentGestureLineRenderer;

        //GUI
        private string message;
        private bool recognized = false;
        private string newGestureName = "";
        private Texture2D drawAreaBackground;
        private int texturePixelCount;


        //GameObject for rezising the drawArea
        // public RectTransform drawAreaCanvas;

        //Unity Events
        public UnityEvent OnXRecognized;
        [Range(0f, 3f)]
        [SerializeField] private float timeToRecognize;
        [SerializeField] private GameSequenceController gameSequenceController;
        [SerializeField] private StoryScene scene;

        void Start()
        {
            platform = Application.platform;

            // Set draw area to half the screen width and height, centered
            drawAreaWidth = Screen.width / 2;
            drawAreaHeight = Screen.height / 2;

            // Center the draw area
            float x = (Screen.width - drawAreaWidth) / 2;
            float y = (Screen.height - drawAreaHeight) / 2;
            drawArea = new Rect(x, y, drawAreaWidth, drawAreaHeight);

            //Load pre-made gestures
            TextAsset[] gesturesXml = Resources.LoadAll<TextAsset>("GestureSet/10-stylus-MEDIUM/");
            foreach (TextAsset gestureXml in gesturesXml)
                trainingSet.Add(GestureIO.ReadGestureFromXML(gestureXml.text));

            //Load user custom gestures
            string[] filePaths = Directory.GetFiles(Application.persistentDataPath, "*.xml");
            foreach (string filePath in filePaths)
                trainingSet.Add(GestureIO.ReadGestureFromFile(filePath));
        }

        void Update()
        {

            if (platform == RuntimePlatform.Android || platform == RuntimePlatform.IPhonePlayer)
            {
                if (Input.touchCount > 0)
                {
                    virtualKeyPosition = new Vector3(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y);
                }
            }
            else
            {
                if (Input.GetMouseButton(0))
                {
                    virtualKeyPosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y);
                }
            }

            if (drawArea.Contains(virtualKeyPosition))
            {
                if (Input.GetMouseButtonDown(0))
                {
                    if (recognized)
                    {
                        recognized = false;
                        strokeId = -1;
                        points.Clear();

                        foreach (LineRenderer lineRenderer in gestureLinesRenderer)
                        {
                            lineRenderer.positionCount = 0;
                            Destroy(lineRenderer.gameObject);
                        }

                        gestureLinesRenderer.Clear();
                    }

                    ++strokeId;

                    Transform tmpGesture = Instantiate(gestureOnScreenPrefab, transform.position, transform.rotation) as Transform;
                    currentGestureLineRenderer = tmpGesture.GetComponent<LineRenderer>();

                    gestureLinesRenderer.Add(currentGestureLineRenderer);

                    vertexCount = 0;

                    // Start the coroutine when the first stroke begins
                    StopAllCoroutines(); // Prevent multiple timers running
                    StartCoroutine(DelayedRecognition());
                }

                if (Input.GetMouseButton(0))
                {
                    points.Add(new Point(virtualKeyPosition.x, -virtualKeyPosition.y, strokeId));

                    currentGestureLineRenderer.positionCount = ++vertexCount;
                    currentGestureLineRenderer.SetPosition(vertexCount - 1, Camera.main.ScreenToWorldPoint(new Vector3(virtualKeyPosition.x, virtualKeyPosition.y, 10)));
                }
            }
        }

//        void OnGUI()
//        {
//            // Scale GUI based on 1920x1080 resolution
//            float scaleX = Screen.width / 1920f;
//            float scaleY = Screen.height / 1080f;

//            // Create background texture matching the new size
//            drawAreaBackground = new Texture2D(drawAreaWidth, drawAreaHeight, TextureFormat.RGBA32, false);
//            var drawAreaBackgroundPixels = new Color[drawAreaBackground.width * drawAreaBackground.height];
//            for (int i = 0; i < drawAreaBackgroundPixels.Length; i++)
//            {
//                drawAreaBackgroundPixels[i] = new Color(0.8f, 0.8f, 0.8f, 0.1f); // Made slightly visible
//            }
//            drawAreaBackground.SetPixels(drawAreaBackgroundPixels);
//            drawAreaBackground.Apply();

//            var style = new GUIStyle();
//            style.normal.background = drawAreaBackground;

//            GUI.Box(drawArea, GUIContent.none, style);

//            // Scale and position UI elements relative to screen size
//            float buttonWidth = 100 * scaleX;
//            float buttonHeight = 30 * scaleY;
//            float margin = 10 * scaleX;

//            // Recognition button - top right
//            if (GUI.Button(new Rect(Screen.width - buttonWidth - margin, margin,
//                buttonWidth, buttonHeight), "Recognize"))
//            {
//                recognized = true;
//                Gesture candidate = new Gesture(points.ToArray());
//                Result gestureResult = PointCloudRecognizer.Classify(candidate, trainingSet.ToArray());
//                message = gestureResult.GestureClass + " " + gestureResult.Score;
//            }

//            // Message label - bottom
//            GUI.Label(new Rect(margin, Screen.height - 40 * scaleY,
//                500 * scaleX, 50 * scaleY), message);

//            // Add gesture controls - right side
//            float rightSideX = Screen.width - 200 * scaleX;
//            float rightSideY = 150 * scaleY;

//            GUI.Label(new Rect(rightSideX, rightSideY,
//                70 * scaleX, buttonHeight), "Add as: ");

//            newGestureName = GUI.TextField(new Rect(rightSideX + 70 * scaleX, rightSideY,
//                100 * scaleX, buttonHeight), newGestureName);

//            if (GUI.Button(new Rect(rightSideX + 180 * scaleX, rightSideY,
//                50 * scaleX, buttonHeight), "Add") && points.Count > 0 && newGestureName != "")
//            {
//                string fileName = String.Format("{0}/{1}-{2}.xml",
//                    Application.persistentDataPath, newGestureName, DateTime.Now.ToFileTime());

//#if !UNITY_WEBPLAYER
//                GestureIO.WriteGesture(points.ToArray(), newGestureName, fileName);
//#endif
//                trainingSet.Add(new Gesture(points.ToArray(), newGestureName));
//                newGestureName = "";
//            }
//        }

        public void TryRecognizeGesture()
        {
            recognized = true;

            Gesture candidate = new Gesture(points.ToArray());
            Result gestureResult = PointCloudRecognizer.Classify(candidate, trainingSet.ToArray());

            message = gestureResult.GestureClass + " " + gestureResult.Score;

            Debug.Log(message);

            if (gestureResult.Score > 0.8f)
            {
                switch (gestureResult.GestureClass)
                {
                    case "asterisk":
                        Debug.Log("Recognized: Asterisk");
                        break;

                    case "N":
                        Debug.Log("Recognized: N");
                        break;

                    case "T":
                        Debug.Log("Recognized: T");
                        break;

                    case "D":
                        Debug.Log("Recognized: D");
                        break;

                    case "P":
                        Debug.Log("Recognized: P");
                        break;

                    case "X":
                        Debug.Log("Recognized: X");
                        
                        break;

                    case "H":
                        Debug.Log("Recognized: H");
                        OnXRecognized?.Invoke();
                        gameSequenceController.PerformChoose(scene);
                        break;

                    case "I":
                        Debug.Log("Recognized: I");
                        break;

                    case "half note":
                        Debug.Log("Recognized: Half note");
                        break;

                    default:
                        break;
                }
            }
        }

        public void ClearLine()
        {
            recognized = false;
            strokeId = -1;

            points.Clear();

            foreach (LineRenderer lineRenderer in gestureLinesRenderer)
            {
                lineRenderer.positionCount = 0;
                Destroy(lineRenderer.gameObject);
            }

            gestureLinesRenderer.Clear();
        }

        private IEnumerator DelayedRecognition()
        {
            yield return new WaitForSeconds(timeToRecognize); // Wait for 3 seconds
            TryRecognizeGesture();
            ClearLine();
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;

            // Calculate centered draw area corners
            float x = (Screen.width - drawAreaWidth) / 2f;
            float y = (Screen.height - drawAreaHeight) / 2f;

            Vector3 bottomLeft = Camera.main.ScreenToWorldPoint(new Vector3(x, y, 10));
            Vector3 bottomRight = Camera.main.ScreenToWorldPoint(new Vector3(x + drawAreaWidth, y, 10));
            Vector3 topLeft = Camera.main.ScreenToWorldPoint(new Vector3(x, y + drawAreaHeight, 10));
            Vector3 topRight = Camera.main.ScreenToWorldPoint(new Vector3(x + drawAreaWidth, y + drawAreaHeight, 10));

            Gizmos.DrawLine(bottomLeft, bottomRight);
            Gizmos.DrawLine(bottomRight, topRight);
            Gizmos.DrawLine(topRight, topLeft);
            Gizmos.DrawLine(topLeft, bottomLeft);
        }
    }

}
