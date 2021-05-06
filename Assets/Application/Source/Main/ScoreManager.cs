using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using NCMB;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance{get; private set;}
    public NCMB.UserName UserName;

    public GameObject Obj;
    public Text HeaderText;
    [Header("Line")]
    public RectTransform LineRect;
    public RectTransform ScrollContent;
    public RectTransform[] LineRects;

    [Header("UserLine")]
    public Text UserRankText;
    public Text UserScoreText;
    public Text UserDateText;
    public InputField UserNameText;

    void Awake()
    {
        if(!Instance){Instance = this;}
    }

    IEnumerator Start()
    {
        Obj.SetActive(false);
        yield return new WaitForSeconds(0.1f);
        Yak.LoadManager.Instance.IsLoading = true;
        yield return null;
        UserName = new NCMB.UserName();

        while(UserName.Id == 0 || UserName.Name == "")
        {
            yield return null;
        }

        UserNameText.text = UserName.Name;
        yield return null;

        LineRects = new RectTransform[99];
        for(int i = 0; i < LineRects.Length; i++)
        {
            yield return null;
            LineRects[i] = Instantiate(LineRect, ScrollContent);
            LineRects[i].anchoredPosition = new Vector3(0, - 20.0f - i * 40.0f, 0);
            Text[] _texts = LineRects[i].gameObject.GetComponentsInChildren<Text>();
            _texts[0].text = (i+1).ToString();
            if(i % 2 == 1)
            {
                LineRects[i].gameObject.GetComponentInChildren<Image>().color = new Color(0.3f, 0.3f, 0.3f);
            }
            else
            {
                LineRects[i].gameObject.GetComponentInChildren<Image>().color = new Color(0.5f, 0.5f, 0.5f);
            }
        }
    }

    IEnumerator RankingStream(string _modeName)
    {
        while(UserName == null || UserName.Name == ""){yield return null;}

        HighScore myScore = new HighScore(_modeName, UserName.Id, UserName.Name);
        myScore.fetch();
        string[] _names = new string[LineRects.Length];
        int[] _scores = new int[LineRects.Length];
        string[] _dates = new string[LineRects.Length];
        int _length = -1;
        NCMBQuery<NCMBObject> query = new NCMBQuery<NCMBObject>(_modeName);
        query.AddDescendingOrder("Score");
        query.Limit = LineRects.Length;
        query.FindAsync((List<NCMBObject> objList, NCMBException e) => {
            if(e == null)
            {
                _length = objList.Count;
                for(int i = 0; i < _length; i++)
                {
                    _names[i] = System.Convert.ToString( objList[i]["Name"]);
                    _scores[i] = System.Convert.ToInt32( objList[i]["Score"]);
                    _dates[i] = System.Convert.ToString( objList[i]["Date"]);
                }
            }
        });

        while(myScore.Score == -1){yield return HeaderLoading(_modeName);}
        UserScoreText.text = myScore.Score != -2?myScore.Score.ToString():"---";
        UserDateText.text = myScore.Score != -2?myScore.Date.ToString():"---";
        UserRankText.text = "-";

        while(_length < 0){yield return HeaderLoading(_modeName);}
        ScrollContent.sizeDelta = new Vector2(ScrollContent.sizeDelta.x, 40.0f * _length);
        for(int i = 0; i < _length; i++)
        {
            LineRects[i].gameObject.SetActive(true);
            Text[] _texts = LineRects[i].gameObject.GetComponentsInChildren<Text>();
            _texts[1].text = _names[i];
            _texts[2].text = _scores[i].ToString();
            _texts[3].text = _dates[i];
            yield return null;
        }
        
        for(int i = _length; i < LineRects.Length; i++)
        {
            LineRects[i].gameObject.SetActive(false);
            yield return null;
        }
        HeaderText.text = _modeName;
    }

    IEnumerator HeaderLoading(string _str)
    {
        HeaderText.text = "-" + _str + "-";
        yield return new WaitForSeconds(0.1f);
        HeaderText.text = "\\" + _str + "\\";
        yield return new WaitForSeconds(0.1f);
        HeaderText.text = "|" + _str + "|";
        yield return new WaitForSeconds(0.1f);
        HeaderText.text = "/" + _str + "/";
        yield return new WaitForSeconds(0.1f);
    }

    public string DateString()
    {
        return DateTime.Now.ToString("yyyy/MM/dd-hh:mm:ss");
    }

    public void SetUserName()
    {
        UserName.Name = UserNameText.text;
        UserName.save();
    }

    public void Open(string _name)
    {
        Obj.SetActive(true);
        StartCoroutine(RankingStream(_name));
    }
}

namespace NCMB
{
    public class HighScore
    {
        public string ModeName{get; private set;}
        public int Score;
        public int Id{get; private set;}
        public string Name;
        public string Date;

        public HighScore(string _modeName, int _id, string _name, int _score = -1, string _date = "")
        {
            ModeName = _modeName;
            Id = _id;
            Name = _name;
            Score = _score;
            Date = _date;
        }

        public void save()
        {
            NCMBQuery<NCMBObject> query = new NCMBQuery<NCMBObject>(ModeName);
            query.WhereEqualTo("ID", Id);
            query.FindAsync((List<NCMBObject> objList, NCMBException e) => {
                if(e == null)
                {
                    if(objList.Count == 0)
                    {
                        NCMBObject obj = new NCMBObject(ModeName);
                        obj["Score"] = Mathf.Max(Score, 0);
                        obj["Date"] = Date;
                        obj["ID"] = Id;
                        obj["Name"] = Name;
                        obj.SaveAsync();
                        Score = 0;
                    }
                    else
                    {
                        objList[0]["Score"] = Score;
                        objList[0]["Date"] = Date;
                        objList[0]["Name"] = Name;
                        objList[0].SaveAsync();
                    }
                }
            });
        }

        public void fetch()
        {
            NCMBQuery<NCMBObject> query = new NCMBQuery<NCMBObject>(ModeName);
            query.WhereEqualTo("ID", Id);
            query.FindAsync((List<NCMBObject> objList, NCMBException e) => {
                if(e == null)
                {
                    if(objList.Count == 0)
                    {
                        Score = -2;
                    }
                    else
                    {
                        Name = System.Convert.ToString( objList[0]["Name"]);
                        Score = System.Convert.ToInt32( objList[0]["Score"]);
                        Date = System.Convert.ToString( objList[0]["Date"]);
                    }
                }
            });
        }
    }

    public class UserName
    {
        public int Id;
        public string ObjectId;
        public string Name;

        public UserName()
        {
            Id = PlayerPrefs.GetInt("ID", 0);
            Name = "";
            ObjectId = "";
            if(Id == 0){MaxID();}
            else{fetch();}
        }

        public void save()
        {
            NCMBQuery<NCMBObject> query = new NCMBQuery<NCMBObject>("UserName");
            query.WhereEqualTo("ID", Id);
            query.FindAsync((List<NCMBObject> objList, NCMBException e) => {
                if(e == null)
                {
                    objList[0]["ID"] = Id;
                    objList[0]["Name"] = Name;
                    objList[0].SaveAsync();
                }
            });
        }

        public void fetch()
        {
            NCMBQuery<NCMBObject> query = new NCMBQuery<NCMBObject>("UserName");
            query.WhereEqualTo("ID", Id);
            query.FindAsync((List<NCMBObject> objList, NCMBException e) => {
                if(e == null)
                {
                    if(objList.Count == 0)
                    {
                        NCMBObject obj = new NCMBObject("UserName");
                        obj["ID"] = 0;
                        obj["Name"] = Name;
                        obj.SaveAsync();
                        ObjectId = obj.ObjectId;
                    }
                    else
                    {
                        Id = System.Convert.ToInt32( objList[0]["ID"]);
                        Name = System.Convert.ToString( objList[0]["Name"]);
                        ObjectId = objList[0].ObjectId;
                    }
                }
            });
        }

        public void MaxID()
        {
            NCMBQuery<NCMBObject> query = new NCMBQuery<NCMBObject>("UserName");
            query.AddDescendingOrder("ID");
            query.Limit = 1;
            
            query.FindAsync((List<NCMBObject> objList, NCMBException e) => {
                if(e == null)
                {
                    if(objList.Count == 0)
                    {
                        Id = 1;
                    }
                    else
                    {
                        Id = System.Convert.ToInt32( objList[0]["ID"]) + 1;
                        ObjectId = objList[0].ObjectId;
                    }
                    NCMBObject obj = new NCMBObject("UserName");
                    obj["ID"] = Id;
                    obj["Name"] = Name;
                    obj.SaveAsync();
                    PlayerPrefs.SetInt("ID", Id);
                    PlayerPrefs.Save();
                }
            });
        }
    }
}