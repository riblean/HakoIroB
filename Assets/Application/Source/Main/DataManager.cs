using System.IO;
using System.Text;
using UnityEngine;

namespace Yak
{
    /*
    ファイル入出力用クラス。2021年2月22日
    DataManager.Instance.SaveJson<>(ref a);
    のように使う。
    または、
    DataManager dataManager = new DataManager(directry, extension);
    dataManager.SaveJson<>(ref a);
    のようにデフォルトパスを指定できる。
    */
    public class DataManager
    {
        static DataManager instace;
        public string FilePath;
        public string Extension;

        public static DataManager Instance
        {
            get
            {
                if(instace == null){ instace = new DataManager(Application.persistentDataPath + "/"); }
                return instace;
            }
        }

        public DataManager(string _path, string _ext = "")
        {
            FilePath = _path;
            Extension = _ext;
            if(!Directory.Exists(FilePath))
            {
                Directory.CreateDirectory(FilePath);
                Debug.Log("DataManager : フォルダを新規作成しました" + FilePath);
            }
        }

        // ファイルorフォルダが存在するか？
        public bool IsFile(string _str)
        {
            return File.Exists(FilePath + _str + Extension);
        }

        public void CreateDirectory(string _str)
        {
            if(!IsFile(_str))
            {
                Directory.CreateDirectory(FilePath + _str + Extension);
            }
        }

        public string[] FileList()
        {   // フォルダorファイル名を取得。
            string[] _strs = Directory.GetFiles(FilePath, "*" + Extension);
            for(int i = 0; i < _strs.Length; i++)
            {
                _strs[i] = Path.GetFileNameWithoutExtension(_strs[i]);
            }
            return _strs;
        }

        // csv
        public bool SaveCSV(string[][] _contens, string _fileName = "temp.csv")
        {
            StreamWriter _steram = new StreamWriter(FilePath + _fileName + Extension, false, Encoding.GetEncoding("Shift_JIS"));
            for(int i = 0; i < _contens.Length; i++)
            {
                string _str = string.Join(",", _contens[i]);
                _steram.WriteLine(_str);
            }
            _steram.Close();
            return true;
        }

        public bool LoadCSV(ref string[][] _content, string _fileName = "temp.csv")
        {
            if(IsFile(_fileName))
            {
                StreamReader _stream = new StreamReader(FilePath + _fileName + Extension);
                string[] _dataLines = _stream.ReadToEnd().Replace("\r\n", "\n").Split(new[]{'\n', '\r'});
                _stream.Close();

                _content = new string[_dataLines.Length][];
                for (int i = 0; i < _dataLines.Length; i++)
                {
                    _content[i] = _dataLines[i].Split(',');
                }
                return true;
            }
            return false;
        }

        // json
        public bool SaveJson<Type>(Type _type, string _fileName = "temp.json")
        {
            string _json = JsonUtility.ToJson(_type);
            StreamWriter _stream = new StreamWriter(FilePath + _fileName + Extension, false);
            _stream.Write(_json);
            _stream.Flush();
            _stream.Close();
            return true;
        }

        public bool LoadJson<Type>(ref Type _type, string _fileName = "temp.json")
        {
            if(IsFile(_fileName))
            {
                StreamReader _stream = new StreamReader(FilePath + _fileName + Extension);
                string _data = _stream.ReadToEnd();
                _stream.Close();
                _type = JsonUtility.FromJson<Type>(_data);
                return true;
            }
            return false;
        }
    }
}