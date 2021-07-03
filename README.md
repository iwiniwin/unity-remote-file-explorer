# Remote File Explorer

[![license](http://img.shields.io/badge/license-MIT-blue.svg)](https://github.com/iwiniwin/unity-remote-file-explorer/blob/main/LICENSE.md)
[![release](https://img.shields.io/badge/release-v0.0.1-blue.svg)](https://github.com/iwiniwin/unity-remote-file-explorer/releases)
[![PRs Welcome](https://img.shields.io/badge/PRs-welcome-blue.svg)](https://github.com/iwiniwin/unity-remote-file-explorer/pulls)

[Remote File Explorer](https://github.com/iwiniwin/unity-remote-file-explorer)是一个远程文件浏览器，使用户通过Unity Editor就能操作应用所运行平台上的目录文件。比如浏览，下载，上传，删除等操作。通过本工具可以极大提高开发调试效率。例如：
* 应用在设备上运行时，出现异常情况，推测可能是资源丢失导致的。此情况下可以通过Remote File Explorer查看关键资源是否存在
* 应用在设备上运行出现与本地环境不同的现象时，可通过Remote File Explorer下载关键代码，对比与本地的不同
* 使用XLua或SLua开发时，排除bug，修改代码后可以通过Remote File Explorer快速上传修改后的代码，而避免重复的打包


## 安装
本程序是Unity Package形式
可通过Unity Package + 输入本程序git地址完成安装
或直接克隆本程序到项目Pacages目录下
更多安装包的方式可查看[Unity官方文档](https://docs.unity3d.com/cn/2019.4/Manual/upm-ui-actions.html)

## 使用

给任意游戏对象挂载FileExplorerClient组件
如果固定已知IP，可通过Inspector面板输入IP和端口号，然后勾选connectAutomatically，则会在应用启动时自动连接
否则，可以在调试模式下，点击一个按钮，然后打开输入框，输入通过Remote File Explorer查看到的IP和端口进行连接 然后调用FileExplorerClient组件的StartConnect接口连接
当连接成功后，Remote File Explorer的title icon会变回绿色，status connect statue也会显示绿色Established

## 功能
* 通过GOTO直接打开，Unity预定义的一些路径
* 通过输入框直接输入路径跳转
* 右键选中下载
* 右键选中删除
* 右键空白区域上传
* 右键空白区域刷新
* 直接从Unity Project窗口或系统文件浏览器拖拽到窗口进行上传
* 通过status面板可以查看连接状态以及连接设备的信息，以此判断连接的是哪台设备

### TODO
* 名称显示过长换成省略号优化
* 文件网格cell复用
* 窗口大小改变自动刷新cell布局
* 连接后自动GO TO上次路径
* 处理.jar只读问题
* 下载成功提示有打开所在文件夹
* 当前任务显示
* 命令支持超时
* 右键选中Rename
* 支持新建文件夹
* 上传下载时Icon变化以显示状态

### Test
* 测试打开RemoteFileExplorer后，多次Play Game后的连接情况
* 测试多客户端请求连接
* Android平台测试
* IOS平台测试
* Mac平台测试
* 不同平台分隔符问题