using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace EasyTools
{
    public class SpriteReferenceWindow : EditorWindow
    {
        private Texture2D tarObj;
        private List<string> fileList = new List<string>();
        private Dictionary<string, UnityEngine.Object> objDict = new Dictionary<string, UnityEngine.Object>();

        private Vector2 scrollPos;
        private bool isAutoCheck;

        [MenuItem("Tools/UI/检测Sprite引用")]
        public static void CheckSpriteReference()
        {
            SpriteReferenceWindow window = (SpriteReferenceWindow) EditorWindow.GetWindow(typeof(SpriteReferenceWindow));
            window.autoRepaintOnSceneChange = true;
            window.titleContent = new GUIContent("检测Sprite引用对象");
            window.Show();

            window.tarObj = Selection.activeObject as Texture2D;
        }

        private void Check()
        {
            if (tarObj == null)
            {
                Debug.LogError("检测对象为空");
                return;
            }

            var assetPath = AssetDatabase.GetAssetPath(tarObj);
            var guid = AssetDatabase.AssetPathToGUID(assetPath);

            fileList.Clear();

            foreach (var fileName in EasyToolsData.Instance.AllUIPrefabData.Data.Keys)
            {
                var filePath = string.Format("{0}/{1}", EasyToolsData.UI_PREFAB_RELATIVE_PATH, fileName);

                if (EasyToolsData.Instance.AllUIPrefabData.Data[fileName].RefGuids.Contains(guid))
                {
                    if (!fileList.Contains(filePath))
                    {
                        fileList.Add(filePath);
                    }
                }
            }

            Debug.Log("SpriteReferenceWindow检测引用数：" + fileList.Count);
        }

        void OnInspectorUpdate()
        {
            Repaint();
        }

        public void OnGUI()
        {
            EditorGUILayout.BeginVertical();

            EditorGUILayout.BeginHorizontal();

            tarObj = EditorGUILayout.ObjectField(tarObj, typeof(Texture2D), true) as Texture2D;

            if (GUILayout.Button("检测"))
            {
                Check();
            }

            isAutoCheck = GUILayout.Toggle(isAutoCheck, "自动");

            if (isAutoCheck)
            {
                if (Selection.activeObject is Texture2D)
                {
                    var obj = Selection.activeObject as Texture2D;
                    if (tarObj != obj)
                    {
                        tarObj = obj;
                        Check();
                    }
                }
            }

            EditorGUILayout.EndHorizontal();

            string msg = "1.当前为检测UIAtals中的Sprite在UIPrefab中的引用对象\n2.选中“自动”将会自动判断检测当前Inspector的Sprite\n3. 注意：窗口失去焦点没法及时刷新";

            EditorGUILayout.HelpBox(msg, MessageType.Info);

            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Width(800), GUILayout.Height(500));

            foreach (var filePath in fileList)
            {
                if (GUILayout.Button(filePath))
                {
                    if (!objDict.ContainsKey(filePath))
                    {
                        objDict[filePath] = AssetDatabase.LoadAssetAtPath(filePath, typeof(UnityEngine.Object));
                    }
                    Selection.activeObject = objDict[filePath];
                }
            }

            EditorGUILayout.EndScrollView();

            EditorGUILayout.EndVertical();

        }

        public void OnDestroy()
        {
            fileList = null;
            objDict = null;
            Resources.UnloadUnusedAssets();
        }

    }
}