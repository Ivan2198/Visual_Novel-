using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewGameSequence", menuName = "Data/New Game Sequence")]
[System.Serializable]
public class GameSequence : GameScene
{
    public List<ChooseLabel> labels;

    [System.Serializable]
    public struct ChooseLabel
    {
        public StoryScene nextScene;
    }
}
