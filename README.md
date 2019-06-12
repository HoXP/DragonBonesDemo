# DragonBonesDemo
Unity下龙骨动画解决方案
### 一、从龙骨软件里导出龙骨数据
##### 1、【文件】【导出】，弹出导出数据窗口；
##### 2、设置导出数据类型，选择【JSON】格式，其他选项保持默认。
![1](https://github.com/HoXP/DragonBonesDemo/blob/master/ReadmePics/1.png?raw=true)
### 二、将龙骨数据导入Unity
##### 1、将导出的3个文件【xxx_ske.json】【xxx_tex.json】【xxx_tex.png】一起拖入Unity某文件夹内。
![2](https://github.com/HoXP/DragonBonesDemo/blob/master/ReadmePics/2.png?raw=true)
![3](https://github.com/HoXP/DragonBonesDemo/blob/master/ReadmePics/3.png?raw=true)
##### 2、右键【xxx_ske】文件，【Create】【DragonBones】【Create Unity Data】，生成【xxx_Data.asset】资源文件；
![4](https://github.com/HoXP/DragonBonesDemo/blob/master/ReadmePics/4.gif?raw=true)
##### 3、在Hierarchy面板创建一个空GameObject，并挂载【UnityArmatureComponent.cs】脚本，将【xxx_Data.asset】拖到【UnityArmatureComponent.cs】脚本的【DragonBones Data】处，点击【Create】，选择【Animation】的动画选项可查看龙骨动画效果。如下图：
![4](https://github.com/HoXP/DragonBonesDemo/blob/master/ReadmePics/5.gif?raw=true)
##### 4、至此，就完成了龙骨数据的导入，改下名字存成预制体即可。
