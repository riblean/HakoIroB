using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{

    public class HakoAnimation : MonoBehaviour
    {
        [SerializeField] HakoType type = HakoType.E;
        bool active = true;

        [SerializeField] float defaultAppearSpeed = 10.0f;
        [SerializeField] float defaultDeleteTime = 0.2f;
        [SerializeField] float MoveSpeed = 0.2f;
        [SerializeField] float MoveMaxTime = 0.8f;
        [SerializeField] float MoveCancelTime = 0.3f;

        public float AppearSpeed = 10.0f;
        public bool IsGhost = false;
        IEnumerator MoveIE;

        public Vector2Int Position;
        public int Number;
        void Start()
        {
            Active = false;
            MoveIE = MoveCoroutine(Vector2Int.zero, false, 0);
        }

        public bool Active
        {
            get{return active;}
            set
            {
                active = value;
                gameObject.SetActive(value);
            }
        }

        public HakoType Type
        {
            get{ return type;}
        }

        public void Appear(float _speed = -1, bool _isGhost = false)
        {
            if(_isGhost){IsGhost = true;}
            if(_speed < 0){_speed = defaultAppearSpeed;}
            Active = true;
            AppearSpeed = _speed;
            StartCoroutine(AppearCoroutine());
        }

        IEnumerator AppearCoroutine()
        {
            for(float _progress = 0; _progress < 1; _progress += Time.deltaTime * AppearSpeed)
            {
                transform.localScale = _progress * Vector3.one;
                yield return null;
            }
            transform.localScale = Vector3.one;
            IsGhost = false;
            if(GameMode.Instance){GameMode.Instance.HakoUpdate();}
        }

        public void Delete(bool _fast = false)
        {
            if(!Active){return;}
            StartCoroutine(DeleteCoroutine(defaultDeleteTime, _fast));
        }

        IEnumerator DeleteCoroutine(float _time, bool _fast)
        {
            if(!_fast)
            {
                yield return new WaitForSeconds(MoveMaxTime);
            }
            float _start = Time.time;
            while(Time.time - _start - _time < 0)
            {
                transform.localScale = (1 - (Time.time - _start) / _time) * Vector3.one;
                yield return null;
            }
            transform.localScale = Vector3.one;
            Active = false;
        }

        public void Move(Vector2Int _pos, float _lateTime = 0, bool _back = false)
        {
            if(_lateTime < 0){
                transform.localPosition = HakoManaer.Instance.WorldPosition(_pos);
                Position = _pos;
                return;
            }
            if(!Active){return;}
            if(!_back){Position = _pos;}
            StopCoroutine(MoveIE);
            MoveIE = MoveCoroutine(_pos, _back, _lateTime);
            StartCoroutine(MoveIE);
        }

        IEnumerator MoveCoroutine(Vector2Int _worldPos, bool _back, float _lateTime)
        {
            yield return new WaitForSeconds(_lateTime);
            float _start = Time.time;
            while(Time.time - _start < MoveMaxTime)
            {
                if(_back && Time.time - _start - MoveCancelTime > 0){break;}
                Vector3 _delta =  HakoManaer.Instance.WorldPosition(_worldPos) - transform.localPosition;
                if(_back){_delta = (_delta + HakoManaer.Instance.WorldPosition(Position) - transform.localPosition) * 0.5f;}
                transform.localPosition += _delta * MoveSpeed * Time.deltaTime;
                yield return null;
            }
            if(_back)
            {
                while(Time.time - _start < MoveMaxTime * 2.0f)
                {
                    transform.localPosition += (HakoManaer.Instance.WorldPosition(Position) - transform.localPosition) * MoveSpeed * Time.deltaTime;
                    yield return null;
                }
            }
            transform.localPosition = HakoManaer.Instance.WorldPosition(Position);
        }
    }
}
