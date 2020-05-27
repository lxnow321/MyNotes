# Shader每日记录

## 2020.5.27 星期三

1. rgb * rgb的操作，应该是color的调制(modulate)操作
fixed3 diffuse = _LightColor0.rgb * _Diffuse.rgb * saturate(dot(worldNormal, worldLight));

a.rgb * b.rgb => Color(a.r * b.r, a.g * b.g, a.c * b.c)