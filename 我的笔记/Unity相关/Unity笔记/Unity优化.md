# **Unity优化知识**

## 1. foreach
foreach会导致额外的内存开销问题，Unity2017已经解决了可以不用考虑。但Unity5.x的一些版本需要注意，自行测试。

## 2. Tag访问
go.Tag的访问会造成额外的内存开销，Unity2017测试会产生"34B"的样子。缓存或使用go.CompareTag("tagName")代替，注意CompareTag中的参数必须是存在的tag名称，否则报错。