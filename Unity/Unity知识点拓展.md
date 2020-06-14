# Unity知识点拓展


## GUI

UnityEngine: //可在编辑器或发布后的程序中使用

GUI
GUILayout (The GUILayout class is the interface for Unity gui with automatic layout.)
GUILayoutUtility (Utility functions for implementing and extending the GUILayout class.)


UnityEditor: //编辑器使用

EditorGUI
EditorGUILayout ->GUIUtility  (Auto-layouted version of EditorGUI.)
EditorGUIUtilty  ->GUIUtility (Miscellaneous helper stuff for EditorGUI.)


Editor ->ScriptableObject
EditorWindow ->ScriptableObject

GUIUtility
EditorUtility  (Editor utility functions.)


注意：
带Layout与不带Layout的区别在于，带Layout的控件有自动布局，不需要指定位置信息，自动布局比较省事。二不带Layout的控件需要传参Rect用于指定控件所在位置，自由度比较高。
如：
GUI.Button(Rect position, string text);
GUILayout.Button(string text, params GUILayoutOption[] options);



## 脚本编译顺序 

The phases of compilation are as follows:

* Phase 1: Runtime scripts in folders called Standard Assets, Pro Standard Assets and Plugins. 
（Standard Assets, Pro Standard Assets 以及Plugins中的脚本）

* Phase 2: Editor scripts in folders called Editor that are anywhere inside top-level folders called Standard Assets, Pro Standard Assets and Plugins. 
（顶层目录Standard Assets, Pro Standard Assets和Plugins中的任何地方命名为Editor的脚本）

* Phase 3: All other scripts that are not inside a folder called Editor. 
（所有其他“不在”Editor目录中的脚本）

* Phase 4: All remaining scripts (those that are inside a folder called Editor).
（其他所有剩下的脚本（其他的非Phase2中的Editor目录下的脚本））