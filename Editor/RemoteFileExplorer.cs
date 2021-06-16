using UnityEngine;
using UnityEditor;
using System.Collections;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using URFS.Editor.UI;
using PopupWindow = UnityEditor.PopupWindow;

namespace URFS.Editor
{
    public class RemoteFileExplorer : EditorWindow
    {

        class PopupExample : PopupWindowContent
        {
            bool toggle1 = true;
            bool toggle2 = true;
            bool toggle3 = true;

            public override Vector2 GetWindowSize()
            {
                return new Vector2(200, 150);
            }

            public override void OnGUI(Rect rect)
            {
                GUILayout.Label("Popup Options Example", EditorStyles.boldLabel);
                toggle1 = EditorGUILayout.Toggle("Toggle 1", toggle1);
                toggle2 = EditorGUILayout.Toggle("Toggle 2", toggle2);
                toggle3 = EditorGUILayout.Toggle("Toggle 3", toggle3);
            }

            public override void OnOpen()
            {
                Debug.Log("Popup opened: " + this);
            }

            public override void OnClose()
            {
                Debug.Log("Popup closed: " + this);
            }
        }

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
            if (!m_WindowInitialized)
                Init();
            

            if(m_StatsToggle.value)
            {
                PopupWindow.Show(GetRect(m_StatsToggle), new PopupExample());
            }
        }

        void Init()
        {
            m_WindowInitialized = true;
            titleContent = EditorGUIUtility.TrTextContentWithIcon("Remote File Explorer", "Project");
        }

        private RFSServer m_Server;

        IMGUIContainer m_BreadCrumbsContainer;
        ToolbarToggle m_StatsToggle;

        private void Awake()
        {
            var root = this.rootVisualElement;
            root.styleSheets.Add(AssetDatabase.LoadAssetAtPath<StyleSheet>(k_WindowCommonStyleSheetPath));
            if (EditorGUIUtility.isProSkin)
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

            m_StatsToggle = root.Q<ToolbarToggle>("statsToggle");
            // m_StatsToggle.RegisterValueChangedCallback(e =>
            // {
            //     // EditorGUILayout.Dropdowntogg
            //     // Debug.Log(e.newValue);
            //     PopupWindow.Show(GetRect(toggle), new PopupExample());

            // });

            var menu = root.Q<ToolbarMenu>("goToMenu");
            menu.style.borderLeftWidth = 0;
            menu.RegisterCallback<MouseUpEvent>((MouseUpEvent e) =>
            {
                var m = new GenericMenu();
                m.AddItem(new GUIContent("1111"), true, () =>
                {

                });
                m.AddItem(new GUIContent("2222"), false, () =>
                {

                });
                m.AddItem(new GUIContent("3333"), false, () =>
                {

                });
                m.DropDown(GetRect(menu));
            });

            var captureButton = root.Q<Button>("snapshot-control-area__capture-button");

            var backwardsBtn = root.Q<ToolbarButton>("backwardsInHistoryButton");
            var forwardsBtn = root.Q<ToolbarButton>("forwardsInHistoryButton");
            forwardsBtn.style.borderLeftWidth = 0;

            var breadCrumbEdit = root.Q<TextField>("breadCrumbEdit");
            var breadCrumbRoot = root.Q<VisualElement>("breadCrumbRoot");
            var breadCrumbView = root.Q<VisualElement>("breadCrumbView");
            m_BreadCrumbsContainer = root.Q<IMGUIContainer>("breadCrumbContainer");
            breadCrumbRoot.RegisterCallback<MouseUpEvent>((MouseUpEvent e) =>
            {
                breadCrumbEdit.style.display = DisplayStyle.Flex;
                breadCrumbEdit.value = "this/sflsf/ddd/dddd";
                var l = breadCrumbEdit.Q<VisualElement>("unity-text-input");
                l.Focus();
                breadCrumbEdit.SelectAll();

                breadCrumbView.style.display = DisplayStyle.None;
            });
            breadCrumbEdit.RegisterCallback<FocusOutEvent>((FocusOutEvent e) =>
            {
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

            if (GUI.Button(rect, content, style))
            {

            }

            rect.y += 1;

            rect.x += size.x;
            GUIStyle separatorStyle = "ArrowNavigationRight";

            Rect buttonRect = new Rect(rect.x, rect.y + (rect.height - separatorStyle.fixedHeight) / 2, separatorStyle.fixedWidth, separatorStyle.fixedHeight);
            if (GUI.Button(buttonRect, GUIContent.none, separatorStyle))
            {

            }
        }

        public Rect GetRect(VisualElement element)
        {
            return new Rect(element.LocalToWorld(element.contentRect.position), element.contentRect.size);
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