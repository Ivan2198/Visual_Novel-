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

        [SerializeField] private MaterialPropertyLerp materialPropertyLerp;


        //GameObject for rezising the drawArea
        // public RectTransform drawAreaCanvas;

        //Unity Events
        public UnityEvent OnXRecognized;
        [Range(0f, 3f)]
        [SerializeField] private float timeToRecognize;
        [SerializeField] private GameSequenceController gameSequenceController;


        // Scenes to transition
        [SerializeField] private StoryScene sceneEnojo;
        [SerializeField] private StoryScene sceneFelicidad;
        [SerializeField] private StoryScene sceneSorpresa;
        [SerializeField] private StoryScene sceneTristeza;
        [SerializeField] private StoryScene sceneFelicidadSorpresa;

        void Start()
        {
            platform = Application.platform;

            // Set draw area to cover the right half of the screen
            drawAreaWidth = Screen.width / 2;
            drawAreaHeight = Screen.height;

            // Position it to start from the middle of the screen
            float x = Screen.width / 2;
            float y = 0;
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

                    Transform tmpGesture = Instantiate(gestureOnScreenPrefab, transform.position, transform.rotation);
                    currentGestureLineRenderer = tmpGesture.GetComponent<LineRenderer>();

                    gestureLinesRenderer.Add(currentGestureLineRenderer);

                    vertexCount = 0;
                }

                if (Input.GetMouseButton(0))
                {
                    points.Add(new Point(virtualKeyPosition.x, -virtualKeyPosition.y, strokeId));

                    currentGestureLineRenderer.positionCount = ++vertexCount;
                    currentGestureLineRenderer.SetPosition(vertexCount - 1, Camera.main.ScreenToWorldPoint(new Vector3(virtualKeyPosition.x, virtualKeyPosition.y, 10)));
                }

                // Start the recognition timer when the player releases the mouse button
                if (Input.GetMouseButtonUp(0) && points.Count > 0)
                {
                    StopAllCoroutines(); // Prevent multiple timers running
                    StartCoroutine(DelayedRecognition());
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
                    case "enojo":
                        Debug.Log("Recognized: enojo");
                        OnXRecognized?.Invoke();
                        materialPropertyLerp.DoMaterialLerp();
                        gameSequenceController.PerformChoose(sceneEnojo);
                        break;

                    case "felicidad":
                        Debug.Log("Recognized: felicidad");
                        OnXRecognized?.Invoke();
                        materialPropertyLerp.DoMaterialLerp();
                        gameSequenceController.PerformChoose(sceneFelicidad);
                        break;

                    case "sorpresa":
                        Debug.Log("Recognized: sorpresa");
                        OnXRecognized?.Invoke();
                        materialPropertyLerp.DoMaterialLerp();
                        gameSequenceController.PerformChoose(sceneSorpresa);
                        break;

                    case "tristeza":
                        Debug.Log("Recognized: tristeza");
                        OnXRecognized?.Invoke();
                        materialPropertyLerp.DoMaterialLerp();
                        gameSequenceController.PerformChoose(sceneTristeza);
                        break;

                    case "felicidad_sorpresa":
                        Debug.Log("Recognized: felicidad_sorpresa");
                        OnXRecognized?.Invoke();
                        materialPropertyLerp.DoMaterialLerp();
                        gameSequenceController.PerformChoose(sceneFelicidadSorpresa);
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

            float x = Screen.width / 2f;
            float y = 0f;

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
