using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MusicData", menuName = "Yak/MusicData")]
public class MusicData : ScriptableObject
{
    public MusicContent[] contents;
    public int GetNumber(string _name)
    {
        for(int i = 0; i < contents.Length; i++)
        {
            if(contents[i].Name == _name){return i;}
        }
        Debug.Log("MusicContent:ありません。");
        return 0;
    }
}

[System.Serializable]
public class MusicContent
{
    public string Name = "";
    public AudioClip Clip;
    public float volume = 0.5f;
}