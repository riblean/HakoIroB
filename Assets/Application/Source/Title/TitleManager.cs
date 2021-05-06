using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Yak;
using System;

public class TitleManager : MonoBehaviour
{
    public static TitleManager Instance{get; private set;}
    public RectTransform TitleRoot;
    [SerializeField] TitleState state;
    [SerializeField] float changeTime = 1;
    [SerializeField] float changeSpeed = 0.5f;

    [SerializeField] Text subTitle;

    public GameObject[] GameStarterPrefab;


    void Start()
    {
        ScreenCurtain.Instance.SetColor(ScreenCurtain.State.Open);
        if(!Instance){Instance = this;}

        subTitle.gameObject.SetActive(false);
        subTitle.text = "ばーじょん：" + Application.version;
    }

    void Update()
    {
        if(!InputManager.Instance.DebugMode)
        {
            subTitle.gameObject.SetActive(false);
        }
    }

    public void ChangeState(TitleState _state)
    {
        StopAllCoroutines();
        if(state == TitleState.Score){ScoreManager.Instance.Obj.SetActive(false);}
        switch(_state)
        {
            case TitleState.Root:
                StartCoroutine(ChangeStateAnimation(new Vector2(0, 0)));
                break;
            case TitleState.PlaySelect:
                StartCoroutine(ChangeStateAnimation(new Vector2(0, -Screen.height)));
                break;
            case TitleState.Score:
                StartCoroutine(ChangeStateAnimation(new Vector2(-Screen.width, 0)));
                ScoreManager.Instance.Open("Survival05x5");
                break;
            case TitleState.Dressup:
                StartCoroutine(ChangeStateAnimation(new Vector2(Screen.width, 0)));
                break;
            case TitleState.Option:
                StartCoroutine(ChangeStateAnimation(new Vector2(0, Screen.height)));
                break;
        }
        state = _state;
    }

    public void InputDir(Dir _dir)
    {
        switch(state)
        {
            case TitleState.Root:
            if(_dir == Dir.up)
            {
                ChangeState(TitleState.PlaySelect);
            }
            if(_dir == Dir.down)
            {
                ChangeState(TitleState.Option);
            }
            if(_dir == Dir.left)
            {
                ChangeState(TitleState.Dressup);
            }
            if(_dir == Dir.right)
            {
                ChangeState(TitleState.Score);
            }
            break;
            case TitleState.PlaySelect:
                if(_dir == Dir.down)
                {
                    ChangeState(TitleState.Root);
                }
                break;
            case TitleState.Score:
                if(_dir == Dir.left)
                {
                    ChangeState(TitleState.Root);
                }
                break;
            case TitleState.Dressup:
                if(_dir == Dir.right)
                {
                    ChangeState(TitleState.Root);
                }
                break;
            case TitleState.Option:
                if(_dir == Dir.up)
                {
                    ChangeState(TitleState.Root);
                }
                break;
        }
    }

    IEnumerator ChangeStateAnimation(Vector2 _pos)
    {
        Vector3 _start = TitleRoot.anchoredPosition;
        float _startTime = Time.time;
        while(Time.time - _startTime < changeTime)
        {
            TitleRoot.anchoredPosition += (_pos - TitleRoot.anchoredPosition) * changeSpeed;
            yield return null;
        }
        TitleRoot.anchoredPosition = _pos;
    }

    public void GameStartButton(int _stage = 0)
    {
        Instantiate(GameStarterPrefab[_stage]);
    }

    public void ChangeStateButton(string _name)
    {
        ChangeState((TitleState)Enum.Parse(typeof(TitleState), _name));
    }
}

public enum TitleState
{
    Root,
    PlaySelect,
    Score,
    Option,
    Dressup
}