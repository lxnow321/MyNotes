# Shader每日记录

## 2020.5.27 星期三

1. rgb * rgb的操作，应该是color的调制(modulate)操作
fixed3 diffuse = _LightColor0.rgb * _Diffuse.rgb * saturate(dot(worldNormal, worldLight));

a.rgb * b.rgb => Color(a.r * b.r, a.g * b.g, a.c * b.c)


## 2020.6.4 星期四

1.关于法线变换公式的的推导中的明白的地方

$T_A\cdot N_A=0  \Rightarrow$ $T_A^T N_A=0$ 

Ta切向量，Na法向量，垂直点乘为0没问题。Ta的转置与Na的乘法看成列向量与横向量的乘法他的运算结果与Ta*Na点乘算法一致
$T_A\cdot N_A=T_A^T N_A=T_Ax*N_Ax+T_Ay*N_Ay+T_Az*N_Az=0$