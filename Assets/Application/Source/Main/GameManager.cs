using System.Collections;
using UnityEngine;
using System;

namespace Yak
{
    // GameApplication全体の進行／共通処理
    // 2021 03 02
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance{get; private set;}
        // デフォルトステイと。途中再生する場合、ここを変更しておく。
        [SerializeField]State gameState = State.Start;
        [SerializeField]string titleSceneName = "Title";

        [Header("Menu")]
        bool isOpenMenu = true;
        [SerializeField] GameObject MenuObject;
        public KeyCode OpenMenuKey = KeyCode.Escape;
        public KeyCode ResetKey = KeyCode.F1;
        public KeyCode ExitKey = KeyCode.F2;

        void Awake()
        {
            if(Instance)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
        }

        void OnGUI()
        {
            // テスト用。
            // if(GameState == State.MainMenu && Yak.SettingManager.Data.PlayCount == 1)
            // {
            //     GUI.TextField(new Rect(16, 16, 116, 32), "初回起動です。:" + Yak.DataManager.Instance.FilePath);
            // }
            // data.UserName = GUI.TextField(new Rect(16, 16, 116, 66), data.UserName);
        }

        IEnumerator Start()
        {
            OpenMenu = false;
            yield return null;
            StateStart(gameState);
            SoundManager.MusicPlay("");
        }

        void Update()
        {
            if(Input.GetKeyDown(OpenMenuKey))
            {
                OpenMenu = !OpenMenu;
            }
            if(Input.GetKeyDown(ResetKey))
            {
                Reset();
            }
            if(Input.GetKeyDown(ExitKey))
            {
                Exit();
            }
            if(Input.GetKeyDown(KeyCode.F12))
            {
                string _name = DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".png";
                ScreenCapture.CaptureScreenshot("Assets/.Data/" + _name);
                Debug.Log("ScreenShot:" + _name);
            }
            StateUpdate(gameState);
        }

        // ApplicationMenu関連
        public bool OpenMenu
        {
            get{return isOpenMenu;}
            set{
                if(isOpenMenu != value)
                {
                    isOpenMenu = value;
                    MenuObject.SetActive(value);
                }
            }
        }

        public void CloseMenu(bool _b = false)
        {
            OpenMenu = _b;
        }

        public void Reset()
        {
            StartCoroutine(ResetStream());
        }

        IEnumerator ResetStream()
        {
            yield return Selecter.Instance.AsyncAnswer("タイトルに戻りますか?");
            if(Selecter.Instance.Answer == 1)
            {
                GameState = State.MainMenu;
                OpenMenu = false;
            }
        }

        public void Exit()
        {
            StartCoroutine(ExitStream());
        }

        IEnumerator ExitStream()
        {
            yield return Selecter.Instance.AsyncAnswer("ゲームを終了しますか?");
            if(Selecter.Instance.Answer == 1)
            {
                Debug.Log("Quit");
                Application.Quit();
            }
        }

        // State 場面ごとのスクリプト。必要ない。
        public enum State
        {
            Start,
            MainMenu,
            Game
        }

        public State GameState
        {
            get{return gameState;}
            set
            {
                if(gameState != value)
                {
                    StateEnd(gameState);
                    gameState = value;
                    StateStart(value);
                }
            }
        }

        void StateStart(State _state)
        {
            switch(gameState)
            {
                case State.Start:
                    GameState = State.MainMenu;
                break;
                case State.MainMenu:
                    LoadManager.Instance.LoadScene(titleSceneName);
                break;
                case State.Game:
                break;
            }
        }

        void StateUpdate(State _state)
        {
            switch(gameState)
            {
                case State.Start:
                break;
                case State.MainMenu:
                break;
                case State.Game:
                break;
            }
        }

        void StateEnd(State _state)
        {
            switch(gameState)
            {
                case State.MainMenu:
                break;
                case State.Game:
                break;
            }
        }
    }
}