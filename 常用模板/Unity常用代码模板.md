Unity常用代码模板

## Unity编辑器常用代码

## 注意：Unity内部接口所传入或返回的路径，都为Assets下目录的相对路径，如:Assets/xxx.file


## 工具常用Unity类

AssetDatabase //访问assets和可在assets上执行操作的接口
EditorApplication //编辑器应用类，可获取编辑器相关状态等
Application //访问应用的运行时数据
Selection //访问editor中选择项
AssetPostprocessor //assets后置处理器
AssetModificationProcessor //assets修改处理器

--编辑器或发布程序使用
GUI
GUILayout
GUILayoutUtility
--编辑器用
EditorGUI
EditorGUILayout
EditorGUIUtilty

EditorWindow //创建编辑器窗口
EditorUtility //编辑器工具类

### 获得工程目录绝对路径
var absolutePath = Application.dataPath + '/path';

Application.dataPath 包含了"Assets"，故path为Assets下的相对路径

### 获取指定目录的符合条件的所有文件guid

var path = "Assets/..."; //必须是Assets下的某个目录，传相对路径
var guids = AssetDatabase.FindAssets("t:Texture", new[] { path, .. }); //此处举例“t:Texture”获得所有Texture文件，不需要可以不传

### 文件路径 转 guid
AssetDatabase.AssetPathToGUID

### guid 转 文件路径
AssetDatabase.GUIDToAssetPath

## 编辑器增加自定义菜单

class MyMenuClass
{
	[MenuItem("测试/测试")]
	public static void Test()
	{
		//方法体
	}
}


## 遍历获取编辑器中选中的对象或文件夹

public static void SearchSelection()
{
	UnityEngine.Object[] objs = Selection.GetFiltered(typeof(GameObject), SelectionMode.DeepAssets); //DeepAssets遍历选中的所有子文件夹
	foreach(var obj in objs)
	{
		var target = obj as GameObject;
	}
}


## 遍历指定文件夹资源

public static void SearchAssets()
{
	string searchPath = "Assets\\...";
	string[] guids = AssetDatabase.FindAssets("t:Prefab", new string[]{searchPath,});
	foreach (var guid in guids)
	{
		string assetPath = AssetDatabase.GUIDToAssetPath(guid);
		var prefab = AssetDatabase.LoadAssetAtPath(assetPath, typeof(GameObject)) as GameObject; //获取对应asset
		var importer = AssetImporter.GetAtPath(assetPath); //获取对应assetimporter
	}
}


## 检测是否开启raycastTarget Gizemo绘制蓝框展示

#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class DebugUILine : MonoBehaviour
{
    static Vector3[] fourCorners = new Vector3[4];
    void OnDrawGizmos()
    {
        Debug.LogError("test");
        foreach (MaskableGraphic g in GameObject.FindObjectsOfType<MaskableGraphic>())
        {
            if (g.raycastTarget)
            {
                RectTransform rectTransform = g.transform as RectTransform;
                rectTransform.GetWorldCorners(fourCorners);
                Gizmos.color = Color.blue;
                for (int i = 0; i < 4; i++)
                    Gizmos.DrawLine(fourCorners[i], fourCorners[(i + 1) % 4]);

            }
        }
    }
}
#endif



