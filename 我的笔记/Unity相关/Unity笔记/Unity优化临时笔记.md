# **Unity优化临时笔记**

## 2019.1.3 星期四

1. 合批测试（Batching Test）

    * 静态合批 Static Batching

        - 条件：
          - 相同材质；
          - 不移动，旋转，缩放

        - 优点：Static batching并不减少Draw call的数量，但是由于预先将所有的子模型的顶点变换到了世界空间下，并且这些子模型共享材质，所以多次Draw call调用之前并没有渲染状态的切换，渲染API会缓存绘制指令，起到渲染优化的目的。
  
        - 缺点：打包后包体增大，应用运行时占用的内存体积也会增大。

    * 动态合批 Dynamic Batching

        开启Dynamic batching，Unity会自动将所有共享同一材质的动态GameObject在一个Draw call内绘制。

        - 条件：
          - 共享材质。
          - 模型最高能有900个“顶点属性”，如果Shader中使用了Vertex Position, Normal and single UV,那么能够进行Dynamic batching的模型最多有300个顶点。
          - 镜像变换不能合批（scale呈反数设置）
          - 使用Multi-pass Shader的物体会禁止Dynamic batching。
        - 缺点：每一帧都会有一些CPU性能消耗。

    > 参考资料\
    > [图形渲染及优化—Batch][1]\
    > [图形渲染及优化—Unity合批技术实践][2]
    
    [1]:http://gad.qq.com/article/detail/26927 
    [2]:http://gad.qq.com/article/detail/28456
