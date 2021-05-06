using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Game
{
    public class HakoMeter : MonoBehaviour
    {
        public Transform BackRoot;
        public Vector2Int StartPos;
        public Vector2Int TargetPos;
        public Vector2Int EndPos;
        DressupData dressupData;
        Vector2Int Size;

        [SerializeField] int width = 3;

        HakoAnimation[] backs = new HakoAnimation[0];
        HakoAnimation[] fronts = new HakoAnimation[0];

        int HakoValue = 0;
        bool bisy = false;
        
        public void Set(DressupData _data, Vector2Int _size)
        {
            dressupData = _data;
            Size = _size;
        }

        public void MaxHakos(int _max)
        {
            StartCoroutine(MaxHakoStream(_max));
        }

        IEnumerator MaxHakoStream(int _max)
        {
            while(bisy){yield return null;}
            bisy = true;
            HakoValue = 0;
            for(int i = 0; i < fronts.Length; i++)
            {
                fronts[i].Move(GetPosition(2), 0.1f * i * 0);
                fronts[i].Delete();
                yield return new WaitForSeconds(0.1f);
            }
            for(int i = 0; i < _max; i++)
            {
                if(backs.Length == i)
                {
                    Array.Resize(ref backs, i + 1);
                    backs[i] = Instantiate(dressupData.E_Prefab, BackRoot);
                    backs[i].Move(GetPosition(0), -1);
                    yield return new WaitForSeconds(0.1f);
                    backs[i].Appear(10.0f);
                    backs[i].Move(GetPosition(1, i));
                }
            }
            bisy = false;
        }

        public void AddHako(HakoType _type, int _count = 1)
        {
            StartCoroutine(AddHakoStream(_type,  _count));
        }

        IEnumerator AddHakoStream(HakoType _type, int _count = 1)
        {
            while(bisy){yield return null;}
            _count -= HakoValue;
            if(_count <= 0){yield break;}
            bisy = true;
            int[] _nums = new int[_count];
            for(int i = 0; i < _nums.Length; i++)
            {
                _nums[i] = -1;
            }
            int _cur = 0;
            for(int i = 0; i < fronts.Length; i++)
            {
                if(!fronts[i].Active && fronts[i].Type == _type)
                {
                    _nums[_cur] = i;
                    _cur++;
                    if(_cur>=_nums.Length - 1){break;}
                }
            }
            for(int i = _cur; i < _nums.Length; i++)
            {
                yield return null;
                Array.Resize(ref fronts, fronts.Length + 1);
                fronts[fronts.Length - 1] = Instantiate(dressupData.GetHakoPrefab(_type), transform);
                _nums[i] = fronts.Length - 1;
            }
            yield return null;
            for(int i = 0; i < _nums.Length; i++)
            {
                fronts[_nums[i]].Move(GetPosition(0), -1);
                yield return new WaitForSeconds(0.1f);
                fronts[_nums[i]].Appear(10.0f);
                fronts[_nums[i]].Move(GetPosition(1, HakoValue));
                HakoValue++;
            }
            bisy = false;
        }

        Vector2Int GetPosition(int _num, int _pos = 0)
        {
            if(_num == 0){
                return new Vector2Int(StartPos.x + Size.x / 2, StartPos.y - Size.y / 2);
            }
            else if(_num == 1)
            {
                return new Vector2Int(TargetPos.x + Size.x / 2 + _pos % width, TargetPos.y - Size.y / 2 + _pos / width);
            }
            return new Vector2Int(EndPos.x + Size.x / 2, EndPos.y - Size.y / 2);
        }
    }
}
