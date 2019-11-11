git笔记

ssh-keygen 生成ssh key

# 基本操作

git init
git clone 克隆仓库，除了可以clone远程仓库，还可以clone本地仓库

克隆本地仓库
1.在D:/test/gittest 目录下，使用git init初始化仓库
2.在D:/test/test 目录下clone上述本地仓库
git clone D:/test/gittest D:/test/test 注意如果是\则需要使用\\

git config --global use.name 'name'
git config --global user.email test@mail.com


git checkout -- <file> 还原未缓存的修改，
git reset HEAD <file> 取消已缓存的内容，可指定文件。或全部还原已缓存的内容到非缓存
git reset --soft HEAD^ 取消已缓存的内容，还原到上个版本，保留缓存的修改

git diff <file> 查看未缓存的改动
git diff --cached/--staged  查看已暂存的改动
git diff HEAD 查看已缓存区，未缓存的所有改动
git diff --stat 显示摘要而非整个 diff

git pull（拉取） / git fetch（获取）的区别？
git  pull     从远程拉取最新版本 到本地  自动合并 merge            git pull origin master
git  fetch   从远程获取最新版本 到本地   不会自动合并 merge    git fetch  origin master       git log  -p master ../origin/master     git merge orgin/master

实际使用中  使用git fetch 更安全    在merge之前可以看清楚 更新情况  再决定是否合并


git commit -am 可绕过add过程，一步到位将修改内容commit


删除文件

git rm <file>  删除即放到了暂存区

git rm -f <file> 删除前已经修改过，不管是否放到了暂存区，需要强制删除都需要使用-f参数

git rm --cached <file> 删除后还未放到暂存区，需要继续add和commit操作


git mv   可修改文件名，或者移动文件位置。注意移动文件位置需要保证路径文件夹存在


=======分支管理

git branch (branchname)  创建一个分支

git checkout (branchname)  切换到某个分支

git checkout -b (branchname) 创建并切换到某个分支

git branch -d (branchname) 删除一个分支

疑问：是否能创建一个远程仓库中已存在的分支，如果删除这个自己创建的同名分支，是否会影响到远程仓库中的同名分支？

git merge (branchname) 合并某个分支内容到当前分支



git merge --abort 中止合并


git log 查看提交历史
git log --oneline 选项来查看历史记录的简洁的版本

git log --author=liuxi --oneline -5  使用--author指定提交者，包含字符串即可， --oneline查看简介版本，-5查看多少行

--since 和 --before，也可以用 --until 和 --after。 指定日期


标签：

git tag 查看所有tag

git tag -a v1.0  给HEAD上打上v1.0的标签，输入完指令后会进入编辑模式输入一些tag相关的信息
git tag -a <tagname> -m "runoob.com标签" 就不需要像上述那样进入编辑模式编辑tag相关的信息

git log --oneline --decorate --graph   加了--decorate才能看到标签

git tag -a v0.9 85fc7e7 指定在85fc7e7（可通过git log --oneline查看）上追加v0.9的tag。 



其他：
1.git中文显示乱码
gitbash窗口中右击选择Options->Text
local：zh_CN  Character set: UTF-8



# Git 其他使用

## 2019.1.4 星期五
### 问题：想要将已存在的目录转换为一个Git项目托管到GitHub上

* 在本地目录执行  
    > git init
* 将所有文件加入到本地git仓库
    > git add .
* 提交到仓库
    > git commit -m "Init"
* 访问GtiHub，创建一个新的仓库，并获取该git仓库的url地址
    > https://github.com/xxx.git
* 本地gitbash终端执行指令
    > git remote add origin https://github.com/xxx.git
* 检查是否成功
    > git remote -v
* 推送本地到远端GitHub仓库
    > git push origin master
    
    `注意：如果推送失败，并提示refusing to merge unrelated histories错误，那么应该是GitHub上创建时勾选了Initialize this repository with a README初始化远程仓库。那么本地跟远程仓库就是两个不同的git库，无法直接合并。需要先从远端进行pull更新合并`
    >git push origin master --allow-unrelated-histories
    
    `使用如上指令先pull，不加--allow-unrelated-histories会导致pull失败。执行完pull后，再执行git push origin master即可`


***