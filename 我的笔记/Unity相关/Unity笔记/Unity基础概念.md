# Unity基础概念

## Asset:
* 1.“An Asset is a representation of any item you can use in your game or Project”.
“资产”是任何可用于Unity工程中的物件表示，这些资产可能来自Unity以外的第三方工具制作而成，如3D Model，音频，图片等任何Unity支持的文件类型。

## GameObject:
* 1.“Every object in your game is GameObject”.
* 2.“GameObject can’t do anything on its own, need to give it properties before it can become other like character, environment and so on”.
* 3.A GameObject always has a Transform component attached (to represent position and orientation) and it is not possible to remove this.
任何在游戏中的物体都是一个游戏对象。游戏对象自身无法做任何事情，需要为其提供属性才可使之成为其他如角色，环境物体等游戏对象。一个游戏对象一定会附加一个Trasnform组件，且这个组件无法被移除。

## Component:
* 1.“A GameObject contains components”.
一个游戏对象可以包含多个组件，每个组件可为该游戏对象提供各自的特性。例如：Main Camera GameObject。其默认包含Transform，Camera，Audio Listener等组件。Transform是每个游戏对象都会附加的，用于表示位置和方向的组件；Camera提供摄像机所需要的相关属性的组件；Audio Listener则提供音频接收相关的特性。

## Prefab:
1.“The Prefab Asset acts as a template from which you can create new Prefab instances in Scene”.
Prefab资产扮演一个可以在场景中创建新的Prefab实例的模板。