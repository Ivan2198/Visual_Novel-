using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

[CreateAssetMenu(fileName = "NewVideo", menuName = "Data/New Video")]
[System.Serializable]
public class Videos : GameScene
{
    public List<ChooseLabel> labels;

    [System.Serializable]
    public struct ChooseLabel
    {
        public VideoClip _videoClip;
        public StoryScene nextScene;
    }
}
