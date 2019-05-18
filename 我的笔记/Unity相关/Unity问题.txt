Unity问题

1.弃用WWW，使用UnityWebRequest请求网络资源
WWW:
This is a small utility module for retrieving the contents of URLs.
这是一个小的实用模块用于从URL上获取内容

UnityWebRequest：
The UnityWebRequest object is used to communicate with web servers.
UnityWebRequest对象被用于与web服务器通信


2.AssetBundle:
AssetBundle只是一种使用LZMA压缩方式压缩的资源文件

AssetBundle在不同平台是不兼容的

Unity已经推荐使用UnityWebRequest类来代替WWW从网络上获取资源

加载AssetBundle
(对象方法)
ab.LoadAllAssets    同步加载bundle中所有的资源 根据type参数类型
ab.LoadAllAssetsAsync  异步
ab.LoadAsset        从bundle中加载资源 通过带扩展名的相对路径， 可以带参数type
ab.LoadAssetAsync  异步
ab.LoadAssetWithSubAssets    从bundle中加载资源和子资源
ab.LoadAssetWithSubAssetsAsync  异步



（静态方法，返回AssetBundle对象）
AssetBundle.LoadFromeFile    同步在磁盘文件中加载
AssetBundle.LoadFromeFileAsync  异步
AssetBundle.LoadFromeMemory    同步在内存中加载
AssetBundle.LoadFromeMemoryAsync  异步
2017以后应该还有
AssetBundle.LoadFromeStream   同步在托管流中加载
AssetBundle.LoadFromeStreamAsync  异步

卸载AssetBundle
AssetBundle.Unload(bool unloadAllLoadedObjects) 如果unloadAllLoadedObjects为false，那么只卸载bundle内捆绑的资源，而不卸载已从该bundle中加载的对象，且不能再通过该bundle进行加载。unloadAllLoadeObjects为ture时，卸载全部，包含已经从该bundle加载的对象。
以及从AssetBundle中加载的资源可以通过Resource.UnloadAsset(Object)卸载。如果想通过Resouces.UnloadUnuseAssets()卸载从AssetBundle加载的资源，一定要先将AssetBundle卸载后才能生效

生成AssetBundle
BuildPipeline.BuildAssetBundles()


资源meta文件中的assetBundleName 和 assetBundleVariant
assetBundleName不必多说，assetBundleVariant为变体名

AssetBundle不是资源组件，故无法用资源组件的方式载入，只能使用WWWW或者AssetBundle相关接口载入与读取

其他加载方式：
AssetDatabase.LoadAssetAtPath(Assets/x.txt) 等，只能在编辑器模式下使用，游戏内无法运作
Resources.Load("fileName") fileName是相对于Resource目录下的文件路径与名称，不包含后缀。Assets目录下可以拥有任意路径和数量的Resrouces文件夹，在运行时，Resources下的文件路径将被合并。如果存在相对路径相同且名称相同的资源（后缀不同），那么Load方法只会加载第一个满足条件的资源。想要加载所有符合条件的资源使用Resources.LoadAll("fileName")。在工程进行打包后，Resrouces文件夹中的资源将进行加密和压缩，打包后的程序内将不存在Resouce文件夹，故无法通过路径访问以及更新资源。

3.LZMA 与 LZ4

LZMA，（Lempel-Ziv-Markov chain-Algorithm的缩写），是一个Deflate和LZ77算法改良和优化后的压缩算法，开发者是Igor Pavlov，2001年被首次应用于7-Zip压缩工具中，是 2001年以来得到发展的一个数据压缩算法。它使用类似于 LZ77 的字典编码机制，在一般的情况下压缩率比 bzip2 为高，用于压缩的可变字典最大可达4GB。



压缩比：
LZMA -e < LZMA < LZ4

压缩时间:
LZ4 < LZMA < LZMA -e

解压时间：
LZ4 < LZMA -e < LZMA

压缩请求的内存：
LZ4 < LZMA < LZMA -e  LZ4偏稳定，LZMA和LZMA-e在压缩级越高时，所需内存上升比较快


解压请求的内存：

LZMA-E = LZMA < LZ4


4.公司使用项目的资源压缩方式选择
LZ4:  shader,uiatlas,uifont,uiprefab,uitexture

LZMA： audio,config,effect,image,model,scene


LZ4 压缩和解压比较快，应用与需要快速加载的资源压缩上会更适合一些，而LZMA相对压缩率会比较高，故适用于体积较大，但对压缩和解压速度要求不那么高的资源文件。



5.备份中的ab资源，有加入crc后缀。当前的ab资源无crc后缀，这是备份机制中加上去的。


6.打AB时常用的几个操作类 
BuildPipeline  (Lets you programmatically build players or AssetBundles which can be loaded from the web.) 能让你用编程的方式生成应用或者可从web中加载的资源包
AssetDatabase    (An Interface for accessing assets and performing operations on assets.)用于访问资源和操作资源的接口
AssetImporter  (Base class from which asset importers for specific asset types derive.) 特定资源类型导入器的基类


*GUID和fileID(本地ID)
GUID用于确定资源间的依赖关系
fileID用于确定资源内部的依赖关系

*InstanceID(实例ID)



*资源目录
Resource目录
StreamingAssets:
StreamingAssets文件夹为流媒体文件夹，此文件夹的资源将不会经过压缩与加密，原封不动的打包进游戏包内。在游戏安装时，StreamAssets文件夹内的资源将根据平台，移动到对应的文件夹内。StreamingAssets文件夹在Android和IOS平台上为只读文件夹
Application.streamingAssetsPath
Application.dataPath + "/StreamingAssets"   windows或mac
Application.dataPath + "/Raw"      ios
"jar:file://" + Application.dataPath + "/assets/"   android
StreamingAssets文件夹下的文件在游戏中只能通过io stream或者www的方式读取(asset budnle除外)

PersitentDataPath：
Application.presistentDataPath Unity指定的一个可读写的外部文件夹，该路径因平台及系统配置不同而不同。可以用来保存数据及文件。该目录下的资源不会打包时被打入包中，也不会自动被Unity导入及转换。该文件夹只能通过io stream以及www的方式进行资源加载。

在windows平台下unity中使用Directory.GetFiles等获取相对路径的文件,放到mac平台下会找不到文件，需要使用完全路径。

Application.dataPath 获取的路径是游戏目录下的Assets文件目录，如果想要获取上一层，需要使用Path.GetDirectoryName(Application.dataPath)


*Unity日志目录

Mac OS X	~/Library/Logs/Unity/Editor.log
Windows XP *	C:\Documents and Settings\username\Local Settings\Application Data\Unity\Editor\Editor.log
Windows Vista/7 *	C:\Users\username\AppData\Local\Unity\Editor\Editor.log


*MenuItem的使用
public MenuItem(string itemName);
public MenuItem(string itemName, bool isValidateFunction);
public MenuItem(string itemName, bool isValidateFunction, int priority);

MenuItem("菜单项") 在菜单中添加一个菜单新项
MenuItem("菜单项", ture) 表示该方法是用于控制该菜单项是否有效，注意方法返回bool
MenuItem("菜单项", false, priority) priority表示菜单中的显示优先级顺序。
注意：如果不适用priority，貌似在GameObject/目录下添加的菜单项不能够正常的在Hierarchy中右击gameObject显示出来，可尝试加上priority试试。


*MeshFilter与MeshRenderer分别是什么，它们之间有什么关系？

Mesh Filter 网格过滤器 
用于从资源中读取网格信息也就是Mesh ，读取信息之后可以传递给 Mesh Renderer 。 
Mesh Renderer 网格渲染器 
将接受到的网格信息渲染出来


*AssetPostprocessor 与 AssetModificationProcessor

AssetPostprocessor:
AssetPostprocessor lets you hook into the import pipeline and run scripts prior or after importing assets.
AssetPostprocessor可以在资源导入之前或导入之后加入钩子执行脚本。

AssetModificationProcessor：
AssetModificationProcessor lets you hook into saving of serialized assets and scenes which are edited inside Unity.

AssetModificationProcessor允许在unity内部编辑的序列化存储的文件或场景中加入钩子