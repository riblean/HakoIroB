using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class BackgroundDef : BackgroundScale
    {
        [SerializeField] Transform ACenter;
        [SerializeField] Transform BCenter;
        [SerializeField] Transform CCenter;
        [SerializeField] Transform DCenter;
        [SerializeField] SpriteRenderer ABase;
        [SerializeField] SpriteRenderer BBase;
        [SerializeField] SpriteRenderer CBase;
        [SerializeField] SpriteRenderer DBase;

        [SerializeField] GameObject Back;
        public override void SetSize(Vector2Int _size)
        {
            Vector2 _pos = new Vector2( (float)(_size.x - 1) / 2.0f + 1.0f, (float)(_size.y - 1) / 2.0f + 1.0f);
            ACenter.localPosition = new Vector3(0, _pos.y, 0);
            BCenter.localPosition = new Vector3(_pos.x, 0, 0);
            CCenter.localPosition = new Vector3(0, -_pos.y, 0);
            DCenter.localPosition = new Vector3(-_pos.x, 0, 0);
            ABase.size = new Vector3((float)_size.x, 1, 1);
            BBase.size = new Vector3(1, (float)_size.y, 1);
            CBase.size = new Vector3((float)_size.x, 1, 1);
            DBase.size = new Vector3(1, (float)_size.y, 1);

            StartCoroutine(stream(_size));
        }

        IEnumerator stream(Vector2Int _size)
        {
            Back.transform.localPosition = new Vector3Int(_size.x / 2, _size.y / 2, 2) - new Vector3(((float)_size.x - 1.0f) * 0.5f, ((float)_size.y - 1.0f) * 0.5f, 0);
            yield return new WaitForSeconds(0.3f);
            if(_size.x < 3 || _size.y < 3){yield break;}
            List<Vector3> _poss = new List<Vector3>();
            int _max = 0;
            for(int x = 1; x < _size.x - 1; x++)
            {

                for(int y = 1; y < _size.y - 1; y++)
                {
                    if(x == _size.x / 2 && y == _size.y / 2 ){}
                    else
                    {
                        _max++;
                        _poss.Add(new Vector3(x, y, 2));
                        yield return null;
                    }
                }
            }
            while(_max > 0)
            {
                int _cur = Random.Range(0, _max);
                Instantiate(Back, transform).transform.localPosition = _poss[_cur] - new Vector3(((float)_size.x - 1.0f) * 0.5f, ((float)_size.y - 1.0f) * 0.5f, 0);
                _poss.RemoveAt(_cur);
                _max--;
                yield return new WaitForSeconds(0.03f);
            }
        }
    }
}