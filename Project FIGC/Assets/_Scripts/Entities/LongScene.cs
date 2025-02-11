using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewLongScene", menuName = "Data/New LongScene")]
[System.Serializable]
public class LongScene : GameScene
{

    public List<ChooseLabel> labels;

    [System.Serializable]
    public struct ChooseLabel
    {
        public StoryScene nextScene;
    }
}
