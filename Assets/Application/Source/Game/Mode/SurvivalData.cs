using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{

    [CreateAssetMenu(fileName = "SurvivalData", menuName = "HAKOIRO/SurvivalData")]
    public class SurvivalData : ScriptableObject
    {
        public float CameraSize = 4f;
        public Vector2Int Size = new Vector2Int(5, 5);
        public int MaxHakos = 8;
        public float[] AppearTimes =
            {100.0f, 3.5f, 2.5f, 2.0f, 1.8f, 1.6f, 1.4f, 1.3f, 1.2f, 1.1f, 1.0f, 0.9f, 0.8f, 0.7f, 0.6f, 0.5f, 0.4f, 0.3f, 0.2f, 0.1f};
        public int[] LevelUpScore =
            {1, 3, 7, 11, 17, 23, 31, 41, 53, 67, 83, 97, 113, 131, 149, 173, 199, 227, 263, 307};
        public int[] ComboScores =
            {1, 2, 3, 4, 5, 6, 7, 8, 9, 10};

    }
}
