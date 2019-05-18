C# 常用代码模块

1.文件读取和写入

using System.IO;

(1)一般文件读取和写入
//获取或创建一个文件流
FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate, [FileAccess,); //默认FileAccess.ReadWrite
或者
FileStream fs = File.Open(filePath, FileMode.OpenOrCreate, [FileAccess,]) //默认FileAccess.ReadWrite
或者
FileStream fs = File.OpenWrite(filePath)


可直接使用
fs.Write(byte[]) / fs.Write(byte) //写入字节流
或创建一个StreamWriter，指定特定的编码向流中写入字符
StreamWriter sw = new StreamWriter(fs); //创建一个写文件流
sw.Write(str, [boolean, encoding]) //具体方法说明查找对应文档

sw.Close();  //关闭写入流，这里貌似不需要再关闭文件流fs.Close()


示例：
FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate);
StreamWriter sw = new StreamWriter(fs);
sw.Write(str);
sw.Close();

File.ReadAllText(filePath, [Encoding,]);
例：
string str = File.ReadAllText(@"F:\test.txt", System.Text.Encoding.Default);



(2)直接写入

File.WriteAllText(filePath, contentStr, [Encoding,]); //在文件中写入字符串，文件不存在则创建，注意：如果中间文件夹路径不存在，抛出异常


