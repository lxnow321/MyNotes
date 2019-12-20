using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using LitJson;
using UnityEditor;
using UnityEngine;

namespace EasyTools
{
    public class UIPrefabInfo
    {
        public List<string> RefGuids { get; set; }
        public string TimeStr { get; set; }
    }

    public class AllUIPrefabInfo
    {
        public Dictionary<string, UIPrefabInfo> Data { get; set; }
    }

    public class EasyToolsData : AssetPostprocessor
    {
        private static EasyToolsData _instance;
        public static EasyToolsData Instance
        {
            get
            {
                if (_instance == null)
                {
                    Debug.Log("EasyToolsData:单例初始化");
                    _instance = new EasyToolsData();
                    _instance.InitData();
                }
                return _instance;
            }
        }

        public static readonly string DATA_PATH = Application.dataPath.Replace("Assets", "");
        public static readonly string FILE_PATH = Application.dataPath + "/Temp/EasyTools/AllUIPrefabInfo.json";
        public const string UI_PREFAB_RELATIVE_PATH = "Assets/GameAssets/Prefabs/UI";
        public const string UI_ATLAS_RELATIVE_PATH = "Assets/GameAssets/Texture/UIAtlas";
        public static readonly string UI_PREFAB_PATH = DATA_PATH + UI_PREFAB_RELATIVE_PATH;
        public static readonly string UI_ATLAS_PATH = DATA_PATH + UI_ATLAS_RELATIVE_PATH;

        private int _diffCount = 0;
        public int DiffCount
        {
            get { return _diffCount; }
            set
            {
                _diffCount = value;
                if (_diffCount >= UIPrefabCount * 0.1)
                {
                    //当差异量大于原有的10%，则触发一次保存
                    SaveJsonData();
                }
            }
        }

        private int _uiPrefabCount = 0;
        public int UIPrefabCount
        {
            get
            {
                if (_allUIPrefabData != null && _allUIPrefabData.Data != null)
                {
                    return _allUIPrefabData.Data.Count;
                }
                return 0;
            }
        }

        private void InitData()
        {
            _allUIPrefabData = _ReadJsonData();
            RefreshJsonData();
        }

        private AllUIPrefabInfo _allUIPrefabData;

        public AllUIPrefabInfo AllUIPrefabData
        {
            get
            {
                return _allUIPrefabData;
            }
        }

        private static bool _IsUIPrefabPath(string path)
        {
            return path.EndsWith(".prefab") && path.Contains(UI_PREFAB_RELATIVE_PATH);
        }

        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            if (EditorPrefs.GetBool("SkipEasyToolProcess",false))
            {
                Debug.Log("Skip EasyTool to save time");
                return;
            }
            
            foreach (var assetPath in importedAssets)
            {
                if (_IsUIPrefabPath(assetPath))
                {
                    var uiPrefabInfo = new UIPrefabInfo();
                    FileInfo fileInfo = new FileInfo(Application.dataPath.Replace("Assets", "") + assetPath);
                    uiPrefabInfo.TimeStr = fileInfo.LastWriteTime.ToString();
                    uiPrefabInfo.RefGuids = _GetGuids(fileInfo.FullName);

                    var fileName = assetPath.Substring(UI_PREFAB_RELATIVE_PATH.Length + 1).Replace("\\", "/");
                    Instance.AllUIPrefabData.Data[fileName] = uiPrefabInfo;
                    Instance.DiffCount++;
                }
            }

            foreach (var assetPath in deletedAssets)
            {
                if (_IsUIPrefabPath(assetPath))
                {
                    var fileName = assetPath.Substring(UI_PREFAB_RELATIVE_PATH.Length + 1);
                    Instance.AllUIPrefabData.Data.Remove(fileName);
                    Instance.DiffCount++;
                }
            }

            for (int i = 0; i < movedFromAssetPaths.Length; ++i)
            {
                var assetPath = movedFromAssetPaths[i];
                if (_IsUIPrefabPath(assetPath))
                {
                    if (movedAssets.Length > i && _IsUIPrefabPath(movedAssets[i]))
                    {
                        //UIPrefab目录内部移动
                        var moveFromFileName = assetPath.Substring(UI_PREFAB_RELATIVE_PATH.Length + 1);
                        var moveFileName = movedAssets[i].Substring(UI_PREFAB_RELATIVE_PATH.Length + 1);

                        if (Instance.AllUIPrefabData.Data.ContainsKey(moveFromFileName))
                        {
                            Instance.AllUIPrefabData.Data[moveFileName] = Instance.AllUIPrefabData.Data[moveFromFileName];
                            Instance.AllUIPrefabData.Data.Remove(moveFromFileName);
                        }
                        else
                        {
                            var uiPrefabInfo = new UIPrefabInfo();
                            FileInfo fileInfo = new FileInfo(Application.dataPath.Replace("Assets", "") + movedAssets[i]);
                            uiPrefabInfo.TimeStr = fileInfo.LastWriteTime.ToString();
                            uiPrefabInfo.RefGuids = _GetGuids(fileInfo.FullName);
                            Instance.AllUIPrefabData.Data[moveFileName] = uiPrefabInfo;
                        }
                    }
                    else
                    {
                        //非UIPrefab目录内部移动，当做删除
                        var fileName = assetPath.Substring(UI_PREFAB_RELATIVE_PATH.Length + 1);
                        Instance.AllUIPrefabData.Data.Remove(fileName);
                    }
                    Instance.DiffCount++;
                }
            }
        }

        public void ClearJsonData()
        {
            _allUIPrefabData = null;
        }

        public bool IsFileChanged(string fileName, string curTimeStr)
        {
            UIPrefabInfo guidData;
            AllUIPrefabData.Data.TryGetValue(fileName, out guidData);
            if (guidData != null)
            {
                return guidData.TimeStr != curTimeStr;
            }
            return true;
        }

        public void SaveJsonData(AllUIPrefabInfo allInfo = null)
        {
            if (allInfo == null)
            {
                allInfo = AllUIPrefabData;
            }
            _diffCount = 0;
            _WriteJsonData(allInfo);
        }

        public static List<string> _GetGuids(string prefabFilePath)
        {
            List<string> guids = new List<string>();

            string str = File.ReadAllText(prefabFilePath, System.Text.Encoding.Default);
            var matchs = Regex.Matches(str, "guid:.*?([a-z0-9]*),", RegexOptions.IgnoreCase);
            foreach (Match match in matchs)
            {
                if (match != null && match.Groups != null && match.Groups.Count > 1)
                {
                    var guid = match.Groups[1].Value;
                    if (!guids.Contains(guid))
                    {
                        guids.Add(guid);
                    }
                }
            }
            return guids;
        }

        public AllUIPrefabInfo _GenerateAllUIPrefabData()
        {
            var allUIPrefabData = new AllUIPrefabInfo();
            var dirInfo = new DirectoryInfo(UI_PREFAB_PATH);
            FileInfo[] fileInfos = dirInfo.GetFiles("*.prefab", SearchOption.AllDirectories);

            var allData = new Dictionary<string, UIPrefabInfo>();

            foreach (var fileInfo in fileInfos)
            {
                var timeStr = fileInfo.LastWriteTime.ToString();
                var fileName = fileInfo.FullName.Substring(UI_PREFAB_PATH.Length + 1).Replace("\\", "/");

                var guidsData = new UIPrefabInfo();
                guidsData.TimeStr = timeStr;
                guidsData.RefGuids = _GetGuids(fileInfo.FullName);
                allData[fileName] = guidsData;
            }
            allUIPrefabData.Data = allData;

            Debug.Log("EasyToolsData:_GenerateAllUIPrefabData生成初始数据");

            SaveJsonData(allUIPrefabData);

            return allUIPrefabData;
        }

        public void RefreshJsonData()
        {
            //根据文件的写入日期对比进行更新
            bool hasChanged = false;
            var dirInfo = new DirectoryInfo(UI_PREFAB_PATH);
            FileInfo[] fileInfos = dirInfo.GetFiles("*.prefab", SearchOption.AllDirectories);

            var allFileNames = new List<string>();

            foreach (var fileInfo in fileInfos)
            {
                var timeStr = fileInfo.LastWriteTime.ToString();
                var fileName = fileInfo.FullName.Substring(UI_PREFAB_PATH.Length + 1).Replace("\\", "/");

                if (IsFileChanged(fileName, timeStr))
                {
                    var guidsData = new UIPrefabInfo();
                    guidsData.TimeStr = timeStr;
                    guidsData.RefGuids = new List<string>();

                    string str = File.ReadAllText(fileInfo.FullName, System.Text.Encoding.Default);
                    var matchs = Regex.Matches(str, "guid:.*?([a-z0-9]*),", RegexOptions.IgnoreCase);
                    foreach (Match match in matchs)
                    {
                        if (match != null && match.Groups != null && match.Groups.Count > 1)
                        {
                            var guid = match.Groups[1].Value;
                            if (!guidsData.RefGuids.Contains(guid))
                            {
                                guidsData.RefGuids.Add(guid);
                            }
                        }
                    }
                    AllUIPrefabData.Data[fileName] = guidsData;
                    hasChanged = true;
                }

                allFileNames.Add(fileName);
            }

            var delHaskSet = new HashSet<string>();
            foreach (var fileName in AllUIPrefabData.Data.Keys)
            {
                if (!allFileNames.Contains(fileName))
                {
                    delHaskSet.Add(fileName);
                    hasChanged = true;
                }
            }
            foreach (var delKey in delHaskSet)
            {
                AllUIPrefabData.Data.Remove(delKey);
            }

            if (hasChanged)
            {
                SaveJsonData();
            }
        }

        private AllUIPrefabInfo _ReadJsonData()
        {
            string json = null;
            try
            {
                json = File.ReadAllText(FILE_PATH, System.Text.Encoding.Default);
            }
            catch (Exception e)
            {
                Debug.Log("EasyToolsData:文件读取异常:" + e.Message);
            }

            AllUIPrefabInfo dataObject = null;
            try
            {
                dataObject = LitJson.JsonMapper.ToObject<AllUIPrefabInfo>(json);
            }
            catch (Exception e)
            {
                Debug.Log("EasyToolsData:Json文件反序列化异常:" + e.Message);
            }

            if (dataObject == null)
            {
                dataObject = _GenerateAllUIPrefabData();
            }

            return dataObject;
        }

        private void _WriteJsonData(AllUIPrefabInfo dataObject)
        {
            var sb = new StringBuilder();
            var jw = new JsonWriter(sb);
            jw.PrettyPrint = true;
            LitJson.JsonMapper.ToJson(dataObject, jw);

            var dirName = Path.GetDirectoryName(FILE_PATH);
            if (!Directory.Exists(dirName))
            {
                Directory.CreateDirectory(dirName);
            }
            File.WriteAllText(FILE_PATH, sb.ToString(), Encoding.UTF8);
            Debug.Log("EasyToolsData:写入数据:" + FILE_PATH);
        }
    }
}