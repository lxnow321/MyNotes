1.LuaJit中Jit模式是否在工程手动开启和关闭？
LuaJit源码中lj_arch.h中有如下定义:

	#if LJ_TARGET_IOS || LJ_TARGET_CONSOLE
	/* Runtime code generation is restricted on iOS. Complain to Apple, not me. */
	/* Ditto for the consoles. Complain to Sony or MS, not me. */
	#ifndef LUAJIT_ENABLE_JIT
	#define LJ_OS_NOJIT		1
	#endif
	#endif

即：当处于IOS平台或Console控制台下，如果未定义LUAJIT_ENABLE_JIT则会定义一个宏LJ_OS_NOJIT，用于标记当前平台不开启jit。


2.Lua为什么可以再ios下进行热更新

https://www.jianshu.com/p/71fffc9a9cac

* IOS无法进行热更新的原因

  一般都说ios禁用了jit。而本质上是ios禁用了应用在ios系统上进行开辟可执行的内存，从而也就禁用了jit。

* C#编译型语言为什么不能热更新，而Lua，Python等解释型语言却可以？

  - 当前所讨论的热更新主要是狭义的热更新，即在ios平台上进行热更新。如果抛开ios，C#同样也可以通过dll反射进行热更新。
  - C#并不能说是一种编译型语言，C#代码会被编译成IL代码，IL再解释成机器码进行执行。Lua，Python也并不是说因为是解释型语言就可以进行热更新。Lua同样是被编译成opcode（lua操作码），不过解释OpCode时不是直接翻译成机器码执行，而是通过Lua虚拟机（C语言编写的，解释OpCode）进行解释执行。
  - JIT对IL代码的解释执行原理是：将IL代码解释成所在平台的机器码，开辟一段内存空间，要求这段内存空间可读，可写，可执行。然后将解释出来的机器码放入，修改CPU中的指令指针寄存器中的地址，让CPU执行之前解释出来的机器码。

  以上即可理解C#因为需要开辟可执行的内存空间而被ios禁止，而Lua是通过自己的虚拟机解释自己的中间代码OpCode进行执行，并没有开辟可执行的内存空间。所以Lua可以在ios上进行热更新，而C#不可以。


3. 以前项目使用C#+JSBinding框架做热更新
   
   JSBinding框架是使用SharpKit将C#代码转换成JavaScript代码进行热更。其框架中还集成了mozjs（对SpiderMoneky引擎的封装）。SpiderMoneky采用解释和JIT混合运行，所以该套框架JavaScript在ios上的热更同样是寄语SpiderMoneky的解释运行。
   注意：V8引擎抛弃了解释器，完全使用了JIT

	引擎浅谈 SpiderMonkey & Google V8
	https://www.wangshaoxing.com/blog/javascript-engines.html