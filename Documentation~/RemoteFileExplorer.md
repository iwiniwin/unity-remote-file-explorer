## 使用
安装本工具后，给任意游戏对象添加`FileExplorerClient`组件

![](Images/file_explorer_client.png)
* 如果是固定连接到某台机器上，可直接通过Inspector面板在`Host`域输入这台机器的IP地址，然后勾选`Connect Automatically`，则会在应用启动时自动连接
* 如果希望应用启动后能主动选择连接到哪台机器，则可在Debug模式下封装一套简单的UI，使开发人员能够输入想要连接到的IP地址。例如在自己的菜单中添加一个条目或按钮，点击后弹出输入窗口。在成功获取到的IP地址后，将其赋值给`FileExplorerClient`组件的`Host`属性，然后调用`FileExplorerClient`组件的`StartConnect`方法开启连接

通过本工具的`Status`面板可以查看到当前机器的IP地址

![](Images/status_panel_small.png)

当连接成功后，本工具标题栏的Icon会变成绿色，以表示连接状态是已连接

![](Images/status_panel_small2.png)

## 功能介绍
* 通过状态栏的`GOTO`可直接跳转到Unity预定义的一些路径

![](Images/go_to_path_key.png)

* 通过单击路径栏可打开输入框，以直接输入路径跳转或复制当前路径

![](Images/input_path.png)

* 右键所选中的文件夹或文件支持下载

![](Images/right_click_download.png)

* 右键所选中的文件夹或文件支持删除

![](Images/right_click_delete.png)

* 右键空白区域支持上传文件夹或文件

![](Images/right_click_upload.png)

* 右键空白区域支持刷新当前路径的内容

![](Images/right_click_refresh.png)

* 支持直接从`Unity Project`窗口拖拽文件夹或文件到本工具上传

![](Images/drag_from_project_upload.png)

* 支持直接从系统文件浏览器拖拽文件夹或文件到本工具上传

![](Images/drag_from_explorer_upload.png)

* 通过本工具`status`面板可查看连接状态以及已连接设备的信息，可用于辨别连接的是哪台设备

![](Images/status_panel.png)

