Unity常用代码模板

## 1.编辑器增加自定义菜单

class MyMenuClass
{
	[MenuItem("测试/测试")]
	public static void Test()
	{
		//方法体
	}
}


## 2.遍历获取编辑器中选中的对象或文件夹

public static void SearchSelection()
{
	UnityEngine.Object[] objs = Selection.GetFiltered(typeof(GameObject), SelectionMode.DeepAssets); //DeepAssets遍历选中的所有子文件夹
	foreach(var obj in objs)
	{
		var target = obj as GameObject;
	}
}


## 3.遍历指定文件夹资源

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