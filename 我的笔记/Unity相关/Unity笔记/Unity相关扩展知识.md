# Unity相关扩展知识

## 1.如何调试UGUI，学习UGUI源码

- 下载UGUI源码 https://bitbucket.org/Unity-Technologies/ui/downloads/?tab=tags。注意：下载自己Unity版本对应的源码，否则可能有错误
- 将所下载的源码中UnityEditor.UI和UnityEngine.UI拷贝到工程下的Assets目录下。并删除这两个目录中的bin、obj和Properties文件夹，以及.csproj文件。这些是不必要的文件。另外将UnityEditor.UI这个文件夹改名未Editor。因为Unity中规定编辑器实现类必须以Editor命名。
- 将Unity安装目录中Unity\Editor\Data\UnityExtensions\Unity下的GUISystem备份好后，从该目录删除
- 重启Unity即可使用源码执行并调试UGUI