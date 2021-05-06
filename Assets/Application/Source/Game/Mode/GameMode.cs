using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game;

public class GameMode : MonoBehaviour
{
    public static GameMode Instance{get; protected set;}
    public string ModeName = "temp";

    virtual public void InputDir(Dir _dir){}
    virtual public void HakoDelete(Vector3 _pos, HakoType _type){}

    virtual public void HakoUpdate(){}

    virtual public Vector2Int TargetHakoPosition(){return -Vector2Int.one;}
}