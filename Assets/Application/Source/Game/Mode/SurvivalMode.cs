using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yak;

namespace Game
{

    public class SurvivalMode : GameMode
    {
        public DressupData HakoData;
        public SurvivalData Data;
        public string SceneName = "Game";

        public bool GamePlaying = false;
        bool Result = false;
        int Score = 0;
        int Level = 0;
        int Combo = 0;

        IEnumerator Start()
        {
            if(Instance){
                Destroy(gameObject);
                yield break;
            }
            Instance = this;

            if(DressUpSelecter.Instance)
            {
                HakoData = DressUpSelecter.Instance.Datas[DressUpSelecter.Current];
            }

            DontDestroyOnLoad(gameObject);
            if(SceneName != "")
            {
                yield return LoadManager.Instance.LoadSceneAsync(SceneName);
            }
            UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(gameObject, UnityEngine.SceneManagement.SceneManager.GetActiveScene());
            GameManager.Instance.GameState = GameManager.State.Game;

            SurvivalUI.Instance.Result.SetActive(false);
            Camera.main.orthographicSize = Data.CameraSize;
            yield return null;
            SurvivalUI.Instance.ScoreDisplay.Value = 0;
            SurvivalUI.Instance.LevelDisplay.Value = 0;
            SurvivalUI.Instance.ModeName = ModeName;
            while(!HakoManaer.Instance){yield return null;}
            HakoManaer.Instance.hakoMaxs = Data.MaxHakos;
            yield return HakoManaer.Instance.SetupStream(HakoData, Data.Size);
            SurvivalUI.Instance.Meter.Set(HakoData, Data.Size);
            ScreenCurtain.Instance.SetColor(ScreenCurtain.State.Open);
            HakoManaer.Instance.AddHako(HakoType.X, Data.Size / 2);
            SurvivalUI.Instance.Meter.MaxHakos(Data.LevelUpScore[0]);
            SwipeMessage.Play("Ready...", new Vector3(), 56, 0.5f);
            yield return new WaitForSeconds(1.0f);
            GamePlaying = true;
            SwipeMessage.Play("Go!", new Vector3(), 56, 0.5f);
            
            while(!Result)
            {
                HakoUpdate();
                yield return null;
                if(Input.GetKey(KeyCode.Return))
                {
                    AddHakoRandom();
                }
                else
                {
                    yield return new WaitForSeconds(1.0f);
                }
            }
            SwipeMessage.Play("GameOver!", new Vector3(), 56, 30.0f);
            // SurvivalUI.Instance.UserNameText.text = ScoreManager.Instance.PlayerName;
            SurvivalUI.Instance.UserName = "";
            SurvivalUI.Instance.Result.SetActive(true);
            SoundManager.SEPlay("over");
            UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(SurvivalUI.Instance.ExitButton.gameObject);

            // ScoreContent _load = ScoreManager.Instance.LoadScore(ModeName);
            // if(Score > _load.Score)
            // {
            //     SwipeMessage.Play("New Record!", new Vector3(0, -1, 0), 28, 30.0f);
            //     ScoreManager.Instance.SaveScore(ModeName, new ScoreContent(Score));
            // }

            NCMB.HighScore myScore = new NCMB.HighScore(ModeName, ScoreManager.Instance.UserName.Id, ScoreManager.Instance.UserName.Name);
            myScore.fetch();

            for(int i = 0; i < Data.Size.x * Data.Size.y - 10; i++)
            {
                yield return null;
                AddHakoRandom();
            }

            while(myScore.Score == -1){yield return null;}
            if(myScore.Score < Score)
            {
                myScore.Score = Score;
                myScore.Date = ScoreManager.Instance.DateString();
                myScore.Name = ScoreManager.Instance.UserName.Name;
                myScore.save();
                SwipeMessage.Play("New Record!", new Vector3(0, -1, 0), 28, 30.0f);
            }

            yield return new WaitForSeconds(1.0f);
            SurvivalUI.Instance.ScoreButton.interactable = true;
        }

        override public void InputDir(Dir _dir)
        {
            if(GamePlaying)
            {
                if(Combo > 0){Combo = -Combo;}
                else if(Combo < 0){Combo = 0;}
                if(HakoManaer.Instance.MoveHako(_dir))
                {
                    SoundManager.SEPlay("move");
                }
                else
                {
                    SoundManager.SEPlay("block");
                }
            }
        }

        public override void HakoDelete(Vector3 _pos, HakoType _type)
        {
            StartCoroutine(HakoDeleteStream(_pos, _type));
        }

        IEnumerator HakoDeleteStream(Vector3 _pos, HakoType _type)
        {
            Combo = Mathf.Abs(Combo) + 1;
            Score += Combo;
            SurvivalUI.Instance.ScoreDisplay.Value = Score;
            SwipeMessage.Play("+" + Combo.ToString(), _pos, 28 + Mathf.Min(Combo * 7 / 2, 42));
            yield return null;

            if(Score >= Data.LevelUpScore[Level])
            {
                Level++;
                SurvivalUI.Instance.LevelDisplay.Value = Level;
                SurvivalUI.Instance.Meter.MaxHakos(Data.LevelUpScore[Level] - Data.LevelUpScore[Level - 1]);
                SwipeMessage.Play("LevelUp!", new Vector3(), 42, 0.5f);
                SoundManager.SEPlay("levelup");
                for(int x = 0; x < Data.Size.x; x++)
                {
                    for(int y = 0; y < Data.Size.y; y++)
                    {
                        int _tar = HakoManaer.Instance.Map[x, y];
                        if(_tar > 0 && !HakoManaer.Instance.Hakos[_tar].IsGhost)
                        {
                            HakoManaer.Instance.DeleteHako(_tar);
                        }
                    }
                }
            }
            else
            {
                SoundManager.SEPlay("add", 1.0f + (Combo - 1) * 0.06f);
            }
            SurvivalUI.Instance.Meter.AddHako(_type, Score - Data.LevelUpScore[Level - 1]);
            HakoUpdate();
        }

        public override void HakoUpdate()
        {
            int _hakoCount = 0;
            int _appearNumber = -1;
            for(int x = 0; x < Data.Size.x; x++)
            {
                for(int y = 0; y < Data.Size.y; y++)
                {
                    if(HakoManaer.Instance.Map[x, y] > 0)
                    {
                        _hakoCount ++;
                        if(HakoManaer.Instance.Hakos[HakoManaer.Instance.Map[x, y]].IsGhost)
                        {
                            _appearNumber = HakoManaer.Instance.Map[x, y];
                        }
                    }
                }
            }
            // 場の箱が2以下の時、出現中のブロックの出現速度を上げる。
            if(_hakoCount < 3 && _appearNumber > 0)
            {
                HakoManaer.Instance.Hakos[_appearNumber].AppearSpeed = 3;
            }

            if(_hakoCount < 2)
            {
                AddHakoRandom();
            }

            if(_appearNumber < 0)
            {
                AddHakoRandom();
            }

            // ゲームオーバー？
            bool _isBrank = false;
            for(int i = 0; i < Data.Size.x; i++)
            {
                if(HakoManaer.Instance.Map[i, HakoManaer.Instance.Hakos[0].Position.y] < 0 ||
                    HakoManaer.Instance.Hakos[HakoManaer.Instance.Map[i, HakoManaer.Instance.Hakos[0].Position.y]].IsGhost)
                {
                    _isBrank = true;
                    break;
                }
            }
            for(int i = 0; i < Data.Size.y; i++)
            {
                if(HakoManaer.Instance.Map[HakoManaer.Instance.Hakos[0].Position.x, i] < 0 ||
                    HakoManaer.Instance.Hakos[HakoManaer.Instance.Map[HakoManaer.Instance.Hakos[0].Position.x, i]].IsGhost)
                {
                    _isBrank = true;
                    break;
                }
            }

            // 必殺技（笑）
            if(!_isBrank)
            {
                for(int i = 0; i < 4; i++)
                {
                    Vector2Int _target = HakoManaer.Instance.Hakos[0].Position + HakoManaer.Instance.DirVector(InputManager.GetIntToDir(i));
                    if(_target.x >= 0 && _target.y >= 0 && _target.x < Data.Size.x && _target.y < Data.Size.y && 
                        HakoManaer.Instance.MoveHako(_target,InputManager.GetIntToDir(i))){_isBrank = true;Combo = 0;}
                }
            }

            if(!_isBrank)
            {
                GamePlaying = false;
                Result = true;
            }
        }

        void AddHakoRandom()
        {
            Vector2Int[] _candi = new Vector2Int[Data.Size.x * Data.Size.y];
            int _maxCount = 0;
            int[] _typeCount = new int[]{0, 0, 0, 0, 0, 0};
            // 出現位置は：まず、外周以外にランダム。
            for(int x = 1; x < Data.Size.x - 1; x++)
            {
                for(int y = 1; y < Data.Size.y - 1; y++)
                {
                    if(HakoManaer.Instance.Map[x, y] < 0)
                    {
                        _candi[_maxCount] = new Vector2Int(x, y);
                        _maxCount ++;
                    }
                    else
                    {
                        _typeCount[(int)HakoManaer.Instance.Hakos[HakoManaer.Instance.Map[x, y]].Type]++;
                    }
                }
            }
            // 出現位置は：つぎに、外周にランダム。
            if(_maxCount <= 0)
            {
                Vector2Int _start = Vector2Int.zero;
                for(int i = 0; i < 4; i++)
                {
                    Vector2Int _dir = HakoManaer.Instance.DirVector(InputManager.GetIntToDir(i));
                    int _max = i%2==0?Data.Size.y-1:Data.Size.x-1;
                    for(int j = 0; j < _max; j++)
                    {
                        Vector2Int _target = _start + _dir * j;
                        if(HakoManaer.Instance.Map[_target.x, _target.y] < 0)
                        {
                            _candi[_maxCount] = _target;
                            _maxCount++;
                        }
                        else
                        {
                            _typeCount[(int)HakoManaer.Instance.Hakos[HakoManaer.Instance.Map[_target.x, _target.y]].Type]++;
                        }
                    }
                    _start += _dir * _max;
                }
            }
            if(_maxCount > 0)
            {
                HakoType _type = 0;
                int _typeRandom = 0;
                for(int i = 1; i < 5; i++)
                {
                    _typeRandom += (Data.MaxHakos - _typeCount[i]);
                }
                _typeRandom = Random.Range(0, _typeRandom - 1);
                for(int i = 1; i < 5; i++)
                {
                    _typeRandom -= (Data.MaxHakos - _typeCount[i]);
                    if(_typeRandom < 0){
                        _type = HakoManaer.Instance.GetIntToType(i);
                        break;
                    }
                }
                Vector2Int _pos = _candi[Random.Range(0,_maxCount)];
                HakoManaer.Instance.AddHako(_type, _pos, 1.0f / Data.AppearTimes[Level]);
            }
        }

        public override Vector2Int TargetHakoPosition()
        {
            if(GamePlaying)
            {
                return HakoManaer.Instance.Hakos[0].Position;
            }
            else
            {
                return -Vector2Int.one;
            }
        }
    }
}