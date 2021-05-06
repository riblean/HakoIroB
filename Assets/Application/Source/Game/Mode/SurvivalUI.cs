using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class SurvivalUI : MonoBehaviour
    {
        public static SurvivalUI Instance{get; private set;}
        public HakoMeter Meter;

        public ScoreDisplay ScoreDisplay;
        public ScoreDisplay LevelDisplay;

        public GameObject Result;
        public string UserName = "";
        public bool ScoreSentButtonUsed = false;
        public Button ExitButton;
        public Button ScoreButton;
        public string ModeName = "";
        
        void Start()
        {
            if(!Instance){Instance = this;}
            ScoreButton.interactable = false;
        }

        public void OpenScore()
        {
            ScoreManager.Instance.Open(ModeName);
        }

        public void Exit()
        {
            ScoreManager.Instance.Obj.SetActive(false);
            Yak.GameManager.Instance.GameState = Yak.GameManager.State.MainMenu;
        }
    }
}
