using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "DressupData", menuName = "HAKOIRO/DressupData")]
    public class DressupData : ScriptableObject
    {
        public BackgroundScale BackgroundPrefab;
        public HakoAnimation X_Prefab;
        public HakoAnimation A_Prefab;
        public HakoAnimation B_Prefab;
        public HakoAnimation C_Prefab;
        public HakoAnimation D_Prefab;
        public HakoAnimation E_Prefab;

        [TextArea(1, 10)]
        public string Comment = "";

        public HakoAnimation GetHakoPrefab(HakoType _type)
        {
            switch((int)_type)
            {
                case 0: return X_Prefab;
                case 1: return A_Prefab;
                case 2: return B_Prefab;
                case 3: return C_Prefab;
                case 4: return D_Prefab;
            }
            return E_Prefab;
        }
    }
}
