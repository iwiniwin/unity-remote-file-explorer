using UnityEngine;
using UnityEditor;
using System.Collections;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
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

        bool m_WindowInitialized = false;
        private static string m_Host = "192.168.1.6";
        private static int m_Port = 8999;

        const string k_PackageResourcesPath = "Packages/com.iwin.remotefileexplorer/Resources/";
        const string k_UxmlFilesPath = k_PackageResourcesPath + "UXML/";
        const string k_WindowUxmlPath = k_UxmlFilesPath + "RemoteFileExplorer.uxml";
        const string k_StyleSheetsPath = k_PackageResourcesPath + "StyleSheets/";
        const string k_WindowCommonStyleSheetPath = k_StyleSheetsPath + "RemoteFileExplorer_style.uss";
        const string k_WindowLightStyleSheetPath = k_StyleSheetsPath + "RemoteFileExplorer_style_light.uss";
        const string k_WindowDarkStyleSheetPath = k_StyleSheetsPath + "RemoteFileExplorer_style_dark.uss";
        const string k_WindowNewThemingStyleSheetPath = k_StyleSheetsPath + "RemoteFileExplorer_style_newTheming.uss";

        VisualElement m_AttachToPlayerDropdownHolder;
        VisualElement m_LeftPanel;

        [MenuItem("Window/Remote File Explorer")]
        public static void ShowWindow()
        {
            GetWindow<RemoteFileExplorer>();
        }

        void OnEnable()
        {
            Debug.Log("服务器 vvvvvvvvvvvv");
        }

        void OnGUI() 
        {
            if(m_WindowInitialized)
                return;
            Init();
        }

        void Init() 
        {
            m_WindowInitialized = true;
            titleContent = EditorGUIUtility.TrTextContentWithIcon("Remote File Explorer", "Project");
        }

        private RFSServer m_Server;

        IMGUIContainer m_BreadCrumbsContainer;

        private void Awake()
        {
            var root = this.rootVisualElement;
            root.styleSheets.Add(AssetDatabase.LoadAssetAtPath<StyleSheet>(k_WindowCommonStyleSheetPath));
            if(EditorGUIUtility.isProSkin)
            {
                root.styleSheets.Add(AssetDatabase.LoadAssetAtPath<StyleSheet>(k_WindowDarkStyleSheetPath));
            }
            else
            {
                root.styleSheets.Add(AssetDatabase.LoadAssetAtPath<StyleSheet>(k_WindowLightStyleSheetPath));
            }
            root.styleSheets.Add(AssetDatabase.LoadAssetAtPath<StyleSheet>(k_WindowNewThemingStyleSheetPath));
            var windowTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(k_WindowUxmlPath);
            windowTree.CloneTree(root);
            

            var menu = root.Query<ToolbarMenu>("GoToMenu").First();
            menu.style.borderLeftWidth = 0;

            var backwardsBtn = root.Query<ToolbarButton>("backwardsInHistoryButton").First();
            var forwardsBtn = root.Query<ToolbarButton>("forwardsInHistoryButton").First();
            forwardsBtn.style.borderLeftWidth = 0;

            var breadCrumbEdit = root.Query<TextField>("breadCrumbEdit").First();
            var breadCrumbRoot = root.Query<VisualElement>("breadCrumbRoot").First();
            var breadCrumbView = root.Query<VisualElement>("breadCrumbView").First();
            m_BreadCrumbsContainer = root.Query<IMGUIContainer>("breadCrumbContainer").First();
            breadCrumbRoot.RegisterCallback<MouseUpEvent>((MouseUpEvent e) => {
                breadCrumbEdit.style.display = DisplayStyle.Flex;
                breadCrumbEdit.value = "this/sflsf/ddd/dddd";
                var l = breadCrumbEdit.Q<VisualElement>("unity-text-input");
                l.Focus();
                breadCrumbEdit.SelectAll();

                breadCrumbView.style.display = DisplayStyle.None;
            });
            breadCrumbEdit.RegisterCallback<FocusOutEvent>((FocusOutEvent e) => {
                breadCrumbEdit.style.display = DisplayStyle.None;
                breadCrumbView.style.display = DisplayStyle.Flex;
            });

            m_BreadCrumbsContainer.onGUIHandler = BreadCrumbBar;
        }

        void BreadCrumbBar()
        {
            Rect rect = new Rect(m_BreadCrumbsContainer.contentRect);
            string text = "Assets";
            bool last = false;
           
            GUIContent content = new GUIContent(text);
            GUIStyle style = last ? EditorStyles.boldLabel : EditorStyles.label;
            Vector2 size = style.CalcSize(content);
            rect.y -= 1;
            rect.width = size.x;

            if(GUI.Button(rect, content, style))
            {
                
            }

            rect.y += 1;
            
            rect.x += size.x;
            GUIStyle separatorStyle = "ArrowNavigationRight";

            Rect buttonRect = new Rect(rect.x, rect.y + (rect.height - separatorStyle.fixedHeight) / 2, separatorStyle.fixedWidth, separatorStyle.fixedHeight);
            if(GUI.Button(buttonRect, GUIContent.none, separatorStyle))
            {
                
            }
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

        private void OnDestroy()
        {
            RFS.Instance.Server.Stop();
        }
    }
}