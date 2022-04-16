using System;

namespace RemoteFileExplorer.Editor
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class CustomMenuAttribute : System.Attribute
    {
        public string title;
        public int priority;

        public CustomMenuAttribute(string title) : this(title, 0) { }

        public CustomMenuAttribute(string title, int priority)
        {
            this.title = title;
            this.priority = priority;
        }
    }
}

/*  CustomMenuAttribute使用样例
using RemoteFileExplorer.Editor;

public class TestCustomMenu
{
    [CustomMenu("go to path")]
    public static void TestGoTo(ManipulatorWrapper manipulator)
    {
        manipulator.GoTo("C:/Test");
    }

    [CustomMenu("download path to dest")]
    public static void TestDownload(ManipulatorWrapper manipulator)

    {
        manipulator.Download("C:/Test/a.txt", "C:/Test2/a.txt");  // 下载Test/a.txt到Test2/a.txt
        manipulator.Download("C:/Test", "C:/Test2");  // 下载Test的内容下载Test2目录下
    }

    [CustomMenu("upload path to dest")]
    public static void TestUpload(ManipulatorWrapper manipulator)
    {
        manipulator.Upload("C:/Test/a.txt", "C:/Test2");  // 上传a.txt到Test2目录下
        manipulator.Upload("C:/Test", "C:/Test2");  // 上传Test目录到Test2目录下
    }

    [CustomMenu("delete path")]
    public static void TestDelete(ManipulatorWrapper manipulator)
    {
        manipulator.Delete("C:/Test/a.txt");  // 删除a.txt
        manipulator.Delete("C:/Test");  // 删除Test目录
    }

    [CustomMenu("rename path to dest")]
    public static void TestRename(ManipulatorWrapper manipulator)
    {
        manipulator.Rename("C:/Test", "C:/Test2");  // 将Test重命名为Test2
        manipulator.Rename("C:/Test/a.txt", "C:/Test/b.txt");  // 将a.txt重命名为b.txt
    }

    [CustomMenu("new folder")]
    public static void TestNewFolder(ManipulatorWrapper manipulator)
    {
        manipulator.NewFolder("C:/Test");  // 新建Test文件夹
    }

    [CustomMenu("refresh")]
    public static void TestRefresh(ManipulatorWrapper manipulator)
    {
        manipulator.Refresh();
    }
}
*/