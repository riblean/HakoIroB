using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SEData", menuName = "Yak/SEData")]
public class SEData : ScriptableObject
{
    public SEContent[] contents;
    public int GetNumber(string _name)
    {
        for(int i = 0; i < contents.Length; i++)
        {
            if(contents[i].Name == _name){return i;}
        }
        Debug.Log("SE:ありません。");
        return 0;
    }
}

[System.Serializable]
public class SEContent
{
    public string Name = "";
    public AudioClip Clip;
    public float volume = 0.5f;
    public float startTime = 0.0f;
}