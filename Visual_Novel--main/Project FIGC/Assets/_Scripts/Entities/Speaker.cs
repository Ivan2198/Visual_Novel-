using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

[CreateAssetMenu(fileName = "NewSpeaker", menuName = "Data/New Speaker")]
[System.Serializable]
public class Speaker : ScriptableObject
{
    public string speakerName;
    public Color nameBubbleColor;
    public Color textColor;
    public Sprite boxtext;
    public TextAlignmentOptions aling;
    public float namePositionX;

    public List<Sprite> sprites;
    public SpriteController prefab;
}
