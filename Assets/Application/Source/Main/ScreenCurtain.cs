using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Yak
{
    // 画面最前面のUIに色を配置します。
    // 2021 03 02
    // ScreenCurtain.Instance.SetColor(ScreenCurtain.State.Open);
    public class ScreenCurtain : MonoBehaviour
    {
        public static ScreenCurtain Instance{get; private set;}
        [SerializeField]Image image;
        public Color[] Palette = new Color[]{
            new Color(0.28f, 0.28f, 0.28f, 1),
            new Color(1, 1, 1, 1),
            new Color(1, 1, 1, 0)
        };
        public float DefaultChangeTime = 0.3f;
        IEnumerator coroutine;
        public enum State
        {
            Start,
            Close,
            Open
        }
        
        void Awake()
        {
            if(!Instance)
            {
                Instance = this;
            }
            if(image == null)
            {
                image = GetComponent<Image>();
            }
            SetColor(State.Start, 0);
        }

        public void SetColor(Color _col, float _time)
        {
            if(coroutine != null){StopCoroutine(coroutine);}
            if(_time < 0)
            {
                coroutine = ColorCoroutine(_col, DefaultChangeTime);
                StartCoroutine(coroutine);
            }
            else if(_time > 0)
            {
                coroutine = ColorCoroutine(_col, _time);
                StartCoroutine(coroutine);
            }
            else
            {
                image.color = _col;
            }
        }

        public void SetColor(State _col = State.Open, float _time = -1)
        {
            SetColor(Palette[(int)_col], _time);
        }

        IEnumerator ColorCoroutine(Color _col, float _time)
        {
            Color _currentColor = image.color;
            for(float f = 0; f < _time; f += Time.deltaTime)
            {
                image.color = Color.Lerp(_currentColor, _col, f / _time);
                yield return null;
            }
            image.color = _col;
        }
    }
}
