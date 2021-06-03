using UnityEngine;
using UnityEditor;
using System.Collections;
using UnityEngine.UIElements;
using URFS.Editor.UI;

namespace URFS.Editor
{   
    // class Styles
    //     {
    //         public GUIStyle bottomBarBg = "ProjectBrowserBottomBarBg";
    //         public GUIStyle topBarBg = "ProjectBrowserTopBarBg";
    //         public GUIStyle selectedPathLabel = "Label";
    //         public GUIStyle exposablePopup = GetStyle("ExposablePopupMenu");
    //         public GUIStyle exposablePopupItem = GetStyle("ExposablePopupItem");
    //         public GUIStyle lockButton = "IN LockButton";
    //         public GUIStyle separator = "ArrowNavigationRight";

    //         public GUIContent m_FilterByLabel = EditorGUIUtility.TrIconContent("FilterByLabel", "Search by Label");
    //         public GUIContent m_FilterByType = EditorGUIUtility.TrIconContent("FilterByType", "Search by Type");
    //         public GUIContent m_CreateDropdownContent = EditorGUIUtility.IconContent("CreateAddNew");
    //         public GUIContent m_SaveFilterContent = EditorGUIUtility.TrIconContent("Favorite", "Save search");
    //         public GUIContent m_PackagesVisibilityContent = EditorGUIUtility.TrIconContent("SceneViewVisibility", "Number of hidden packages, click to toggle hidden packages visibility");
    //         public GUIContent m_EmptyFolderText = EditorGUIUtility.TrTextContent("This folder is empty");
    //         public GUIContent m_SearchIn = EditorGUIUtility.TrTextContent("Search:");

    //         public Styles()
    //         {
    //             selectedPathLabel.alignment = TextAnchor.MiddleLeft;
    //         }

    //         static GUIStyle GetStyle(string styleName)
    //         {
    //             return styleName; // Implicit construction of GUIStyle
    //         }
    //     }

    public class RemoteFileExplorer : EditorWindow
    {

        static class Content
        {
            public static GUIContent Title
            {
                get
                {
                    s_Title.image = Icons.RemoteFileExplorerTabIcon;
                    return s_Title;
                }
            }
        }
        static GUIContent s_Title = new GUIContent("Remote File Explorer");

        private static string m_Host = "192.168.1.6";
        private static int m_Port = 8999;

        [MenuItem("Window/Remote File Explorer")]
        public static void ShowWindow()
        {
            Debug.Log(Content.Title.text);
            GetWindow<RemoteFileExplorer>(Content.Title.text);
        }

        void OnEnable()
        {
            Debug.Log("服务器 vvvvvvvvvvvv");
        }

        private RFSServer m_Server;

        private void Awake()
        {

        }

        public void OnConnectStatusChanged(ConnectStatus status)
        {
            switch (status)
            {
                case ConnectStatus.Connected:
                    Debug.Log("服务器 已连接。。。。。。");
                    Coroutines.Start(SendRequest());
                    break;
                case ConnectStatus.Connecting:
                    Debug.Log("服务器 正在连接。。。。。。");
                    break;
                case ConnectStatus.Disconnect:
                    Debug.Log("服务器 断开连接。。。。。。");
                    break;
            }
        }

        public IEnumerator SendRequest()
        {
            QueryDirectoryInfo.Req req = new QueryDirectoryInfo.Req
            {
                Directory = "E:/UnityProject/LastBattle/Assets/Scripts/Game/Message",
            };
            Debug.Log("auto senddd ");
            SendHandle handle = RFS.Instance.Server.Send(req.Pack());
            yield return handle;
            QueryDirectoryInfo.Rsp rsp = new QueryDirectoryInfo.Rsp();
            rsp.Unpack(handle.Rsp);
            // UDK.Output.Dump(rsp.Exists);
            // UDK.Output.Dump(rsp.SubDirectories);
            // UDK.Output.Dump(rsp.SubFiles);
        }

        private void Update()
        {
            Coroutines.Update();
            if (m_Server == null)
            {
                m_Server = RFS.Instance.Server;
                if (m_Server.Status == ConnectStatus.Disconnect)
                {
                    m_Server.OnConnectStatusChanged += OnConnectStatusChanged;
                    m_Server.Start(m_Host, m_Port);
                }
            }
        }

        // private void OnGUI() {
        //     TopToolbar();
        // }

        // void TopToolbar()
        // {
        //     GUILayout.BeginHorizontal(EditorStyles.toolbar);
        //     {
        //         float m_DirectoriesAreaWidth = 100;
        //         float listWidth = position.width - m_DirectoriesAreaWidth;
        //         float spaceBetween = 4f;
        //         bool compactMode = listWidth < 500; // We need quite some space for filtering text
        //         if (!compactMode)
        //         {
        //             spaceBetween = 10f;
        //         }

        //         // CreateDropdown();
        //         GUILayout.FlexibleSpace();

        //         GUILayout.Space(spaceBetween * 2f);
        //         // SearchField();
        //         TypeDropDown();
        //         // AssetLabelsDropDown();
        //     }
        //     GUILayout.EndHorizontal();
        // }

        // void TypeDropDown()
        // {
        //     Rect r = GUILayoutUtility.GetRect(s_Styles.m_FilterByLabel, EditorStyles.toolbarButton);
        //     if (EditorGUI.DropdownButton(r, s_Styles.m_FilterByLabel, FocusType.Passive, EditorStyles.toolbarButton))
        //     {
        //         PopupWindow.Show(r, new PopupList(m_AssetLabels));
        //     }
        // }

        private void OnDestroy()
        {
            RFS.Instance.Server.Stop();
        }
    }
}