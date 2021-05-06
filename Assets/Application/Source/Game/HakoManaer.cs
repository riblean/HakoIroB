using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Yak;

namespace Game
{

    public class HakoManaer : MonoBehaviour
    {
        public static HakoManaer Instance{get; private set;}
        public DressupData Data;
        public Vector2Int Size = new Vector2Int(5, 5);
        public int hakoMaxs = 10;

        public HakoAnimation[] Hakos{get; private set;}
        public int[,] Map;

        [SerializeField] float joinLateTime = 0.1f;

        // Ghost 出現中ハコ移動のデータ一時保存。
        int tempGhostNumber = -1;

        void Awake()
        {
            if(!Instance){Instance = this;}
        }

        void OnGUI()
        {
            if(InputManager.Instance.DebugMode && Map != null)
            {
                string _str = "";
                for(int x = 0; x < Size.x; x++)
                {
                    for(int y = 0; y < Size.y; y++)
                    {
                        if(Map[y,Size.x - x - 1] >= 0)
                        {

                        _str += Map[y,Size.x - x - 1].ToString("00") + " ";
                        }
                        else
                        {
                        _str += "--- ";

                        }
                    }
                    _str += "\n";
                }
                GUI.Label(new Rect(120, 10, 200, 100), _str);

                _str = "";
                for(int i = 0; i < Hakos.Length; i++)
                {
                    _str += i + " " + Hakos[i].Type + " " + Hakos[i].Position + " " + Hakos[i].Active + "\n";
                }
                GUI.Label(new Rect(10, 10, 200, 520), _str);
            }
        }

        public IEnumerator SetupStream(DressupData _data, Vector2Int _size)
        {
            while(LoadManager.Instance.IsLoading){yield return null;}
            LoadManager.Instance.IsLoading = true;
            Data = _data;
            Size = _size;
            yield return null;
            BackgroundScale _backgroundScale = Instantiate(Data.BackgroundPrefab);

            Map = new int[_size.x, _size.y];
            for(int x = 0; x < _size.x; x++)
            {
                for(int y = 0; y < _size.y; y++)
                {
                    Map[x,y] = -1;
                }
            }

            yield return null;
            _backgroundScale.SetSize(_size);
            Hakos = new HakoAnimation[1 + hakoMaxs * 4];
            Hakos[0] = Instantiate(Data.X_Prefab);

            for(int i = 0; i < 4; i++)
            {
                yield return null;
                HakoAnimation temp = Data.A_Prefab;
                if(i == 1){temp = Data.B_Prefab;}
                if(i == 2){temp = Data.C_Prefab;}
                if(i == 3){temp = Data.D_Prefab;}
                for(int j = 0; j < hakoMaxs; j++)
                {
                    Hakos[i*hakoMaxs+j+1] = Instantiate(temp);
                }
            }

            LoadManager.Instance.IsLoading = false;
        }

        public int GetUnuseHako(HakoType _type)
        {
            for(int i = 0; i < Hakos.Length; i++)
            {
                if(!Hakos[i].Active && Hakos[i].Type == _type)
                {
                    return i;
                }
            }
            return -1;
        }

        public bool MoveHako(Dir _dir, int _number = 0)
        {
            return MoveHako(Hakos[_number].Position, _dir);
        }
        
        public bool MoveHako(Vector2Int _pos, Dir _dir, int _count = 0)
        {
            int _myNumber = GetMapNumber(_pos);
            if(_myNumber < 0){return false;}
            Vector2Int _next = _pos + DirVector(_dir);

            void _move(bool _out){
                Hakos[_myNumber].Move(_next, joinLateTime * _count);
                Map[_pos.x,_pos.y] = -1;
                if(_out)
                {
                    Map[_next.x,_next.y] = _myNumber;
                    // Ghost
                    if(_count == 0 && tempGhostNumber >= 0)
                    {
                        Map[_pos.x, _pos.y] = tempGhostNumber;
                        Hakos[tempGhostNumber].Move(_pos, 0);
                        tempGhostNumber = -1;
                    }
                }
                else
                {
                    Hakos[_myNumber].Delete();
                    GameMode.Instance.HakoDelete(WorldPosition(Hakos[_myNumber].Position), Hakos[_myNumber].Type);
                }
            }

            if(_next.x >= 0 && _next.y >= 0 && _next.x < Size.x && _next.y < Size.y){
                if(GetMapNumber(_next) >= 0)
                {
                    if(Hakos[GetMapNumber(_next)].IsGhost)
                    {
                        tempGhostNumber = GetMapNumber(_next);
                        _move(true);
                        return true;
                    }
                    else if(MoveHako(_next, _dir, _count + 1))
                    {
                        _move(true);
                        return true;
                    }
                    else
                    {
                        Hakos[_myNumber].Move(_next, joinLateTime * _count, true);
                        return false;
                    }
                }
                else
                {
                    _move(true);
                    return true;
                }
            }
            else
            {
                if(Hakos[_myNumber].Type == HakoType.A && _dir == Dir.up){ _move(false); return true;}
                else if(Hakos[_myNumber].Type == HakoType.B && _dir == Dir.right){ _move(false); return true;}
                else if(Hakos[_myNumber].Type == HakoType.C && _dir == Dir.down){ _move(false); return true;}
                else if(Hakos[_myNumber].Type == HakoType.D && _dir == Dir.left){ _move(false); return true;}
                else{
                    Hakos[_myNumber].Move(_next, joinLateTime * _count, true);
                    return false;
                }
            }
        }

        public int AddHako(HakoType _type, Vector2Int _pos, float _time = -1)
        {
            if(GetMapNumber(_pos) >= 0){return -1;}
            int _num = GetUnuseHako(_type);
            if(_num < 0){Debug.Log("はこがたりない？"); return -1;}
            Map[_pos.x,_pos.y] = _num;
            Hakos[_num].Appear(_time, true);
            Hakos[_num].Move(_pos, -1);
            return _num;
        }

        public void DeleteHako(int _num)
        {
            Map[Hakos[_num].Position.x, Hakos[_num].Position.y] = -1;
            Hakos[_num].Delete(true);
        }

        public Vector2Int DirVector(Dir _dir)
        {
            switch(_dir)
            {
                case Dir.up: return Vector2Int.up;
                case Dir.right: return Vector2Int.right;
                case Dir.down: return Vector2Int.down;
                case Dir.left: return Vector2Int.left;
            }
            return Vector2Int.zero;
        }

        public Vector3 WorldPosition(Vector2Int _pos)
        {
            return new Vector3((float)_pos.x - (Size.x - 1.0f) * 0.5f, (float)_pos.y - (Size.y - 1.0f) * 0.5f, 0);
        }

        public Vector2 MapPosition(Vector3 _pos)
        {
            return new Vector2(_pos.x + (Size.x - 1.0f) * 0.5f, _pos.y + (Size.y - 1.0f) * 0.5f);
        }

        public Vector2Int MapPositionInt(Vector3 _pos)
        {
            return Vector2Int.RoundToInt(new Vector2(_pos.x + (Size.x - 1.0f) * 0.5f, _pos.y + (Size.y - 1.0f) * 0.5f));
        }

        public int GetMapNumber(Vector2Int _pos)
        {
            return Map[_pos.x,_pos.y];
        }

        public HakoType GetIntToType(int _number)
        {
            return (HakoType)Enum.ToObject(typeof(HakoType), _number);
        }
    }

    public enum HakoType
    {
        X,
        A,
        B,
        C,
        D,
        E
    }
}
/*
1:0
2:0.5
3:1
4
5:2
*/