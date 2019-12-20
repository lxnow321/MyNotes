using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace EasyTools
{
    public class EasyUITools
    {
        [MenuItem("Tools/UI/图集引用数检测(先选中Prefab或GameObject)")]
        public static void CheckUISpriteRefCount()
        {
            HashSet<string> _spritePackingTags = new HashSet<string>();
            Dictionary<string,string> _spritePackingUserDic = new Dictionary<string,string>();
            var go = Selection.activeGameObject;
            var trans = go.transform;
            string fullPath = "";
            Transform parent = null;
            if (go != null)
            {
                var name = go.name;
                var components = go.GetComponentsInChildren(typeof(Image), true);
                if (components != null)
                {
                    foreach (var cmp in components)
                    {
                        var image = cmp as Image;
                        if (image && image.sprite)
                        {
                            var assetPath = AssetDatabase.GetAssetPath(image.sprite.texture);
                            if (!string.IsNullOrEmpty(assetPath))
                            {
                                var importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
                                if (importer && !string.IsNullOrEmpty(importer.spritePackingTag))
                                {
                                    _spritePackingTags.Add(importer.spritePackingTag);
                                    fullPath = image.name;
                                    parent = cmp.transform.parent;
                                    while(parent!=null && parent != trans){
                                        fullPath = parent.name+"/"+fullPath;
                                        parent = parent.parent;
                                    }
                                    if (_spritePackingUserDic.ContainsKey(importer.spritePackingTag)){
                                        _spritePackingUserDic[importer.spritePackingTag] += "\n"+fullPath;
                                    }else
                                    {
                                        _spritePackingUserDic[importer.spritePackingTag] = fullPath;
                                    }
                                }
                            }
                        }
                    }
                }

                Debug.Log(string.Format("图集引用统计：<color=#ffff00>{0}</color> 包含 <color=#ffff00>{1}</color> 个图集引用", name, _spritePackingTags.Count));
                foreach (var tag in _spritePackingTags)
                {
                    Debug.Log(string.Format("{0}包含图集：<color=#ffff00>{1}</color>\n{2}", name, tag,_spritePackingUserDic[tag]));
                }
            }
            else
            {
                Debug.LogError("请选中Prefab或GameObject");
            }
        }

        #if UNITY_STANDALONE_WIN
        [MenuItem("Assets/[EasyTools] 图片加文件夹前缀(仅限common_前缀)")]
        #else
        [MenuItem("Assets/[AoUnity] Build References Database")]
        #endif
        static public void AddImagePrefix()
        {
            string[] strs = Selection.assetGUIDs;
            for (int i = 0; i < strs.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(strs[i]);
                if (path.IndexOf(".png") > -1)
                {
                    ChangeImageName(path);
                }
                else
                {
                    DirectoryInfo direction = new DirectoryInfo(path);
                    SearchOption option = SearchOption.AllDirectories;
                    FileInfo[] files = direction.GetFiles("*.png", option);
                    for (int index = 0; index < files.Length; index++)
                    {

                        FileInfo file = files[index];
                        string assetPath = file.FullName.Substring(EasyToolsData.DATA_PATH.Length).Replace("\\", "/");
                  
                        ChangeImageName(assetPath);
                    }
                }          
            }
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();


        }

        public static void ChangeImageName(string path)
        {
            string folderPath = GetFolderPath(path);
            string shortName = GetPathShortName(path);
            string assetPath = path.Substring(EasyToolsData.UI_ATLAS_RELATIVE_PATH.Length + 1);
          
            int prefixIndex = assetPath.IndexOf("/");

            if (prefixIndex > -1 && shortName.IndexOf("common_") == 0)
            {
                string prefixPath = assetPath.Substring(0, prefixIndex).ToLower();
                if(prefixPath=="common")
                {
                    return;
                }
                string newPathName = prefixPath + "_" + shortName;
                Debug.Log("path:" + path + " newPathName:" + newPathName);
                AssetDatabase.RenameAsset(path, newPathName);
            }
        }

    

        public static string GetFolderPath(string file)
        {
            string filePrefixPath = string.Empty;
            int index = file.LastIndexOf("/");
            if (index > -1)
            {
                filePrefixPath = file.Substring(0, index);
            }
            int index1 = filePrefixPath.LastIndexOf("/");
            if (index1 > -1)
            {
                filePrefixPath = filePrefixPath.Substring(index1+1);
            }
            return filePrefixPath.ToLower();
        }

        public static string GetPathShortName(string path)
        {
            string fileShortName = string.Empty;
            int index = path.LastIndexOf("/");
            if (index > -1)
            {
                fileShortName = path.Substring(index + 1);
            }
            return fileShortName;
        }
    }
}