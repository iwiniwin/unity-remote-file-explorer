using System.IO;

namespace RemoteFileExplorer
{
    public class FileUtil
    {
        public const string Separator = "/";

        /// <summary>
        /// 获取所有子目录，包括自身
        /// </summary>
        public static string[] GetAllDirectories(string path)
        {
            var subDirectories = Directory.GetDirectories(path, "*", SearchOption.AllDirectories);
            string[] directories = new string[subDirectories.Length + 1];
            directories[0] = path;
            for (int i = 1; i < directories.Length; i++)
            {
                directories[i] = subDirectories[i - 1];
            }
            return directories;
        }   

        /// <summary>
        /// 获取所有子文件
        /// </summary>
        public static string[] GetAllFiles(string path)
        {
            return Directory.GetFiles(path, "*", SearchOption.AllDirectories);
        }

        public static string FixedPath(string path)
        {
            return path.Replace("\\", Separator);
        }

        public static string CombinePath(string path1, string path2)
        {
            return path1 + Separator + path2;
        }
    }
}