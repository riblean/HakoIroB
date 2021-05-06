using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Game;

public class DressUpSelecter : MonoBehaviour
{
    public static DressUpSelecter Instance{get; private set;}

    public DressupData[] Datas;

    public Button ChangeButton;
    public Text CommentText;

    public static int Current = 0;
    public int Select = 0;

    void Start()
    {
        if(!Instance){
            Instance = this;
            Current = PlayerPrefs.GetInt("DressUpNumber", 0);
        }
    }

    public void SelectButton(int i)
    {
        ChangeButton.interactable = i != Current;

        if(Select != i)
        {
            Select = i;
            CommentText.text = Datas[i].Comment;
        }
    }

    public void Change()
    {
        Current = Select;
        ChangeButton.interactable = false;
        PlayerPrefs.SetInt("DressUpNumber", Current);
        PlayerPrefs.Save();
    }
}
