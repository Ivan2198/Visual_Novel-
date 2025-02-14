using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace PDollarGestureRecognizer
{
	public class Demo : MonoBehaviour
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

		void Start()
		{
			platform = Application.platform;

			// Construct the draw area with the background
			drawArea = new Rect(Screen.width - 800, Screen.height - 325, drawAreaWidth, drawAreaHeight);

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

                    // GameManager.instance.SetDrawingMode(true);
                    ++strokeId;

					Transform tmpGesture = Instantiate(gestureOnScreenPrefab, transform.position, transform.rotation) as Transform;
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
			}
        }

		void OnGUI()
		{
			
            // Create Texture2D for background of drawing area
            drawAreaBackground = new Texture2D(250, 250, TextureFormat.RGBA32, false);

            // Array of Color to set each pixel color
            var drawAreaBackgroundPixels = new Color[drawAreaBackground.width * drawAreaBackground.height];

            // Fill the array with the corresponding color
            for (int i = 0; i < drawAreaBackgroundPixels.Length; i++)
            {
                drawAreaBackgroundPixels[i] = new Color(0.8f, 0.8f, 0.8f, 0f);
            }

            // Set the pixel colors
            drawAreaBackground.SetPixels(drawAreaBackgroundPixels);
            drawAreaBackground.Apply();

            // Set the painted texture on a style
            var style = new GUIStyle();
            style.normal.background = drawAreaBackground;
			

            GUI.Box(drawArea, GUIContent.none, style);

            GUI.Label(new Rect(10, Screen.height - 40, 500, 50), message);

			/*

			if (GUI.Button(new Rect(Screen.width - 100, 10, 100, 30), "Recognize"))
			{
				recognized = true;

				Gesture candidate = new Gesture(points.ToArray());
				Result gestureResult = PointCloudRecognizer.Classify(candidate, trainingSet.ToArray());

				message = gestureResult.GestureClass + " " + gestureResult.Score;
			}
			*/

			GUI.Label(new Rect(Screen.width - 200, 150, 70, 30), "Add as: ");
			newGestureName = GUI.TextField(new Rect(Screen.width - 150, 150, 100, 30), newGestureName);

			if (GUI.Button(new Rect(Screen.width - 50, 150, 50, 30), "Add") && points.Count > 0 && newGestureName != "")
			{
				string fileName = String.Format("{0}/{1}-{2}.xml", Application.persistentDataPath, newGestureName, DateTime.Now.ToFileTime());

#if !UNITY_WEBPLAYER
				GestureIO.WriteGesture(points.ToArray(), newGestureName, fileName);
#endif

				trainingSet.Add(new Gesture(points.ToArray(), newGestureName));

				newGestureName = "";
			}
		}

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
	}
}