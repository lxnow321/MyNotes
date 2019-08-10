# SQL语句学习

## 菜鸟教程：http://www.runoob.com/sql/sql-tutorial.html

**SQL，指结构化查询语言，全称是 Structured Query Language。**

***

## 1. 查询语句
   
   SELECT column_name,column_name   //表字段
   FROM table_name                  //数据表
   WHERE column_name operator value //条件

   select distinct ... //去重查询

* SELECT * FROM table; 查询table中的所有
* SELECT * FROM userInfo WHERE userId = 1001; 查询table表中userId为1001的数据
  
| :运算符 | 释义                                 | 示例                                                       |
| :------ | :----------------------------------- | ---------------------------------------------------------- |
| =       | 等于                                 |
| <>      | 不等于                               |
| >       | 大于                                 |
| <       | 小于                                 |
| >=      | 大于等于                             |
| <=      | 小于等于                             |
| BETWEEN | 在某个范围                           | where sal between 1500 and 3000                            |
| LIKE    | 搜索某种模式                         | where sal in (5000,3000,1500)                              |
| IN      | 指定针对某个列的多种可能值，模糊查询 | where ename like 'M%' ，%表示多个字符，_下划线表示一个字符 |

## 2. 常用SQL函数

* MAX() 返回指定列的最大值
  
  SELECT MAX(column_name) FROM table_name;

* MIN() 返回指定列的最小值

  SELECT MIN(column_name) FROM table_name;

* AVG() 返回数值列的平均值

  SELECT AVG(column_name) FROM table_name;