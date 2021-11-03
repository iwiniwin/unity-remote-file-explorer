using System.Collections;
using System.Collections.Generic;
using RemoteFileExplorer.Editor.UI;
using UnityEngine;
using UnityEditor;
using System.IO;
using System;

namespace RemoteFileExplorer.Editor
{
    public class Manipulator
    {
        public string m_CurPath;

        private const string CacheKey = "RemoteFileExplorer_Cache_GoTo";
        private const string JarTag = "jar:file://";
        private const string ApkTag = "base.apk!";
        private List<string> m_GoToHistory = new List<string>();
        private int m_GoToHistoryIndex = -1;
        private Coroutine m_GoToCoroutine;

        public string curPath
        {
            get
            {
                return m_CurPath;
            }
            set
            {
                m_CurPath = FileUtil.FixedPath(value);
            }
        }

        private RemoteFileExplorerWindow m_Owner;
        public Manipulator(RemoteFileExplorerWindow owner)
        {
            m_Owner = owner;
        }

        public void UpdateStatusInfo(ConnectStatus status)
        {
            Coroutines.Start(Internal_UpdateStatusInfo(status));
        }

        public void Refresh()
        {
            if(string.IsNullOrEmpty(curPath)) return;
            GoTo(curPath, false, false, false);
        }

        public void GoTo(ObjectItem item)
        {
            var data = item.Data;
            if (data.type == ObjectType.File)
                return;
            GoTo(data.path);
        }

        public void GoTo(string path)
        {
            GoTo(path, false, true, false);
        }

        public void GoToByKey(string key)
        {
            GoTo(key, true, true, false);
        }

        public void GoTo(string path, bool isKey, bool record, bool silent)
        {
            if(m_GoToCoroutine != null)
            {
                Coroutines.Stop(m_GoToCoroutine);
            }
            m_GoToCoroutine = Coroutines.Start(Internal_GoTo(path, isKey, record, silent));
        }

        public void RecordGoTo(string path)
        {
            if(m_GoToHistoryIndex >= 0 && m_GoToHistory.Count > m_GoToHistoryIndex)
            {
                if(m_GoToHistory[m_GoToHistoryIndex].Equals(path)) 
                {
                    return;
                }
            }
            if(m_GoToHistoryIndex >= 0 && m_GoToHistoryIndex < m_GoToHistory.Count - 1)
            {
                m_GoToHistory.RemoveRange(m_GoToHistoryIndex + 1, m_GoToHistory.Count - m_GoToHistoryIndex - 1);
            }
            m_GoToHistory.Add(path);
            m_GoToHistoryIndex = m_GoToHistory.Count - 1;
            if(m_GoToHistoryIndex > 0)
            {
                m_Owner.m_PrevButton.SetEnabled(true);
            }
            m_Owner.m_NextButton.SetEnabled(false);
        }

        public void BackwardGoTo()
        {
            if(m_GoToHistoryIndex > 0)
            {
                GoTo(m_GoToHistory[-- m_GoToHistoryIndex], false, false, false);
                m_Owner.m_NextButton.SetEnabled(true);
            }
            if(m_GoToHistoryIndex <= 0)
            {
                m_Owner.m_PrevButton.SetEnabled(false);
            }
        }

        public void ForwardGoTo()
        {
            if(m_GoToHistoryIndex < m_GoToHistory.Count - 1)
            {
                GoTo(m_GoToHistory[++ m_GoToHistoryIndex], false, false, false);
                m_Owner.m_PrevButton.SetEnabled(true);
            }
            if(m_GoToHistoryIndex >= m_GoToHistory.Count - 1)
            {
                m_Owner.m_NextButton.SetEnabled(false);
            }
        }

        /// <summary>
        /// 记录上次的GOTO，下次启动后自动跳转
        /// </summary>
        public void SaveLastGoTo()
        {
            if(m_GoToHistoryIndex >= 0 && m_GoToHistory.Count > m_GoToHistoryIndex)
            {
                EditorUserSettings.SetConfigValue(CacheKey, m_GoToHistory[m_GoToHistoryIndex]);
            }
        }

        public string ReadLastGoTo()
        {
            if(m_GoToHistoryIndex >= 0 && m_GoToHistory.Count > m_GoToHistoryIndex)  // 窗口未关闭时再次开启连接，直接使用内存记录
            {
                return m_GoToHistory[m_GoToHistoryIndex];
            }
            return EditorUserSettings.GetConfigValue(CacheKey);
        }

        public void Select(ObjectItem item)
        {
            var data = item.Data;
            curPath = data.path;
            m_Owner.m_ObjectListArea.SetSelectData(data);
        }

        /// <summary>
        /// 选择空
        /// </summary>
        public void Select()
        {
            var data = m_Owner.m_ObjectListArea.GetSelectData();
            if (data != null)
            {
                if(data.state == ObjectState.Editing)
                {
                    return;  // 处于编辑模式，不默认选择空
                }
                curPath = Path.GetDirectoryName(data.path);
            }
            m_Owner.m_ObjectListArea.SetSelectData(null);
        }

        public void Download(ObjectItem item)
        {
            var data = item.Data;
            string path = data.path;
            string dest = null;
            if(data.type == ObjectType.File)
            {
                string extension = Path.GetExtension(path);
                if(extension.StartsWith("."))
                {
                    extension = extension.Substring(1, extension.Length - 1);
                }
                dest = EditorUtility.SaveFilePanel(Constants.SelectFileTitle, "", Path.GetFileNameWithoutExtension(path), extension);
                if(string.IsNullOrEmpty(dest)) return;
            }
            else
            {
                dest = EditorUtility.SaveFolderPanel(Constants.SelectFileTitle, "", "");
                if(string.IsNullOrEmpty(dest)) return;
                dest = FileUtil.CombinePath(dest, Path.GetFileName(path));
            }
            Coroutines.Start(Internal_Download(path, dest, false));
        }

        public void Delete(ObjectItem item)
        {
            Coroutines.Start(Internal_Delete(item.Data.path));
        }

        public void StartRename(ObjectItem item)
        {
            if(item.Data.state != ObjectState.Editing)
            {
                item.Data.state = ObjectState.Editing;
                item.SwitchToEdit(true);
            }
        }

        public void EndRename(ObjectItem item, string value)
        {
            if(item.Data.state != ObjectState.Editing)
            {
                return;
            }
            item.Data.state = ObjectState.Selected;
            item.SwitchToEdit(false);
            if(string.IsNullOrEmpty(value))
            {
                return;
            }
            var directory = Path.GetDirectoryName(item.Data.path);
            var dest = FileUtil.CombinePath(directory, value);
            if(dest.Equals(value))
            {
                return;
            }
            Coroutines.Start(Internal_Rename(item.Data.path, dest));
        }

        private static string DefaultNewFolderName = "NewFolder";
        private Comparison<ObjectData> compareFunc = (x, y) => {
            if((x.type <= ObjectType.TempFile) != (y.type <= ObjectType.TempFile))
            {
                return x.type <= ObjectType.TempFile ? 1 : -1;
            }
            return x.path.CompareTo(y.path);
        };
        public void StartNewFolder()
        {
            if(string.IsNullOrEmpty(curPath)) return;
            if (!CheckConnectStatus()) return;
            var list = m_Owner.m_ObjectListArea.GetAllData();
            var data = new ObjectData(ObjectType.TempFolder, FileUtil.CombinePath(curPath, DefaultNewFolderName), ObjectState.Editing);
            list.Add(data);
            list.Sort(compareFunc);
            m_Owner.m_ObjectListArea.UpdateView(list);
        }

        public void EndNewFolder(ObjectItem item, string value)
        {
            if(item.Data.state != ObjectState.Editing)
            {
                return;
            }
            if(string.IsNullOrEmpty(value))
            {
                value = DefaultNewFolderName;
            }
            string path = FileUtil.CombinePath(curPath, value);
            var list = m_Owner.m_ObjectListArea.GetAllData();
            list.Remove(item.Data);  // 移除临时文件夹视图
            m_Owner.m_ObjectListArea.UpdateView(list);
            Coroutines.Start(Internal_NewFolder(path));
        }

        public void UploadFile()
        {
            string path = EditorUtility.OpenFilePanel(Constants.SelectFileTitle, "", "");
            if (!string.IsNullOrEmpty(path))
            {
                Upload(new string[] { path });
            }
        }

        public void UploadFolder()
        {
            string path = EditorUtility.OpenFolderPanel(Constants.SelectFolderTitle, "", "");
            if (!string.IsNullOrEmpty(path))
            {
                Upload(new string[] { path });
            }
        }

        public void Upload(string[] paths)
        {
            string dest = curPath;
            var data = m_Owner.m_ObjectListArea.GetSelectData();
            if (data != null)
            {
                dest = Path.GetDirectoryName(data.path);
            }
            if (string.IsNullOrEmpty(dest))
            {
                EditorUtility.DisplayDialog(Constants.WindowTitle, Constants.NoDestPathTip, Constants.OkText);
                return;
            }
            foreach (string path in paths)
            {
                if (!File.Exists(path) && !Directory.Exists(path))
                {
                    EditorUtility.DisplayDialog(Constants.WindowTitle, string.Format(Constants.PathNotExistTip, path), Constants.OkText);
                    return;
                }
            }
            Coroutines.Start(Internal_Upload(paths, dest));
        }

        /// <summary>
        /// 跳转到指定路径
        /// </summary>
        private IEnumerator Internal_GoTo(string path, bool isKey, bool record, bool silent)
        {
            if (!CheckConnectStatus(!silent)) yield break;
            Command req;
            if (isKey)
            {
                req = new QueryPathKeyInfo.Req
                {
                    PathKey = path,
                };
            }
            else
            {
                req = new QueryPathInfo.Req
                {
                    Path = path,
                };
            }
            CommandHandle handle = m_Owner.m_Server.Send(req);
            yield return handle;
            if (!CheckHandleError(handle, "", !silent) || !CheckCommandError(handle.Command, "", !silent))
            {
                yield break;
            }
            var rsp = handle.Command as QueryPathInfo.Rsp;
            if (!rsp.Exists)
            {
                string realPath = path;
                if(rsp is QueryPathKeyInfo.Rsp)
                {
                    realPath = (rsp as QueryPathKeyInfo.Rsp).Path;
                }
                if(realPath.StartsWith(JarTag) && realPath.Contains(ApkTag))
                {
                    yield return Internal_GoToApkFile(path, realPath, isKey, record, silent);
                }
                else
                {
                    Internal_OnPathNotExist(path, realPath, isKey, silent);
                }
                yield break;
            }
            List<ObjectData> list = new List<ObjectData>();

            foreach (var item1 in rsp.Directories)
            {
                list.Add(new ObjectData(ObjectType.Folder, item1));
            }
            foreach (var item1 in rsp.Files)
            {
                list.Add(new ObjectData(ObjectType.File, item1));
            }
            m_Owner.m_ObjectListArea.UpdateView(list);
            if (rsp is QueryPathKeyInfo.Rsp)
            {
                curPath = (rsp as QueryPathKeyInfo.Rsp).Path;
            }
            else
            {
                curPath = path;
            }
            if(record)
            {
                RecordGoTo(curPath);
            }
        }

        /// <summary>
        /// 专用于处理Android平台 Application.streamingAssetsPath 的读写问题
        /// jar:file:///data/app/package_name/base.apk!/assets
        /// </summary>
        private IEnumerator Internal_GoToApkFile(string path, string realPath, bool isKey, bool record, bool silent)
        {
            // TODO
            // string filePath = realPath.Substring(JarTag.Length, realPath.IndexOf(ApkTag) - JarTag.Length + ApkTag.Length - 1);
            // 计算MD5 判断缓存是否有，有则使用缓存，无则下载
            // yield return Internal_Download(filePath, dest, true);
            Internal_OnPathNotExist(path, realPath, isKey, silent);
            yield break;
        }

        private void Internal_OnPathNotExist(string path, string realPath, bool isKey, bool silent)
        {
            string msg = string.Format(Constants.PathNotExistTip, path);
            if(isKey)
            {
                msg = string.Format(Constants.PathKeyNotExistTip, path, realPath);
            }
            Log.Debug(msg);
            if(!silent)
            {
                EditorUtility.DisplayDialog(Constants.WindowTitle, msg, Constants.OkText);
            }
        }

        /// <summary>
        /// 下载
        /// </summary>
        private IEnumerator Internal_Download(string path, string dest, bool silent)
        {
            if (!CheckConnectStatus(!silent)) yield break;
            var req = new Pull.Req
            {
                Path = path,
            };
            CommandHandle handle = m_Owner.m_Server.Send(req);
            yield return handle;
            string downloadFailedTip = string.Format(Constants.DownloadFailedTip, path);
            while (CheckHandleError(handle, downloadFailedTip, !silent) && CheckCommandError(handle.Command, downloadFailedTip, !silent))
            {
                if (handle.Command is CreateDirectory.Req)
                {
                    
                    var createDirectoryReq = handle.Command as CreateDirectory.Req;
                    CreateDirectory.Rsp rsp = new CreateDirectory.Rsp()
                    {
                        Ack = createDirectoryReq.Seq,
                    };
                    try
                    {
                        foreach (string directory in ConvertPaths(path, dest, createDirectoryReq.Directories))
                        {
                            if(!Directory.Exists(directory))
                            {
                                Directory.CreateDirectory(directory);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        rsp.Error = e.Message;
                    }
                    m_Owner.m_Server.Send(rsp);
                    if (!CheckCommandError(rsp, downloadFailedTip, !silent))
                    {
                        yield break;
                    }
                    handle.Finished = false;
                    yield return handle;
                }
                else if (handle.Command is TransferFile.Req)
                {
                    var transferFileReq = handle.Command as TransferFile.Req;
                    TransferFile.Rsp rsp = new TransferFile.Rsp()
                    {
                        Ack = transferFileReq.Seq,
                    };
                    try
                    {
                        File.WriteAllBytes(ConvertPath(path, dest, transferFileReq.Path), transferFileReq.Content);
                    }
                    catch (Exception e)
                    {
                        rsp.Error = e.Message;
                    }
                    m_Owner.m_Server.Send(rsp);
                    if (!CheckCommandError(rsp, downloadFailedTip, !silent))
                    {
                        yield break;
                    }
                    handle.Finished = false;
                    yield return handle;
                }
                else if (handle.Command is Pull.Rsp)
                {
                    if(!silent)
                    {
                        string msg = string.Format(Constants.DownloadSuccessTip, path);
                        bool revealInExplorer = EditorUtility.DisplayDialog(Constants.WindowTitle, msg, Constants.RevealInExplorerText, Constants.OkText);
                        if(revealInExplorer)
                        {
                            EditorUtility.RevealInFinder(dest);
                        }
                    }
                    yield break;
                }
                else
                {
                    if(!silent)
                    {
                        EditorUtility.DisplayDialog(Constants.WindowTitle, downloadFailedTip + Constants.UnknownError, Constants.OkText);
                    }
                    yield break;
                }
            }
        }

        private IEnumerator Internal_Upload(string[] paths, string dest)
        {
            if (!CheckConnectStatus()) yield break;
            string uploadConfirmTip = string.Format(Constants.UploadConfirmTip, "\n", string.Join("\n", paths), dest);
            bool ret = EditorUtility.DisplayDialog(Constants.WindowTitle, uploadConfirmTip, Constants.OkText, Constants.CancelText);
            if (!ret)
            {
                yield break;
            }
            string curGoToPath = curPath;
            foreach (string p in paths)
            {
                string path = FileUtil.FixedPath(p);
                string error = null;
                string[] directories = null;
                string[] files = null;
                string curDest = FileUtil.CombinePath(dest, Path.GetFileName(path));  // dest一定是路径
                EditorReflection.CallBeforeUploadMethods(path, curDest);
                if (File.Exists(path))
                {
                    files = new string[] { path };
                }
                else
                {
                    directories = FileUtil.GetAllDirectories(path);
                    files = FileUtil.GetAllFiles(path);
                }
                if (directories != null)
                {
                    CreateDirectory.Req req = new CreateDirectory.Req()
                    {
                        Directories = ConvertPaths(path, curDest, directories),
                    };
                    CommandHandle handle = m_Owner.m_Server.Send(req);
                    yield return handle;
                    if (handle.Error != null)
                    {
                        error = handle.Error;
                    }
                    else if (!string.IsNullOrEmpty(handle.Command.Error))
                    {
                        error = handle.Command.Error;
                    }
                }
                if (error == null)
                {
                    foreach (string file in files)
                    {
                        byte[] content;
                        try
                        {
                            content = File.ReadAllBytes(file);
                        }
                        catch (Exception e)
                        {
                            error = e.Message;
                            break;
                        }
                        TransferFile.Req req = new TransferFile.Req()
                        {
                            Path = ConvertPath(path, curDest, file),
                            Content = content,
                        };
                        CommandHandle handle = m_Owner.m_Server.Send(req);
                        yield return handle;
                        if (handle.Error != null)
                        {
                            error = handle.Error;
                            break;
                        }
                        else if (!string.IsNullOrEmpty(handle.Command.Error))
                        {
                            error = handle.Command.Error;
                            break;
                        }
                    }
                }
                if (error == null)
                {
                    if(curGoToPath == curPath)
                    {
                        Refresh();
                    }
                    EditorUtility.DisplayDialog(Constants.WindowTitle, string.Format(Constants.UploadSuccessTip, path), Constants.OkText);
                }
                else
                {
                    EditorUtility.DisplayDialog(Constants.WindowTitle, string.Format(Constants.UploadFailedTip, path) + error, Constants.OkText);
                }
            }
        }

        private IEnumerator Internal_Delete(string path)
        {
            if (!CheckConnectStatus()) yield break;
            string curGoToPath = curPath;
            string deleteConfirmTip = string.Format(Constants.DeleteConfirmTip, path);
            bool ret = EditorUtility.DisplayDialog(Constants.WindowTitle, deleteConfirmTip, Constants.OkText, Constants.CancelText);
            if (!ret)
            {
                yield break;
            }
            var req = new Delete.Req(){
                Path = path,
            };
            CommandHandle handle = m_Owner.m_Server.Send(req);
            yield return handle;
            string deleteFailedTip = string.Format(Constants.DeleteFailedTip, path);
            if (!CheckHandleError(handle, deleteFailedTip) || !CheckCommandError(handle.Command, deleteFailedTip))
            {
                yield break;
            }
            if(curGoToPath == curPath)
            {
                GoTo(Directory.GetParent(curPath).ToString(), false, false, false);  // 刷新
            }
            EditorUtility.DisplayDialog(Constants.WindowTitle, string.Format(Constants.DeleteSuccessTip, path), Constants.OkText);
        }

        private IEnumerator Internal_NewFolder(string path)
        {
            if (!CheckConnectStatus()) yield break;
            var req = new NewFolder.Req(){
                Path = path,
            };
            CommandHandle handle = m_Owner.m_Server.Send(req);
            yield return handle;
            string newFolderFailedTip = string.Format(Constants.NewFolderFailedTip, path);
            if (!CheckHandleError(handle, newFolderFailedTip) || !CheckCommandError(handle.Command, newFolderFailedTip))
            {
                GoTo(curPath, false, false, false);  // 新建文件夹失败，刷新界面，移除预创建的文件夹视图
                yield break;
            }
            GoTo(curPath, false, false, false);  // 新建文件夹成功，不做提醒
        }

        private IEnumerator Internal_Rename(string path, string newPath)
        {
            if (!CheckConnectStatus()) yield break;
            var req = new Rename.Req(){
                Path = path,
                NewPath = newPath,
            };
            CommandHandle handle = m_Owner.m_Server.Send(req);
            yield return handle;
            string renameFailedTip = string.Format(Constants.RenameFailedTip, path);
            if (!CheckHandleError(handle, renameFailedTip) || !CheckCommandError(handle.Command, renameFailedTip))
            {
                // 失败了，不用刷新界面
                yield break;
            }
            GoTo(Directory.GetParent(curPath).ToString(), false, false, false);  // 重命名成功仅刷新界面，不做提醒
            // EditorUtility.DisplayDialog(Constants.WindowTitle, string.Format(Constants.RenameSuccessTip, path), Constants.OkText);
        }

        private IEnumerator Internal_UpdateStatusInfo(ConnectStatus status)
        {
            yield return null;  // 下一帧执行，保证在主线程更新UI
            if(status != ConnectStatus.Connecting)
            {
                m_Owner.Focus();
            }
            if (status != ConnectStatus.Connected)
            {
                m_Owner.m_DeviceNameLabel.text = Constants.UnknownText;
                m_Owner.m_DeviceModelLabel.text = Constants.UnknownText;
                m_Owner.m_DeviceSystemLabel.text = Constants.UnknownText;
                m_Owner.titleContent.image = TextureUtility.GetTexture("project");
                m_Owner.m_ConnectStateLabel.text = "Unconnected";
                m_Owner.m_ConnectStateLabel.style.color = Color.red;
                yield break;
            }
            CommandHandle handle = m_Owner.m_Server.Send(new QueryDeviceInfo.Req());
            yield return handle;
            if(handle.Error == null && string.IsNullOrEmpty(handle.Command.Error))
            {
                var rsp = handle.Command as QueryDeviceInfo.Rsp;
                m_Owner.m_DeviceNameLabel.text = rsp.Name;
                m_Owner.m_DeviceModelLabel.text = rsp.Model;
                m_Owner.m_DeviceSystemLabel.text = rsp.System;
                m_Owner.titleContent.image = TextureUtility.GetTexture("project active");
                m_Owner.m_ConnectStateLabel.text = "Established";
                m_Owner.m_ConnectStateLabel.style.color = Color.green;

                // 自动跳转到上次路径
                string path = ReadLastGoTo();
                if(!string.IsNullOrEmpty(path))
                {
                    GoTo(path, false, true, true);
                }
            }
        }

        public string[] ConvertPaths(string src, string dest, string[] curs)
        {
            string[] paths = new string[curs.Length];
            for (int i = 0; i < curs.Length; i++)
            {
                paths[i] = ConvertPath(src, dest, curs[i]);
            }
            return paths;
        }

        public string ConvertPath(string src, string dest, string cur)
        {
            src = FileUtil.FixedPath(src);
            dest = FileUtil.FixedPath(dest);
            cur = FileUtil.FixedPath(cur);
            return FileUtil.CombinePath(dest, cur.Replace(src, ""));
        }

        public bool CheckConnectStatus(bool displayDialog = true)
        {
            if (m_Owner.m_Server.Status == ConnectStatus.Connected)
            {
                return true;
            }
            if(displayDialog)
            {
                EditorUtility.DisplayDialog(Constants.WindowTitle, Constants.NotConnectedTip, Constants.OkText);
            }
            return false;
        }

        public bool CheckHandleError(CommandHandle handle, string tip, bool displayDialog = true)
        {
            if (handle.Error != null)
            {
                if(displayDialog)
                {
                    EditorUtility.DisplayDialog(Constants.WindowTitle, tip + "handle.Error", Constants.OkText);
                }
                return false;
            }
            return true;
        }

        public bool CheckCommandError(Command command, string tip, bool displayDialog = true)
        {
            if (!string.IsNullOrEmpty(command.Error))
            {
                Log.Error(tip + command.Error);
                if(displayDialog)
                {
                    EditorUtility.DisplayDialog(Constants.WindowTitle, tip + command.Error, Constants.OkText);
                }
                return false;
            }
            return true;
        }
    }
}