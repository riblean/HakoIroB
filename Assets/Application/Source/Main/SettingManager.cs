using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Yak
{
    // 基本情報自動読み込み。 2021 03 02
    // ほかのスクリプトはここからデータを読み込む。
    public class SettingManager : MonoBehaviour
    {
        public static SettingManager Instance{get; private set;}
        string fileName = "UserData.json";
        UserData data;
        void Awake()
        {
            if(!Instance){Instance = this;}
            data = new UserData();
            Yak.DataManager.Instance.LoadJson<UserData>(ref data, fileName);

            data.PlayCount++;
            data.LastPlay = DateTime.Now;
        }

        void OnApplicationQuit()
        {
            TimeSpan _ts = DateTime.Now - data.LastPlay;
            data.LastPlay = DateTime.Now;
            data.PlayTime += _ts.TotalMinutes;
            Yak.DataManager.Instance.SaveJson<UserData>(data, fileName);
        }

        public static UserData Data
        {
            get{ return Instance.data;}
        }
    }

    public class UserData
    {
        public string UserName = "Guest";
        public int PlayCount = 0;
        public Double PlayTime = 0;
        public DateTime LastPlay;
    }
}