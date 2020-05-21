# Unity项目压缩纹理选择

## 资料整理

* Android 
	
ETC1 OpenGL ES2.0支持，移动GPU均支持的一种纹理压缩格式，但不支持Alpha通道，需要将纹理的rgb与alpha通道分离成两张贴图，通过shader进行两张贴图采样。Unity5.4.3版本之后提供了官方支持。

ETC2 只有OpenGL ES3.0支持，且支持alpha通道。如果使用ETC2，那么么旧的OpenGL ES2.0的机型将不被支持，需要考虑实际项目的最低机型选择。

* iOS
  
PVRTC PowerVR架构独有的压缩格式，所有IOS平台都支持。支持2位或4位像素，支持alpha通道和不支持alpha通道选择。长宽相等且为2的N次幂。压缩效果后贴图失真严重。（当前项目需要还是做rgb与alpha的分离）

ASTC 
从IOS9(A8架构)Apple 手机开始支持ASTC压缩格式 ，iphone6/plus以上机型可用。相对于PVRTC2/4而言，ASTC(4X4)的压缩比会增加到0.25，不过显示效果也会好很多，而且不需要把图片设置为方形。


https://blog.uwa4d.com/archives/TechSharing_186.html



## Performance and size of NPOT texture in POT atlas?
https://forum.unity.com/threads/performance-and-size-of-npot-texture-in-pot-atlas.442456/

JC_SummitTech：

GPUs love POT textures, because if a texture is NPOT, it needs to be padded with "garbage" and an implicit offset needs to be used to acces the actual picture data. This is all done automagically by Unity, but is still a hit on performance. There is also the issue of compression, as you have said yourself.

GUP喜欢POT纹理，因为如果一个纹理是NPOT,它需要使用垃圾入扩充，且隐式偏移使得能访问真实的图片数据。这些都是Unity自动完成的，但是它仍然是一个对影响性能开销的。另一个就是如你所说的问题就是压缩。

Another issue is GPU stalling. if you have 500 different sprites, on 500 different textures, the GPU needs to grab one texture, draw it's sprite (or several instances if they can be batched) then grab another texture and so on. The action of grabbing a texture is slow. While you are uploading graphic data to the GPU, it can't really do much, and must wait for the texture to be loaded. By using an Atlas, you are sending (ideally) several sprites at the same time on the GPU. you then use offsets to find the data of each separate sprite, and draw them. So you would only do one (albeit massive) texture upload, and then draw 500 sprites, which is much more performant.

Atlas are not magical though. fi you need 30 pixels out of a 2048x2048 atlas, you actually lose performance. So you generally want to take that into consideration when packing an atlas.

Another advantage of an Atlas is putting a bunch of NPOT images together, into a POT one. That pesky 1025x1025 image would need 1023x1023 px of padding to become POT... but pack it with other smaller images and tadaaa you saved all that space.
I hope that answered your question. In short, POT good, NPOT bad, and Atlas... good or bad depending how well you use them.