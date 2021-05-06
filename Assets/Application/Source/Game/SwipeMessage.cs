using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SwipeMessage : MonoBehaviour
{
    public static SwipeMessage Instance{get; private set;}
    [SerializeField] Text[] texts;
    bool[] actives;
    [SerializeField] float speed = 0.1f;
    [SerializeField] float feedOutTime = 1f;
    
    void Start()
    {
        if(!Instance){Instance = this;}
        actives = new bool[texts.Length];
    }

    public static void Play(string _message, Vector3 _pos = new Vector3(), int _fontSize = 28, float _outTime = -1)
    {
        if(Instance)
        {
            int _num = 0;
            for(int i = 0; i < Instance.actives.Length; i++)
            {
                if(!Instance.actives[i])
                {
                    _num = i;
                    break;
                }
            }

            Instance.texts[_num].text = _message;
            Instance.texts[_num].fontSize = _fontSize;
            Instance.StartCoroutine(Instance.stream(_num, _pos, _outTime));
        }
    }

    IEnumerator stream(int _num, Vector3 _pos, float _outTime)
    {
        actives[_num] = true;
        texts[_num].gameObject.SetActive(true);
        if(_outTime < 0){_outTime = feedOutTime;}
        _pos = RectTransformUtility.WorldToScreenPoint(Camera.main, _pos);

        texts[_num].rectTransform.position = new Vector3(_pos.x, -100);

        float _start = Time.time;
        while(Time.time - _start - _outTime < 0)
        {
            texts[_num].rectTransform.position += (_pos - texts[_num].rectTransform.position) * Time.deltaTime * speed;
            yield return null;
        }
        yield return new WaitForSeconds(_outTime);
        _pos = new Vector3(_pos.x, Screen.height + 100);
        while(Time.time - _start - _outTime * 3.0f < 0)
        {
            texts[_num].rectTransform.position += (_pos - texts[_num].rectTransform.position) * Time.deltaTime * speed;
            yield return null;
        }
        
        actives[_num] = false;
        texts[_num].gameObject.SetActive(false);
    }
}
