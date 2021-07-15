using System;
using System.Text;
using System.IO;
using ICSharpCode.SharpZipLib.Checksum;
using ICSharpCode.SharpZipLib.Zip;

namespace RemoteFileExplorer.Editor
{
    public class ZipUtility
    {
        private static FastZip zipInstance;

        private static FastZip ZipInstance
        {
            get 
            {
                if(zipInstance == null)
                {
                    zipInstance = new FastZip();
                }
                return zipInstance;
            }
        }

        public static bool Compress(string src, string zipFilePath)
        {
            if(File.Exists(zipFilePath))
            {
                File.Delete(zipFilePath);
            }
            ZipInstance.CreateZip(zipFilePath, src, true, "");
            return File.Exists(zipFilePath);
        }

        public static bool Decompress(string zipFilePath, string dest)
        {
            if(Directory.Exists(dest))
            {
                Directory.Delete(dest, true);
            }
            ZipInstance.ExtractZip(zipFilePath, dest, "");
            return Directory.Exists(dest);
            // if (!File.Exists(zipFilePath)) return false;
            // if (!Directory.Exists(dest))
            // {
            //     Directory.CreateDirectory(dest);
            // }
            // using (ZipFile file = new ZipFile(zipFilePath))
            // {
            //     foreach (ZipEntry entry in file)
            //     {
            //         if (entry == null || string.IsNullOrEmpty(entry.Name))
            //         {
            //             continue;
            //         }
            //         string path = Path.Combine(dest, entry.Name);
            //         if (entry.IsDirectory)
            //         {
            //             Directory.CreateDirectory(path);
            //             continue;
            //         }
            //         try
            //         {
            //             string parent = Path.GetDirectoryName(path);
            //             if (!Directory.Exists(parent))
            //             {
            //                 Directory.CreateDirectory(parent);
            //             }
            //             using (BinaryReader br = new BinaryReader(file.GetInputStream(entry)))
            //             {
            //                 var bytes = br.ReadBytes((int)entry.Size);
            //                 File.WriteAllBytes(path, bytes);
            //             }
            //         }
            //         catch (Exception e)
            //         {
            //             Log.Error(e.ToString());
            //             return false;
            //         }
            //     }
            // }
            // return true;
        }
    }
}