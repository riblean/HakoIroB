using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Yak
{
    // 選択肢を表示します。
    // 2021 03 02
    // yield return Selecter.Instance.AsyncAnswer("ゲームを終了しますか?");
    public class Selecter : MonoBehaviour
    {
        public static Selecter Instance{get; private set;}
        [SerializeField] GameObject Object;
        [SerializeField] Text messageText;
        bool isUsing = false;

        public int Answer = 0;

        void Awake()
        {
            if(!Instance){Instance = this;}
            Object.SetActive(false);
        }

        public IEnumerator AsyncAnswer(string _message = "")
        {
            while(isUsing){yield return null;}
            isUsing = true;
            messageText.text = _message;
            Object.SetActive(true);
            Answer = -1;
            while(Answer < 0)
            {
                yield return null;
            }
            isUsing = false;
        }

        public void SetAnswer(int _number)
        {
            Answer = _number;
            Object.SetActive(false);
        }
    }
}
