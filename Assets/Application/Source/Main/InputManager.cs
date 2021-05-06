using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance{ get; private set; }
    public string XAxisName = "Horizontal";
    // KeyDownと長押しを検知。プラマイ両方使う。
    float xAxisPressedTime = 0;
    public string YAxisName = "Vertical";
    float yAxisPressedTime = 0;
    public float LongPressTime = 0.3f;
    public float LongPressCycleTime = 0.1f;

    float mouseLeftPressedTime = 0;
    public bool DebugMode = false;

    void Start()
    {
        if(!Instance){Instance = this;}
    }

    void Update()
    {
        if(Input.GetAxisRaw(YAxisName) > 0)
        {
            if(yAxisPressedTime == 0){InputDir(Dir.up);}
            if(yAxisPressedTime > LongPressTime){
                InputDir(Dir.up);
                yAxisPressedTime -= LongPressCycleTime;
            }
            yAxisPressedTime += Time.deltaTime;
        }
        else if(Input.GetAxisRaw(YAxisName) < 0)
        {
            if(yAxisPressedTime == 0){InputDir(Dir.down);}
            if(yAxisPressedTime < -LongPressTime){
                InputDir(Dir.down);
                yAxisPressedTime += LongPressCycleTime;
            }
            yAxisPressedTime -= Time.deltaTime;
        }
        else
        {
            yAxisPressedTime = 0;
        }

        if(Input.GetAxisRaw(XAxisName) > 0)
        {
            if(xAxisPressedTime == 0){InputDir(Dir.right);}
            if(xAxisPressedTime > LongPressTime){
                InputDir(Dir.right);
                xAxisPressedTime -= LongPressCycleTime;
            }
            xAxisPressedTime += Time.deltaTime;
        }
        else if(Input.GetAxisRaw(XAxisName) < 0)
        {
            if(xAxisPressedTime == 0){InputDir(Dir.left);}
            if(xAxisPressedTime < -LongPressTime){
                InputDir(Dir.left);
                xAxisPressedTime += LongPressCycleTime;
            }
            xAxisPressedTime -= Time.deltaTime;
        }
        else
        {
            xAxisPressedTime = 0;
        }

        if(GameMode.Instance)
        {
            if(Input.GetKey(KeyCode.Mouse0))
            {
                if(mouseLeftPressedTime == 0 || mouseLeftPressedTime > LongPressTime)
                {

                    // マウスクリック位置を追尾。
                    Vector2Int _target = GameMode.Instance.TargetHakoPosition();
                    if(_target.x >= 0)
                    {
                        Vector3 _mouse = Input.mousePosition;
                        _mouse.z = 1;
                        Vector2 _dir = Game.HakoManaer.Instance.MapPosition(Camera.main.ScreenToWorldPoint(_mouse)) - _target;

                        if(Mathf.Abs(_dir.x) > Mathf.Abs(_dir.y))
                        {
                            if(_dir.x > 0.5f)
                            {
                                InputDir(Dir.right);
                            }
                            else if(_dir.x < -0.5f)
                            {
                                InputDir(Dir.left);
                            }
                        }
                        else
                        {
                            if(_dir.y > 0.5f)
                            {
                                InputDir(Dir.up);
                            }
                            else if(_dir.y < -0.5f)
                            {
                                InputDir(Dir.down);
                            }
                        }
                    }
                }
                if(mouseLeftPressedTime > LongPressTime){mouseLeftPressedTime -= LongPressCycleTime;}
                mouseLeftPressedTime += Time.deltaTime;
            }
            else
            {
                mouseLeftPressedTime = 0;
            }
        }

        if(Input.GetKeyDown(KeyCode.F3)){DebugMode = !DebugMode;}
    }

    void InputDir(Dir _dir)
    {
        if(TitleManager.Instance)
        {
            TitleManager.Instance.InputDir(_dir);
            SoundManager.SEPlay("move");
        }
        if(GameMode.Instance && !Yak.GameManager.Instance.OpenMenu)
        {
            GameMode.Instance.InputDir(_dir);
        }
    }
    public static Dir GetIntToDir(int _number)
    {
        return (Dir)Enum.ToObject(typeof(Dir), _number);
    }
}

public enum Dir
{
    up,
    right,
    down,
    left
}