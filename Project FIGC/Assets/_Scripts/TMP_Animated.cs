using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace TMPro
{
    public enum Emotion { happy, sad, suprised, angry };
    [System.Serializable] public class EmotionEvent : UnityEvent<Emotion> { }
    [System.Serializable] public class ActionEvent : UnityEvent<string> { }
    [System.Serializable] public class TextRevealEvent : UnityEvent<char> { }
    [System.Serializable] public class DialogueEvent : UnityEvent { }

    public class TMP_Animated : TextMeshProUGUI
    {
        [SerializeField] private float speed = 10;
        public EmotionEvent onEmotionChange;
        public ActionEvent onAction;
        public TextRevealEvent onTextReveal;
        public DialogueEvent onDialogueFinish;

        private WobblyTextHover wobblyText;
        private List<(int startIndex, int endIndex)> wobbleRanges = new List<(int startIndex, int endIndex)>();
        private bool isWobbling = false;

        protected override void Awake()
        {
            base.Awake();
            wobblyText = gameObject.GetComponent<WobblyTextHover>();
            if (wobblyText == null)
            {
                wobblyText = gameObject.AddComponent<WobblyTextHover>();
                wobblyText.textComponent = this;
            }
        }

        private void Update()
        {
            if (isWobbling && wobbleRanges.Count > 0)
            {
                ForceMeshUpdate();
                
                //var textInfo = textInfo;

                for (int i = 0; i < textInfo.characterCount; ++i)
                {
                    var charInfo = textInfo.characterInfo[i];

                    if (!charInfo.isVisible)
                        continue;

                    bool shouldWobble = false;
                    foreach (var range in wobbleRanges)
                    {
                        if (i >= range.startIndex && i <= range.endIndex)
                        {
                            shouldWobble = true;
                            break;
                        }
                    }

                    if (!shouldWobble)
                        continue;

                    var verts = textInfo.meshInfo[charInfo.materialReferenceIndex].vertices;

                    for (int j = 0; j < 4; ++j)
                    {
                        var orig = verts[charInfo.vertexIndex + j];
                        verts[charInfo.vertexIndex + j] = orig + new Vector3(0, Mathf.Sin(Time.time * 2f + orig.x * 0.01f) * 10f, 0);
                    }
                }

                for (int i = 0; i < textInfo.meshInfo.Length; ++i)
                {
                    var meshInfo = textInfo.meshInfo[i];
                    meshInfo.mesh.vertices = meshInfo.vertices;
                    UpdateGeometry(meshInfo.mesh, i);
                }
            }
        }

        public void ReadText(string newText)
        {
            text = string.Empty;
            wobbleRanges.Clear();
            isWobbling = false;

            string[] subTexts = newText.Split('<', '>');
            string displayText = "";
            int currentIndex = 0;
            Color defaultColor = m_fontColor;

            for (int i = 0; i < subTexts.Length; i++)
            {
                if (i % 2 == 0)
                {
                    if (!string.IsNullOrEmpty(subTexts[i]))
                    {
                        displayText += subTexts[i];
                        currentIndex += subTexts[i].Length;
                    }
                }
                else if (!isCustomTag(subTexts[i].Replace(" ", "")))
                {
                    displayText += $"<{subTexts[i]}>";
                }
                else if (subTexts[i].StartsWith("wobble"))
                {
                    int startIndex = currentIndex;
                    // Find the closing tag
                    for (int j = i + 1; j < subTexts.Length; j++)
                    {
                        if (j % 2 == 0)
                        {
                            displayText += subTexts[j];
                            currentIndex += subTexts[j].Length;
                        }
                        else if (subTexts[j] == "/wobble")
                        {
                            wobbleRanges.Add((startIndex, currentIndex - 1));
                            i = j;
                            break;
                        }
                    }
                }
                else if (subTexts[i].StartsWith("fontcolor="))
                {
                    string[] colorValues = subTexts[i].Split('=')[1].Split(',');
                    Color textColor;

                    if (colorValues.Length >= 3)
                    {
                        float r = float.Parse(colorValues[0]) / 255f;
                        float g = float.Parse(colorValues[1]) / 255f;
                        float b = float.Parse(colorValues[2]) / 255f;
                        float a = colorValues.Length == 4 ? float.Parse(colorValues[3]) / 255f : 1f;
                        textColor = new Color(r, g, b, a);
                    }
                    else
                    {
                        textColor = defaultColor;
                    }

                    displayText += $"<color=#{ColorUtility.ToHtmlStringRGBA(textColor)}>";

                    // Find the closing fontcolor tag
                    for (int j = i + 1; j < subTexts.Length; j++)
                    {
                        if (j % 2 == 0)
                        {
                            displayText += subTexts[j];
                            currentIndex += subTexts[j].Length;
                        }
                        else if (subTexts[j] == "/fontcolor")
                        {
                            displayText += "</color>";
                            i = j;
                            break;
                        }
                    }
                }
            }

            bool isCustomTag(string tag)
            {
                return tag.StartsWith("speed=") ||
                       tag.StartsWith("pause=") ||
                       tag.StartsWith("emotion=") ||
                       tag.StartsWith("action=") ||
                       tag.StartsWith("fontcolor=") ||
                       tag == "/fontcolor" ||
                       tag.StartsWith("wobble") ||
                       tag == "/wobble";
            }

            text = displayText;
            maxVisibleCharacters = 0;
            StartCoroutine(Read());

            IEnumerator Read()
            {
                int subCounter = 0;
                int visibleCounter = 0;
                while (subCounter < subTexts.Length)
                {
                    if (subCounter % 2 == 1)
                    {
                        yield return EvaluateTag(subTexts[subCounter].Replace(" ", ""));
                    }
                    else
                    {
                        while (visibleCounter < subTexts[subCounter].Length)
                        {
                            onTextReveal.Invoke(subTexts[subCounter][visibleCounter]);
                            visibleCounter++;
                            maxVisibleCharacters++;
                            yield return new WaitForSeconds(1f / speed);
                        }
                        visibleCounter = 0;
                    }
                    subCounter++;
                }
                yield return null;

                if (wobbleRanges.Count > 0)
                {
                    isWobbling = true;
                }

                onDialogueFinish.Invoke();
            }

            WaitForSeconds EvaluateTag(string tag)
            {
                if (tag.Length > 0)
                {
                    if (tag.StartsWith("speed="))
                    {
                        speed = float.Parse(tag.Split('=')[1]);
                    }
                    else if (tag.StartsWith("pause="))
                    {
                        return new WaitForSeconds(float.Parse(tag.Split('=')[1]));
                    }
                    else if (tag.StartsWith("emotion="))
                    {
                        onEmotionChange.Invoke((Emotion)System.Enum.Parse(typeof(Emotion), tag.Split('=')[1]));
                    }
                    else if (tag.StartsWith("action="))
                    {
                        onAction.Invoke(tag.Split('=')[1]);
                    }
                }
                return null;
            }
        }
    }
}