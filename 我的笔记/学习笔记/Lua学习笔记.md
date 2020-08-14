Lua学习笔记

第2章：

Lua是动态类型语言，变量不要类型定义。Lua中有8个基本类型分别为：nil、boolean、number、string、userdata、function、thread和table。

一个全局变量没有被赋值以前默认值为nil；给全局变量负nil可以删除该变量。

在控制结构的条件中除了false和nil为假，其他值都为真。所以Lua认为0和空串都是真。




疑问：
1.pairs 和 ipairs的区别？
初步理解： ipairs只能遍历索引以1,2,3,4正整数连续key和value，如果key不是正整数，且序列被中断，则中断索引

pairs能返回所有的key和value键值对


## **2018.12.5  星期三**

第13章 Metatables and Metamethods

setmetatable/getmetatable 设置和获取metatable

元表：__add, __sub, __mul ... _call等


第16章 面向对象程序设计

self的隐藏
使用":"定义函数或者调用函数时，不用显示传入self;
使用"."定义或调用函数函数，需要显示传入self。
function Account:func1(v)
	self.a = self.a + v
end

function Account.func2(self, v)
	self.a = self.a - v
end

Account:func1(10)
Account.func1(Account, 20)

Account:func2(10)
Account.func2(Account, 20)




## **2018.12.29  星期六**

1.require[modname]
(1)加载指定的module，先检查package.loaded，看是否已经加载，已经加载，直接返回package.loaded[modname]。没有加载过，则继续；
(2)查找一个加载器(loader)，以package.searchers的顺序
	package.searchers包含四个方法
	2.1 查找package.preload
	2.2 使用package.searchpath方法，查找存储在package.path中的路径
	package.path = "E:\Tools\lua\?.lua;E:\Tools\lua\?\init.lua;E:\Tools\?.lua;E:\Tools\?\init.lua;.\?.lua"
	2.3 使用package.searchpath方法，查找存储在package.cpath中路径
	package.cpath =  "./?.so;./?.dll;/usr/local/?/init.so"
	2.4 查找一体化加载器。在C路径搜索一个库，查找给定模块的根名称。如require(a.b.c)，将会查找C库中的a.，如果找到，则会在其中查找子模块。用上述举例，子模块将是luaopen_a_b_c。
	除了第一个preload，所有的搜索器都会返回模块所在的文件名作为额外值。
(3)查找到了加载器，加载器将会被调用，并传入两个参数,modname和一个额外参数（如果是通过preload查找的加载器，没有该额外参数）。


2.luajit与lua
由于tolua当前使用的luajit2.1版本，而luajit2.1兼容的lua版本是5.1，所以学习时注意使用lua5.1的文档查阅。

3.由于lua版本的不同，可能有些函数方法可能会不一样。要注意此处的差别。


## 2020.8.14 星期五
### userdata
1. full userdata :看做是一个C的自定义类型交互数据
2. light userdata：看做一个C的指针类型交互数据