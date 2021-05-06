using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Yak
{
    // ロード処理統括。2021 03 02
    // Scene Load 代行。
    // IsLoadingによるIEr処理重複回避。
    public class LoadManager : MonoBehaviour
    {
        public static LoadManager Instance{get; private set;}
        [SerializeField]Text text;
        bool isLoading = false;
        public float UpdateTime = 0.8f;
        public string LoadingString = "NowLoading";
        public string ChangeStrings = ".";
        public int Max = 3;
        IEnumerator coroutine;

        void Awake()
        {
            if(!Instance){Instance = this;}
            if(!text){text = GetComponent<Text>();}
            coroutine = LowUpdate();
            isLoading = true;
            IsLoading = false;
        }

        public bool IsLoading
        {
            get{return isLoading;}
            set
            {
                if(isLoading != value)
                {
                    isLoading = value;
                    text.enabled = value;
                    if(value)
                    {
                        StartCoroutine(coroutine);
                    }
                    else
                    {
                        StopCoroutine(coroutine);
                    }
                }
            }
        }

        IEnumerator LowUpdate()
        {
            while(true)
            {
                text.text = LoadingString;
                for(int i = 0; i < Max; i++)
                {
                    text.text += ChangeStrings;
                    yield return new WaitForSeconds(UpdateTime);
                }
            }
        }

        public void LoadScene(string _name)
        {
            StartCoroutine(LoadSceneAsync(_name));
        }

        public IEnumerator LoadSceneAsync(string _name)
        {
            while(isLoading){yield return null;}
            IsLoading = true;
            ScreenCurtain.Instance.SetColor(ScreenCurtain.State.Close);

            AsyncOperation operation = SceneManager.LoadSceneAsync(_name);
            operation.allowSceneActivation = false;
            yield return new WaitForSeconds(ScreenCurtain.Instance.DefaultChangeTime);
            operation.allowSceneActivation = true;

            while(!operation.isDone)
            {
                yield return new WaitForSeconds(UpdateTime);
            }

            IsLoading = false;
        }
    }
}
