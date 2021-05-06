using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance{get; private set;}

    public SEData SEData;
    public MusicData MusicData;
    [SerializeField] int SourceNumber = 6;
    [SerializeField] AudioSource[] Sources;

    public float MasterVolume = 0.1f;
    
    IEnumerator Start()
    {
        if(!Instance){Instance = this;}

        Sources = new AudioSource[SourceNumber];
        for(int i = 0; i < Sources.Length; i++)
        {
            yield return null;
            Sources[i] = gameObject.AddComponent<AudioSource>();
        }
    }

    void Update()
    {
        
    }

    public static void SEPlay(string _name, float _pitch = 1.0f)
    {
        if(Instance)
        {
            int _source = Instance.GetScourceNum();
            if(_source < 0){Debug.Log("Sourceが足りません。"); return;}
            int _clip = Instance.SEData.GetNumber(_name);
            Instance.Sources[_source].clip = Instance.SEData.contents[_clip].Clip;
            Instance.Sources[_source].Play();
            Instance.Sources[_source].volume = Instance.MasterVolume * Instance.SEData.contents[_clip].volume;
            Instance.Sources[_source].time = Instance.SEData.contents[_clip].startTime;
            Instance.Sources[_source].pitch = _pitch;
        }
    }

    public static void MusicPlay(string _name)
    {
        if(Instance)
        {
            int _source = Instance.GetScourceNum();
            if(_source < 0){Debug.Log("Sourceが足りません。"); return;}
            int _clip = Instance.MusicData.GetNumber(_name);
            Instance.Sources[_source].clip = Instance.MusicData.contents[_clip].Clip;
            Instance.Sources[_source].Play();
            Instance.Sources[_source].volume = Instance.MasterVolume * Instance.MusicData.contents[_clip].volume;
            Instance.Sources[_source].loop = true;
        }
    }

    int GetScourceNum()
    {
        for(int i = 0; i < Sources.Length; i++)
        {
            if(!Sources[i].isPlaying){return i;}
        }
        return -1;
    }
}
