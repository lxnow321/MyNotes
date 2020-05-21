# Sprite同图集但在不同bundle中

测试：
将两个文件夹中的sprite设置为同一个PackingTag，使用SpritePacker工具将其打入同一个图集中。这两个文件夹设置不同的AssetBundleName，使其处于不同的bundle中。
打出的ab中，ab1和ab2都包含有完整的图集，即有总共有两份图集的拷贝。


测试代码：

using System.IO;
using UnityEditor;
using UnityEngine;

public class BuildPipelineTest : MonoBehaviour
{
	[MenuItem("测试/打ab")]
	static void Build()
	{
		var path = "Assets/test";
		Directory.Delete(path, true); //注意，BuildAssetBundles不会自动删除旧的ab，只会覆盖，所以需要自己手动清理
		if (!Directory.Exists(path))
		{
			Directory.CreateDirectory("Assets/test");
		}
		BuildPipeline.BuildAssetBundles(path, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);
	}
}