using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace EasyTools
{
    public class UnuseSpriteWindow : EditorWindow
    {

        private List<string> pathList = new List<string>();
        private HashSet<string> refGuids = new HashSet<string>();
        private HashSet<string> delList = new HashSet<string>();
        private string infoMsg = "";

        private Dictionary<string, UnityEngine.Object> objDict = new Dictionary<string, UnityEngine.Object>();

        private Vector2 scrollPos;

        [MenuItem("Tools/UI/检测未被引用的Sprite(不包含Dynamic)")]
        public static void CheckSpriteReference()
        {
            UnuseSpriteWindow window = (UnuseSpriteWindow) EditorWindow.GetWindow(typeof(UnuseSpriteWindow));
            window.autoRepaintOnSceneChange = true;
            window.titleContent = new GUIContent("检测未被引用的Sprite(不包含Dynamic)");
            window.Show();
            window.Check();
        }

        private void Check()
        {
            pathList.Clear();
            refGuids.Clear();
            delList.Clear();

            foreach (var data in EasyToolsData.Instance.AllUIPrefabData.Data.Values)
            {
                foreach (var guid in data.RefGuids)
                {
                    refGuids.Add(guid);
                }
            }

            string[] guids = AssetDatabase.FindAssets("t:Sprite", new string[] { "Assets\\GameAssets\\Texture\\UIAtlas", });
            var exceptGuids = AssetDatabase.FindAssets("t:Sprite", new string[] { "Assets\\GameAssets\\Texture\\UIAtlas\\Dynamic", });

            var guidsHashSet = new HashSet<string>(guids);
            var totalCount = guidsHashSet.Count;
            guidsHashSet.ExceptWith(exceptGuids);

            guidsHashSet.ExceptWith(refGuids);

            infoMsg = string.Format("统计: Sprite总数：{0} Dynamic剔除数：{1}",
                totalCount, exceptGuids.Length);

            Debug.Log(string.Format("统计: Sprite总数：<color=#ffff00>{0}</color> Dynamic剔除数：<color=#ffff00>{1}</color> 未被引用Sprite总数：<color=#ffff00>{2}</color>",
                totalCount, exceptGuids.Length, guidsHashSet.Count));

            foreach (var guid in guidsHashSet)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                pathList.Add(path);
            }

        }

        private void OnGUI()
        {
            EditorGUILayout.BeginVertical();

            string msg = "1.检测UIAtals中未被任何UIPrefab引用的Sprite\n2.注意：Dynamic目录下的Sprite不属于检测范围";

            EditorGUILayout.HelpBox(msg, MessageType.Info);

            GUI.contentColor = Color.green;

            GUILayout.Label(infoMsg, GUILayout.Width(700));

            EditorGUILayout.BeginHorizontal();

            GUI.contentColor = Color.yellow;
            GUILayout.Label(string.Format("当前未被引用Sprite总数：{0}", pathList.Count), GUILayout.Width(700));
            GUI.contentColor = Color.white;

            if (GUILayout.Button("全部", GUILayout.Width(50)))
            {
                delList = new HashSet<string>(pathList);
            }

            if (GUILayout.Button("取消", GUILayout.Width(50)))
            {
                delList.Clear();
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Width(800), GUILayout.Height(500));

            foreach (var filePath in pathList)
            {
                EditorGUILayout.BeginHorizontal();

                bool needDel = delList.Contains(filePath);

                GUI.color = needDel ? Color.red : Color.white;
                if (GUILayout.Button(filePath))
                {
                    if (!objDict.ContainsKey(filePath))
                    {
                        objDict[filePath] = AssetDatabase.LoadAssetAtPath(filePath, typeof(UnityEngine.Object));
                    }
                    Selection.activeObject = objDict[filePath];
                }

                if (!needDel)
                {
                    if (GUILayout.Button("删除", GUILayout.Width(50)))
                    {
                        delList.Add(filePath);
                    }
                }
                else
                {

                    if (GUILayout.Button("取消", GUILayout.Width(50)))
                    {
                        delList.Remove(filePath);
                    }
                }
                GUI.color = Color.white;

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndScrollView();

            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();

            GUILayout.Space(700);
            if (delList.Count > 0)
            {
                if (GUILayout.Button(string.Format("应用(删除:{0})", delList.Count), GUILayout.Width(100), GUILayout.Height(50)))
                {
                    foreach (var path in delList)
                    {
                        objDict.Remove(path);
                        pathList.Remove(path);
                        AssetDatabase.DeleteAsset(path);
                    }
                    delList.Clear();
                }
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
        }

        public void OnDestroy()
        {
            pathList = null;
            objDict = null;
            Resources.UnloadUnusedAssets();
        }
    }
}